using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get { return _instance; } }
    private static PauseMenu _instance;

    // Rewired -------------------------------------
    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();

    public GameObject pauseMenu;
    private bool menuActive = false;
    List<bool> once = new List<bool> { false, false, false, false};
    public int cursorPosOption;
    [SerializeField] List<Transform> cPositionOption = new List<Transform>();
    public float timer = 0.5f;
    public float timer2 = 0.2f;
    List<float> playerTimer = new List<float> { 0, 0, 0, 0 };
    List<bool> boolTimer = new List<bool> { false, false, false, false };
    [SerializeField] List<FeelGood> buttonFeelOption = new List<FeelGood>();
    public GameObject cursor;
    public float deadZone;



    // Slider --------------------------------------------
    public List<Slider> Sliders = new List<Slider>();

    private void Awake()
    {
        _instance = this;
    }

    public void Start()
    {
        CheckControllers();
        loadMusicVolume();
    }

    private void CheckControllers()
    {
        joysticks = ReInput.controllers.GetJoysticks();
        for (int i = 0; i < joysticks.Count; i++)
        {
            if (!players.Contains(ReInput.players.GetPlayer(i))) 
                players.Add(ReInput.players.GetPlayer(i));
        }

        if (joysticks.Count == 0) // keyboard only, no controllers connected
        {
            players.Add(ReInput.players.GetPlayer(0));
        }

    }

    public void OpenMenu()
    {
        pauseMenu.SetActive(true);
        menuActive = true;
    }

    private void Update()
    {
        if (menuActive)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player item = players[i];
                if (item.GetButtonDown("Start"))
                {
                    pauseMenu.SetActive(false);
                    menuActive = false;
                    GameManager.Instance.UnpauseGame();
                }

                if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("Move Vertical") > 0 + deadZone)
                {

                    Debug.Log("up");
                    if (cursorPosOption > 0) cursorPosOption--;
                    once[(players.IndexOf(item))] = true;
                    setCursor();
                }

                else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("Move Vertical") < 0 - deadZone)
                {
                    Debug.Log("down");

                    if (cursorPosOption < cPositionOption.Count - 1) cursorPosOption++;
                    once[(players.IndexOf(item))] = true;
                    setCursor();

                }

                if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("Move Horizontal") > 0 + deadZone)
                {
                    Debug.Log("right");

                    changeSliders(true);
                    once[(players.IndexOf(item))] = true;
                    if (boolTimer[players.IndexOf(item)]) playerTimer[players.IndexOf(item)] = timer2;
                    else playerTimer[players.IndexOf(item)] = timer;
                }


                else if (once[(players.IndexOf(item))] == false && item.GetAxisRaw("Move Horizontal") < 0 - deadZone)
                {
                    Debug.Log("left");

                    changeSliders(false);
                    once[(players.IndexOf(item))] = true;
                    if (boolTimer[players.IndexOf(item)]) playerTimer[players.IndexOf(item)] = timer2;
                    else playerTimer[players.IndexOf(item)] = timer;
                }


                else if (item.GetAxisRaw("Move Vertical") < deadZone && item.GetAxisRaw("Move Vertical") > -deadZone
                         && item.GetAxisRaw("Move Horizontal") < deadZone && item.GetAxisRaw("Move Horizontal") > -deadZone)

                {
                    once[(players.IndexOf(item))] = false;
                    boolTimer[(players.IndexOf(item))] = false;
                    playerTimer[players.IndexOf(item)] = -2;
                    setCursor();
                }

                if (item.GetButtonDown("Confirm"))
                {
                    if (cursorPosOption == 3) ; // go to menu / return to game
                }

                if (item.GetButtonDown("Cancel"))
                {
                    // toMenu();
                }

            }

            delayToMove();
        }
    }

    void delayToMove()
    {
        for (int i = 0; i < playerTimer.Count; i++)
        {
            playerTimer[i] -= Time.deltaTime;

            if (playerTimer[i] <= 0.1f && playerTimer[i] >= -0.1f)
            {
                once[i] = false;
                boolTimer[i] = true;
            }
        }
    }
    void changeSliders(bool plus)
    {
        if (cursorPosOption < Sliders.Count)
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
        cursor.GetComponent<RectTransform>().transform.position = cPositionOption[cursorPosOption].position;

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

}
