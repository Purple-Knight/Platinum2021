using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MainMenu : MonoBehaviour
{
    // Instance -------------------------------------
    private static MainMenu _instance = null;
    public static MainMenu Instance { get => _instance; }


    // Rewired -------------------------------------
    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();


    // Object / Info -------------------------------------
    public List<GameObject> menuScreens = new List<GameObject>();
    [HideInInspector] public MenuState state;



    //Menu VAr -------------------------------------------
    int cursorPos;
    List<bool> once = new List<bool> { true, true, true, true };
    float deadZone;

    public enum MenuState
    {
        TITLE,
        MENU,
        CHARSELECT,
    }

    public void Awake()
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

            switch (state)
            {
                case MenuState.TITLE:
                    if (item.GetButtonDown("Start"))
                    {
                        toMenu();
                    }
                    break;
                case MenuState.MENU:
                    if(once[(players.IndexOf(item))] == true && item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                    {

                    }
                    break;
                case MenuState.CHARSELECT:
                    break;
                default:
                    break;
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
