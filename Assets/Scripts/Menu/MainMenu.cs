using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;

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
    public MenuState state;



    //Menu Var -------------------------------------------
    int cursorPos;
    public GameObject Cursor;
    [SerializeField] List<Transform> cPosition = new List<Transform>();
    List<bool> once = new List<bool> { false, false, false, false };
    public float deadZone;

    // ------------------------------------
    bool forChecking;

    public enum MenuState
    {
        TITLE,
        MENU,
        CHARSELECT,
    }

    public void Awake()
    {
        _instance = this;

        checkController();

    }

    public void checkController()
    {
        joysticks = ReInput.controllers.GetJoysticks(); ///////////////////check connected disconected

        for (int i = 0; i < joysticks.Count; i++)
        {
            if(!players.Contains(ReInput.players.GetPlayer(i))) players.Add(ReInput.players.GetPlayer(i));
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

                    if(once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuVertical") > 0 + deadZone)
                    {
                        if(cursorPos > 0) cursorPos--;
                        once[(players.IndexOf(item))] = true;
                        setCursor();
                    }

                    else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuVertical") < 0 - deadZone)
                    {
                        if (cursorPos < cPosition.Count - 1) cursorPos++;
                        once[(players.IndexOf(item))] = true;
                        setCursor();

                    } else if (item.GetAxisRaw("MenuVertical") < deadZone && item.GetAxisRaw("MenuVertical") > -deadZone) {
                        once[(players.IndexOf(item))] = false;
                        setCursor();
                    }



                    if (item.GetButtonDown("Confirm"))
                    {
                        switch (cursorPos)
                        {
                            case 0:
                                toCharSelect();
                                break;
                            case 1:
                                forChecking = true;
                                break;
                            case 2:
                                toTitle();
                                break;
                            default:
                                break;
                        }
                    }

                    break;


                case MenuState.CHARSELECT:
                    break;
                default:
                    break;
            }

        }

        if (forChecking)
        {
            forChecking = false;
            checkController();
        }
    }


    void setCursor()
    {
        Cursor.GetComponent<RectTransform>().transform.position = cPosition[cursorPos].position;
    }

    public void toTitle()
    {
        state = MenuState.TITLE;
        changeScreen(0);
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
