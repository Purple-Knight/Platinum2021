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
    int cursorPosOption;
    public GameObject Cursor;
    [SerializeField] List<Transform> cPosition = new List<Transform>();
    [SerializeField] List<Transform> cPositionOption = new List<Transform>();
    List<bool> once = new List<bool> { false, false, false, false };
    public float deadZone;


    // Slider --------------------------------------------
    public List<Slider> Sliders = new List<Slider>();

    // ------------------------------------
    bool forChecking;

    public enum MenuState
    {
        TITLE,
        MENU,
        CHARSELECT,
        OPTION,
    }

    public void Awake()
    {
        _instance = this;

        checkController();

    }

    private void Start()
    {
        loadMusicVolume();
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
                                toOption();
                                break;
                            case 3:
                                toTitle();
                                break;
                            default:
                                break;
                        }
                    }

                    break;






                case MenuState.CHARSELECT:
                    break;




                case MenuState.OPTION:

                    if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuVertical") > 0 + deadZone)
                    {
                        if (cursorPosOption > 0) cursorPosOption--;
                        once[(players.IndexOf(item))] = true;
                        setCursor();
                    }

                    else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuVertical") < 0 - deadZone)
                    {
                        if (cursorPosOption < cPositionOption.Count - 1) cursorPosOption++;
                        once[(players.IndexOf(item))] = true;
                        setCursor();

                    }

                    if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                    {
                        changeSliders(true);
                        once[(players.IndexOf(item))] = true;
                    }


                    else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                    {
                        changeSliders(false);
                        once[(players.IndexOf(item))] = true;
                    }


                    else if (item.GetAxisRaw("MenuVertical") < deadZone && item.GetAxisRaw("MenuVertical") > -deadZone
                        && item.GetAxisRaw("MenuHorizontal") < deadZone && item.GetAxisRaw("MenuHorizontal") > -deadZone)
                    {
                        once[(players.IndexOf(item))] = false;
                        setCursor();
                    }



                    if (item.GetButtonDown("Confirm"))
                    {
                        toMenu();
                    }

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




    void changeSliders(bool plus)
    {
        if(cursorPosOption < Sliders.Count)
        {
            if (plus) Sliders[cursorPosOption].value += 5;
            else Sliders[cursorPosOption].value -= 5;

            SetVolumeGen();
            SetVolumeMusic();
            SetVolumeSFX();
        }
    }

    void setCursor()
    {
        if(state == MenuState.MENU) Cursor.GetComponent<RectTransform>().transform.position = cPosition[cursorPos].position;
        if(state == MenuState.OPTION) Cursor.GetComponent<RectTransform>().transform.position = cPositionOption[cursorPosOption].position;
    }


    // Scene ---------------------------------------------------------------------
    #region Change Scene /

    public void toTitle()
    {
        state = MenuState.TITLE;
        changeScreen(0, false);
        cursorPos = 0;
    }

    public void toMenu()
    {
        state = MenuState.MENU;
        changeScreen(1, true);
        cursorPosOption = 0;
    }

    public void toCharSelect()
    {
        state = MenuState.CHARSELECT;
        changeScreen(2, false);
        CharacterSelection.Instance.asignPlayers(players);
    }

    public void toOption()
    {
        state = MenuState.OPTION;
        changeScreen(3, true);
    }


    public void changeScreen(int iref, bool cursorOn)
    {
        for (int i = 0; i < menuScreens.Count; i++)
        {
            if (i == iref) menuScreens[i].SetActive(true);
            else menuScreens[i].SetActive(false);

            
        }
        if (cursorOn) Cursor.SetActive(true);
        else Cursor.SetActive(false);
    }

    #endregion

    // Volume ---------------------------------------------------------------------
    #region VOLUME 
    public void SetVolumeGen()
    {

        AkSoundEngine.SetRTPCValue("User_RTPC_Main_Volume", Sliders[0].value);
        PlayerPrefs.SetFloat("MainVolume", Sliders[0].value);
    }

    public void SetVolumeMusic()
    {
     
        AkSoundEngine.SetRTPCValue("User_RTPC_Music_Volume", Sliders[1].value);
        PlayerPrefs.SetFloat("MusicVolume", Sliders[1].value);
    }

    public void SetVolumeSFX()
    {

        AkSoundEngine.SetRTPCValue("User_RTPC_SFX_Volume", Sliders[2].value);
        PlayerPrefs.SetFloat("SFXVolume", Sliders[2].value);
    }

    void loadMusicVolume()
    {
        Sliders[0].value = PlayerPrefs.GetFloat("MainVolume");
        Sliders[1].value = PlayerPrefs.GetFloat("MusicVolume");
        Sliders[2].value = PlayerPrefs.GetFloat("SFXVolume");

        SetVolumeGen();
        SetVolumeMusic();
        SetVolumeSFX();
    }
    #endregion
}
