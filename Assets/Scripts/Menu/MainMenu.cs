using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MainMenu : MonoBehaviour
{
    private static MainMenu _instance = null;
    public static MainMenu Instance { get => _instance; }



    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();

    public List<GameObject> menuScreens = new List<GameObject>();

    public MenuState state;

    public enum MenuState
    {
        TITLE,
        MENU,
        CHARSELECT,
    }

    private void Awake()
    {
        _instance = this;

        joysticks = ReInput.controllers.GetJoysticks(); ///////////////////check connected disconected

        for (int i = 0; i < joysticks.Count; i++)
        {
            players.Add(ReInput.players.GetPlayer(i));
        }

    }

    void Update()
    {
        foreach (var item in players)
        {
            if (state == MenuState.TITLE)
            {
                if (item.GetButtonDown("Start"))
                {
                    toMenu();
                }
            }

            if(state == MenuState.MENU)
            {

            }

            if(state == MenuState.CHARSELECT)
            {

            }
        }
    }




    public void toMenu()
    {
        state = MenuState.MENU;
        changeScreen(1);
    }

    public void toCharSelect()
    {
        state = MenuState.CHARSELECT;
        changeScreen(2);
        CharacterSelection.Instance.asignPlayers(players);
    }


    public void changeScreen(int iref)
    {
        for (int i = 0; i < menuScreens.Count; i++)
        {
            if (i == iref) menuScreens[i].SetActive(true);
            else menuScreens[i].SetActive(false);
            
        }
    }
}
