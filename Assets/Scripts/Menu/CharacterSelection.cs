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
        foreach (var item in players)
        {
            if (item.GetButtonDown("Confirm")) // Confirme fct
            {
                if (!playersActual.Contains(item))
                {
                    playersActual.Add(item);
                    showPlayerSelect();
                }
                else
                {
                    charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeOK(true);
                    checkIfEveryoneIsReady();
                }
            }



            if (playersActual.Contains(item)) // selectioh characters
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

                if (item.GetButtonDown("Start")){
                    goToPlay();
                }


                if (item.GetButtonDown("Cancel"))
                {
                    charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeOK(false);
                    checkIfEveryoneIsReady();
                }
            }
        }
    }


    public void showPlayerSelect()
    {
        for (int i = 0; i < playersActual.Count; i++)
        {
            charPortrait[i].SetActive(true);
        }
    }



    public void goToPlay()
    {
        if (playersActual.Count >= 2 && canStart)
        {
            foreach (var item in playersActual)
            {
                var correct = true;

                foreach (var item2 in playersActual)
                {
                    if (item != item2)
                    {


                        if (charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idChar == charPortrait[playersActual.IndexOf(item2)].GetComponent<CharBox>().idChar &&
                            charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idColor == charPortrait[playersActual.IndexOf(item2)].GetComponent<CharBox>().idColor)
                        {
                            correct = false;

                        }
                    }
                }

                if (correct)
                {
                    var charportrait = charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>();

                    pd.numberOfPlayer = playersActual.Count;
                    pd.allPlayerData[playersActual.IndexOf(item)].myCharID = charportrait.idChar;
                    pd.allPlayerData[playersActual.IndexOf(item)].myColorID = charportrait.colorList[charportrait.idColor];
                    pd.allPlayerData[playersActual.IndexOf(item)].playerControllerID = (item.id);
                    Debug.Log("Start Game");
                    saveData();
                }
                else
                {
                    Debug.Log("Error !!!");
                    break;
                }
            }
        }
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
            var chara = charPortrait[i].GetComponent<CharBox>();
            charPortrait[i].SetActive(true);
            chara.idChar = pd.allPlayerData[i].myCharID;
            chara.idColor = chara.colorList.IndexOf(pd.allPlayerData[i].myColorID);
            playersActual.Add(players[pd.allPlayerData[i].playerControllerID]);
        }
    }





    public void checkIfEveryoneIsReady()
    {
        bool letsGo = true;

        if (playersActual.Count >= 2)
        {
            for (int i = 0; i < playersActual.Count; i++)
            {
                if (!charPortrait[i].GetComponent<CharBox>().ok) letsGo = false;
                Debug.Log(letsGo);
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