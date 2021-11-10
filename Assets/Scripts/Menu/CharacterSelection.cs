using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelection : MonoBehaviour
{
    //Instance ----------------------------------------
    private static CharacterSelection _instance = null;
    public static CharacterSelection Instance { get => _instance; }


    //Rewired ----------------------------------------
    private List<Player> players = new List<Player>();
    private List<Player> playersActual = new List<Player>();


    // Object / Variables -------------------------------------
    public List<GameObject> charPortrait = new List<GameObject>();
    public float deadZone;
    public GameObject buttonStart;
    bool canStart;

    // Save Data -------------------------------------
    [SerializeField] PlayersData pd;

    private void Awake()
    {
        _instance = this;
        checkIfEveryoneIsReady();
    }

    public void asignPlayers(List<Player> pList)
    {
        players = pList;
    }


    public void Update()
    {
        if (MainMenu.Instance.state == MainMenu.MenuState.CHARSELECT)
        {
            foreach (var item in players)
            {



                if (item.GetButtonDown("Confirm")) // Confirme fct ---------------------------------------------------------------
                {

                    if (!playersActual.Contains(item))
                    {
                        foreach (var item1_5 in playersActual)
                        {
                            Debug.Log(item1_5);

                        }

                        if (playersActual.Count != 0)
                        {
                            var toRepace = false;
                            var indexToReplace = 0;

                            var toadd = false;

                            foreach (var item2 in playersActual)
                            {
                                if (item2 == null)
                                {
                                    toRepace = true;
                                    indexToReplace = playersActual.IndexOf(item2);
                                    break;
                                }
                                else if (playersActual[playersActual.Count - 1] == item2 && playersActual.Count < 4) toadd = true;
                            }

                            if (toadd) playersActual.Add(item);

                            if (toRepace) playersActual[indexToReplace] = item;
                        }
                        else
                        {
                            playersActual.Add(item);
                        }

                        showPlayerSelect();
                    }
                    else
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeOK(true);
                    }

                    checkIfEveryoneIsReady();

                }




                if (playersActual.Contains(item)) // selectioh characters --------------------------------
                {

                    #region LEFT/RIGHT/UP/DOWN

                    if (item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeColor(true);
                    }
                    else if (item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeColor(false);
                    }


                    else if (item.GetAxisRaw("MenuVertical") > 0 + deadZone)
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeChar(true);
                    }
                    else if (item.GetAxisRaw("MenuVertical") < 0 - deadZone)
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeChar(false);
                    }


                    else
                    {
                        charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().once = false;
                    }

                    #endregion

                    if (item.GetButtonDown("Start"))
                    {
                        goToPlay();
                    }


                    if (item.GetButtonDown("Cancel"))
                    {
                        if (charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().ok == true)
                        {
                            charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeOK(false);
                        }
                        else
                        {
                            playersActual[playersActual.IndexOf(item)] = null;
                            showPlayerSelect();
                        }

                        checkIfEveryoneIsReady();
                    }
                }
                else
                {
                    if (item.GetButtonDown("Cancel"))
                    {
                        MainMenu.Instance.toMenu();
                    }
                }
            }
        }
    }


    public void showPlayerSelect()
    {
        for (int i = 0; i < 4; i++)
        {
            if (playersActual.Count > i && playersActual[i] != null)
            {
                charPortrait[i].SetActive(true);
                charPortrait[i].GetComponent<CharBox>().isAssigned = true;
            }
            else
            {
                charPortrait[i].SetActive(false);
                charPortrait[i].GetComponent<CharBox>().isAssigned = false;
            }
        }
    }



    public void goToPlay()
    {
        if (playersActual.Count >= 1 && canStart)//------------------------------------------------------------------------------------------------------------------------------------------
        {
            foreach (var item in playersActual)
            {
                var correct = true;

                foreach (var item2 in playersActual)
                {


                    if (item != null && item2 != null && item != item2)
                    {

                        if (charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idChar == charPortrait[playersActual.IndexOf(item2)].GetComponent<CharBox>().idChar &&
                            charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idColor == charPortrait[playersActual.IndexOf(item2)].GetComponent<CharBox>().idColor)
                        {
                            correct = false;
                            Debug.Log(item.name + " " + item2.name);
                            break;

                        }
                    }
                }



                if (correct)
                {
                    saveALL(item);
                    saveData();
                    MainMenu.Instance.toMapSelect();
                }
                else
                {
                    Debug.Log("Error !!!");
                    break;
                }
            }
        }
    }

    void saveALL(Player item)
    {

        var charportrait = charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>();

        pd.numberOfPlayer = playersActual.Count;
        pd.allPlayerData[playersActual.IndexOf(item)].myCharID = charportrait.idChar;
        pd.allPlayerData[playersActual.IndexOf(item)].myColorID = charportrait.colorList[charportrait.idColor];
        if(charportrait.isAssigned) pd.allPlayerData[playersActual.IndexOf(item)].playerControllerID = (item.id);
        pd.allPlayerData[playersActual.IndexOf(item)].isActivated= (charportrait.isAssigned);
        Debug.Log("Start Game");
    }

    void saveData()
    {
        SaveData.Save(pd);
    }


    public void loadPD()
    {
        pd = SaveData.Load();

        for (int i = 0; i < pd.numberOfPlayer; i++)
        {
            if (pd.allPlayerData[i].isActivated)
            {
                var chara = charPortrait[i].GetComponent<CharBox>();
                charPortrait[i].SetActive(true);
                chara.idChar = pd.allPlayerData[i].myCharID;
                chara.idColor = chara.colorList.IndexOf(pd.allPlayerData[i].myColorID);
                playersActual.Add(players[pd.allPlayerData[i].playerControllerID]);
            }
        }
    }





    public void checkIfEveryoneIsReady()
    {
        bool letsGo = true;

        if (playersActual.Count >= 1)//-------------------------------------------------------------------------------------------------------------------------------------------
        {
            for (int i = 0; i < 4; i++)
            {
                if (charPortrait[i].GetComponent<CharBox>().isAssigned)
                {
                    if (!charPortrait[i].GetComponent<CharBox>().ok) letsGo = false;
                }
            }

            if (letsGo)
            {
                buttonStart.SetActive(true);
                canStart = true;
            }
            else
            {
                buttonStart.SetActive(false);
                canStart = false;
            }
        }
        else
        {
            buttonStart.SetActive(false);
            canStart = false;
        }
    }

}