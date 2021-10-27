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
        
    private void Awake()
    {
        _instance = this;
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
            }



            if (playersActual.Contains(item)) // selectioh characters
            {

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
        foreach (var item in playersActual)
        {
            Debug.Log("Controller " + item.name + " choose char id " + charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idChar 
                    + " choose color id" + charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().idColor);
        }
    }
}
