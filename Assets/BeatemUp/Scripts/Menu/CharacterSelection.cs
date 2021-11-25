using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    public List<MapSelector> charPortraitTrue = new List<MapSelector>();
    public List<TextMeshProUGUI> namesZone= new List<TextMeshProUGUI>();
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
                        var n1 = charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>();
                        var n2 = charPortraitTrue[playersActual.IndexOf(item)].GetComponent<Image>();

                        n1.changeOK(true);
                    }

                    checkIfEveryoneIsReady();

                }




                if (playersActual.Contains(item)) // selectioh characters --------------------------------
                {

                    var n1 = charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>();
                    var n2 = charPortraitTrue[playersActual.IndexOf(item)].GetComponent<Image>();

                    if (!n1.ok)
                    {

                        #region LEFT/RIGHT/UP/DOWN

                        if (item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                        {
                            if (!n1.once) charPortraitTrue[playersActual.IndexOf(item)].downValue();
                            n1.changeChar(true);
                            namesZone[playersActual.IndexOf(item)].text = n1.characterList[n1.idChar].name;
                        }
                        else if (item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                        {
                            if (!n1.once) charPortraitTrue[playersActual.IndexOf(item)].upValue();
                            n1.changeChar(false);
                            namesZone[playersActual.IndexOf(item)].text = n1.characterList[n1.idChar].name;
                        }


                        else if (item.GetAxisRaw("MenuVertical") > 0 + deadZone)
                        {
                            if (!n1.once)
                            {
                                n1.changeColor(true);
                                for (int i = 0; i < n2.gameObject.GetComponent<MapSelector>().GoList.Count; i++)
                                {
                                    n2.gameObject.GetComponent<MapSelector>().GoList[i].GetComponent<Image>().color = n1.colorList[n1.idColor];

                                }

                                n2.gameObject.GetComponent<MapSelector>().moveToGO();
                            }
                        }
                        else if (item.GetAxisRaw("MenuVertical") < 0 - deadZone)
                        {
                            if (!n1.once)
                            {
                                n1.changeColor(false);
                                for (int i = 0; i < n2.gameObject.GetComponent<MapSelector>().GoList.Count; i++)
                                {
                                    n2.gameObject.GetComponent<MapSelector>().GoList[i].GetComponent<Image>().color = n1.colorList[n1.idColor];

                                }
                                n2.gameObject.GetComponent<MapSelector>().moveToGO();
                            }
                        }


                        else
                        {
                            n1.once = false;
                        }

                        #endregion

                    }

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
                            Debug.Log("etape 1");
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
                charPortraitTrue[i].gameObject.SetActive(true);
                charPortrait[i].GetComponent<CharBox>().isAssigned = true;
            }
            else
            {
                charPortrait[i].SetActive(false);
                charPortraitTrue[i].gameObject.SetActive(false);
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
                    SceneManager.LoadScene("TestLevelGen");
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