using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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



    // Camera --------------------------------------------
    [Header("Camera")]
    public List<Transform> camPos = new List<Transform>();
    public CameraFocus cam;

    //Title -------------------------------------
    [Header("Title")]
    [SerializeField] List<Feel> doorSystem = new List<Feel>();
    [SerializeField] GameObject pressStart;

    //Menu Var -------------------------------------------
    [Header("Var")]
    bool canInteract;
    [HideInInspector] public float timeToInteract;
    
    
    int cursorPos;
    int cursorPosOption;
    public GameObject Cursor;
    [SerializeField] List<Transform> cPosition = new List<Transform>();
    [SerializeField] List<Feel> buttonFeel = new List<Feel>();
    [SerializeField] List<Transform> cPositionOption = new List<Transform>();
    [SerializeField] List<FeelGood> buttonFeelOption = new List<FeelGood>();
    List<bool> once = new List<bool> { false, false, false, false};
    public float timer = 0.5f;
    public float timer2 = 0.2f;
    List<float> playerTimer = new List<float> { 0, 0, 0, 0};
    List<bool> boolTimer = new List<bool> { false, false, false, false};
    public float deadZone;

    [Header("Map")]
    public int cursorPosMap;
    [SerializeField] List<Feel> buttonFeelMap = new List<Feel>();
    
    //Load level -------------------------------------
    [Header("Loader level")] 
    public GameObject fadeGO;
    public GameObject loadingScreen;
    
    private LevelLoader loader;

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
        CREDITS,
        MAPSELECT,
    }

    public void Awake()
    {
        _instance = this;

        checkController();

    }

    private void Start()
    {
        loader = GetComponent<LevelLoader>();
        loadMusicVolume();
    }

    public void checkController()
    {
        Debug.Log("Check Controller OK");
        joysticks = ReInput.controllers.GetJoysticks(); ///////////////////check connected disconected

        for (int i = 0; i < joysticks.Count; i++)
        {
            if(!players.Contains(ReInput.players.GetPlayer(i))) players.Add(ReInput.players.GetPlayer(i));
        }
        
        if(joysticks.Count == 0) // keyboard only, no controllers connected
        {
            players.Add(ReInput.players.GetPlayer(0));
        }
    }


    void Update()
    {
        timeWaitInteract();
        
        if (canInteract)
        {
            foreach (var item in players)
            {

                switch (state)
                {
                    case MenuState.TITLE:
                        if (item.GetButtonDown("Start"))
                        {
                            foreach (var item2 in doorSystem)
                            {
                                item2.launch = true;
                            }

                            pressStart.SetActive(false);
                            state = MenuState.MENU;
                            toMenu();
                            timeToInteract = 0.5f;
                        }

                        break;




                    case MenuState.MENU:

                        if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                        {
                            if (cursorPos > 0) cursorPos--;
                            once[(players.IndexOf(item))] = true;
                            setCursor();
                        }

                        else if (once[(players.IndexOf(item))] == false &&
                                 item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                        {
                            if (cursorPos < cPosition.Count - 1) cursorPos++;
                            once[(players.IndexOf(item))] = true;
                            setCursor();

                        }
                        else if (item.GetAxisRaw("MenuHorizontal") < deadZone &&
                                 item.GetAxisRaw("MenuHorizontal") > -deadZone)
                        {
                            once[(players.IndexOf(item))] = false;
                            setCursor();
                        }



                        if (item.GetButtonDown("Confirm"))
                        {
                            switch (cursorPos)
                            {
                                case 0:
                                    toCharSelect();
                                    timeToInteract = 0.3f;
                                    break;
                                case 1:
                                    toOption();
                                    timeToInteract = 0.3f;
                                    break;
                                case 2:
                                    toCredits();
                                    timeToInteract = 0.3f;
                                    break;
                                case 3:
                                    toQuit();
                                    break;
                                default:
                                    break;
                            }
                        }

                        break;


                    case MenuState.CHARSELECT:
                        break;

                    case MenuState.CREDITS:
                        if (item.GetButtonDown("Cancel"))
                        {
                            toMenu();
                            timeToInteract = 0.3f;
                        }

                        break;


                    case MenuState.OPTION:

                        if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuVertical") > 0 + deadZone)
                        {
                            if (cursorPosOption > 0) cursorPosOption--;
                            once[(players.IndexOf(item))] = true;
                            setCursor();
                        }

                        else if (once[(players.IndexOf(item))] == false &&
                                 item.GetAxisRaw("MenuVertical") < 0 - deadZone)
                        {
                            if (cursorPosOption < cPositionOption.Count - 1) cursorPosOption++;
                            once[(players.IndexOf(item))] = true;
                            setCursor();

                        }

                        if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                        {
                            changeSliders(true);
                            once[(players.IndexOf(item))] = true;
                            if (boolTimer[players.IndexOf(item)]) playerTimer[players.IndexOf(item)] = timer2;
                            else playerTimer[players.IndexOf(item)] = timer;
                        }


                        else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                        {
                            changeSliders(false);
                            once[(players.IndexOf(item))] = true;
                            if (boolTimer[players.IndexOf(item)]) playerTimer[players.IndexOf(item)] = timer2;
                            else playerTimer[players.IndexOf(item)] = timer;
                        }


                        else if (item.GetAxisRaw("MenuVertical") < deadZone && item.GetAxisRaw("MenuVertical") >
                                                                            -deadZone && item.GetAxisRaw("MenuHorizontal") <
                                                                            deadZone && item.GetAxisRaw("MenuHorizontal") > -deadZone)
                        {
                            once[(players.IndexOf(item))] = false;
                            boolTimer[(players.IndexOf(item))] = false;
                            playerTimer[players.IndexOf(item)] = -2;
                            setCursor();
                        }



                        if (item.GetButtonDown("Confirm"))
                        {
                            if (cursorPosOption == 3) checkController();
                        }

                        if (item.GetButtonDown("Cancel"))
                        {
                            toMenu();
                            timeToInteract = 0.3f;
                        }

                        break;


                    case MenuState.MAPSELECT:

                        if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("MenuHorizontal") < 0 - deadZone)
                        {
                            if (cursorPosMap > 0)
                            {
                                cursorPosMap--;
                                setCursor();
                            }

                            once[(players.IndexOf(item))] = true;

                        }

                        else if (once[(players.IndexOf(item))] == false &&
                                 item.GetAxisRaw("MenuHorizontal") > 0 + deadZone)
                        {
                            if (cursorPosMap < buttonFeelMap.Count - 1)
                            {
                                cursorPosMap++;
                                setCursor();
                            }

                            once[(players.IndexOf(item))] = true;

                        }
                        else if (item.GetAxisRaw("MenuHorizontal") < deadZone &&
                                 item.GetAxisRaw("MenuHorizontal") > -deadZone)
                        {
                            once[(players.IndexOf(item))] = false;
                        }

                        if (item.GetButtonDown("Confirm"))
                        {
                            //SceneManager.LoadScene("TestLevelGen");
                            if (cursorPosMap == 0)
                            {
                                RhythmManager.Instance.level = Level.Medium;
                            }
                            else
                            {
                                RhythmManager.Instance.level = Level.Hard;
                            }
                            fadeGO.SetActive(true);
                            StartCoroutine(loadTime());
                            timeToInteract = 0.7f;
                        }

                        if (item.GetButtonDown("Cancel"))
                        {
                            toCharSelect();
                            timeToInteract = 0.5f;
                        }

                        break;



                    default:
                        break;
                }

            }
        }

        if (forChecking)
        {
            forChecking = false;
            checkController();
        }

        delayToMove();
    }


    IEnumerator loadTime()
    {
        RhythmManager.Instance.StopAllMusic();
        RhythmManager.Instance.inMenu = false;
        yield return new WaitForSeconds(0.5f);
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        fadeGO.SetActive(false);
        loader.LoadLevel("TestLevelGen");
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
        if (state == MenuState.MENU) 
        {
            Cursor.GetComponent<RectTransform>().transform.position = cPosition[cursorPos].position;

            for (int i = 0; i < buttonFeel.Count; i++)
            {
                if(i == cursorPos && buttonFeel[i].onOff)
                {
                    //buttonFeel[i].playOnAwake = true;
                    buttonFeel[i].launch = true;
                }
                else if( i != cursorPos && !buttonFeel[i].onOff)
                {
                    //buttonFeel[i].playOnAwake = false;
                    buttonFeel[i].launch = true;
                }
            }
        }
        else if (state == MenuState.OPTION)
        {
            Cursor.GetComponent<RectTransform>().transform.position = cPositionOption[cursorPosOption].position;
            
            for (int i = 0; i < buttonFeelOption.Count; i++)
            {
                if (i == cursorPosOption)
                {
                    buttonFeelOption[i].playOnAwake = true;
                }
                else
                {
                    buttonFeelOption[i].playOnAwake = false;
                }
            }
        }
        else if (state == MenuState.MAPSELECT) 
        {
            if (cursorPosMap == 0)
            {
                buttonFeelMap[0].onOff = true;
                buttonFeelMap[0].launch = true;
                buttonFeelMap[1].onOff = false;
                buttonFeelMap[1].launch = true;
            }
            else
            {
                buttonFeelMap[1].onOff = true;
                buttonFeelMap[1].launch = true;
                buttonFeelMap[0].onOff = false;
                buttonFeelMap[0].launch = true;
            }
        }
        

    }


    void delayToMove()
    {
        for (int i = 0; i < playerTimer.Count; i++)
        {
            playerTimer[i] -= Time.deltaTime;

            if(playerTimer[i] <= 0.1f && playerTimer[i] >= -0.1f)
            {
                once[i] = false;
                boolTimer[i] = true;
            }
        }
    }

    void timeWaitInteract()
    {
        if(timeToInteract > 0)
        {
            canInteract = false;
            timeToInteract -= Time.deltaTime;
        }
        else
        {
            canInteract = true;
        }
    }
    
    // Scene ---------------------------------------------------------------------
    #region Change Scene /

    public void toQuit()
    {
        Application.Quit();
        /*state = MenuState.TITLE;
        changeScreen(0, false);
        cursorPos = 0;*/
    }

    public void toMenu()
    {
        state = MenuState.MENU;
        changeScreen(1, false);
        cursorPosOption = 0;
    }

    public void toCredits()
    {
        state = MenuState.CREDITS;
        changeScreen(2, false);
    }

    public void toOption()
    {
        state = MenuState.OPTION;
        changeScreen(3, true);
    }
    
    public void toCharSelect()
    {
        state = MenuState.CHARSELECT;
        changeScreen(4, false);
        CharacterSelection.Instance.asignPlayers(players);
    }


    public void toMapSelect()
    {
        state = MenuState.MAPSELECT;
        changeScreen(5, false);
        setCursor();
    }


    public void changeScreen(int iref, bool cursorOn)
    {
        /*for (int i = 0; i < menuScreens.Count; i++)
        {
            if (i == iref) menuScreens[i].SetActive(true);
            else menuScreens[i].SetActive(false);

            
        }*/

        cam.target = camPos[iref];


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
        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            Sliders[0].value = 50;
            Sliders[1].value = 50;
            Sliders[2].value = 50;
        }
        else
        {
            Sliders[0].value = PlayerPrefs.GetFloat("MainVolume");
            Sliders[1].value = PlayerPrefs.GetFloat("MusicVolume");
            Sliders[2].value = PlayerPrefs.GetFloat("SFXVolume");

        }
        SetVolumeGen();
        SetVolumeMusic();
        SetVolumeSFX();
    }
    #endregion
}
