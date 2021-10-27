using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharacterSelection : MonoBehaviour
{
    private static CharacterSelection _instance = null;
    public static CharacterSelection Instance { get => _instance; }

    private List<Player> players = new List<Player>();
    private List<Player> playersActual = new List<Player>();

    public List<GameObject> charPortrait = new List<GameObject>();
        
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
            if (item.GetButtonDown("Confirm"))
            {
                if (!playersActual.Contains(item))
                {
                    playersActual.Add(item);
                    showPlayerSelect();
                }
            }




            if (item.GetAxisRawPrev("MenuHorizontal") > 0)
            {
                charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeColor(true);
            }
            else if (item.GetAxisRawPrev("MenuHorizontal") < 0)
            {
                charPortrait[playersActual.IndexOf(item)].GetComponent<CharBox>().changeColor(false);
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
}
