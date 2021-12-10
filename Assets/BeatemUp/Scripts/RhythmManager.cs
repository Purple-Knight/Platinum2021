using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get { return _instance; } }
    private static RhythmManager _instance;

    public delegate void OnMusicBeat();
    public OnMusicBeat onMusicBeatDelegate;

    [Header("Music Selection")]
    [SerializeField] AK.Wwise.Event menuMusicEvent;
    [SerializeField] AK.Wwise.Event gameMusicEvent;
    [SerializeField] AK.Wwise.Event stopAllMusicEvent;
    [SerializeField] AK.Wwise.Event pauseEvent;
    [SerializeField] AK.Wwise.Event unpauseEvent;

    [Header("Beat")]
    bool onceAtStart;
    public bool inMenu;

    public float numberOfBeat;
    [SerializeField] private float timeBeforeStart;

    public float beatDuration;
    public List<Song> duration = new List<Song>();

    public UnityEvent InstantiateBeat;
    public UnityEvent EndOfMusic;

    [Header("Beat Window")]
    private float perfectBufferTime;
    private float bufferTime;
    public float halfBeatTime;
    
    private float timerInBetweenBeat = 0;

    [Header("Buffer Time")]
    [SerializeField] public Level level;
    [SerializeField] float hardPercentage;
    [SerializeField] float mediumPercentage;
    [SerializeField] float easyPercentage;

    [SerializeField] float BPM = 115;

    //Wwise var
    [HideInInspector]public AK.Wwise.Switch characterSwitch;
    [SerializeField] private AK.Wwise.Event deathSound;
    [SerializeField] private AK.Wwise.Event shotSound;
    [SerializeField] public AK.Wwise.RTPC gtrVolume;
    [SerializeField] private AK.Wwise.RTPC harpeVolume;
    [SerializeField] private AK.Wwise.RTPC syntheVolume;

    private void Awake()
    {
        if (_instance != null)
        {
            
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

    }

    private void Start()
    {
        if (inMenu)
            StartMenu();
        else
        {
            gtrVolume.SetGlobalValue(0);
                harpeVolume.SetGlobalValue(0);
                syntheVolume.SetGlobalValue(0);
        }

    }

    public void StartMenu() {
        inMenu = true;
        menuMusicEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

    public void StartGame()
    {
        onceAtStart = false;
        StartCoroutine(delayStart());
    }


    public void PauseGame()
    {
        pauseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }
    
    public void UnpauseGame()
    {
        unpauseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(timeBeforeStart);
        gameMusicEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

    public void PlayDeathSound()
    {
        deathSound.Post(gameObject);
    }

    public void PlayShotSound()
    {
        shotSound.Post(gameObject);
    }

    public void setTrackVolume(Track track, float volume)
    {
       volume =  Mathf.Clamp(volume, 0f, 100f);
        switch (track)
        {
            case Track.GTR:
                gtrVolume.SetGlobalValue(volume);
        //Debug.Log(volume + " ----- " +gtrVolume.GetGlobalValue());
                break;
            case Track.HARPE:
                harpeVolume.SetGlobalValue(volume);
                break;
            case Track.SYNTHE:
                syntheVolume.SetGlobalValue(volume);
                break;
            default:
                break;
        }

    }

    private void Update()
    {
        timerInBetweenBeat += Time.deltaTime;
    }

    public Timing AmIOnBeat()
    {
        if (timerInBetweenBeat >= (beatDuration - perfectBufferTime))
        {
            //Perfect before
            //Debug.Log("Perfet before");
            return Timing.PERFECT;
        }
        else if (timerInBetweenBeat >= (beatDuration - bufferTime))
        {
            //Before
            //Debug.Log("Before");

            return Timing.BEFORE;
        }
        else if (timerInBetweenBeat > bufferTime) // && timerInBetweenBeat < (beatDuration - bufferTime)
        {
            //Miss
            //Debug.Log("Miss");

            return Timing.MISS;
        }
        else if (timerInBetweenBeat > perfectBufferTime)
        {
            //After
            //Debug.Log("After");

            return Timing.AFTER;
        }
        else if (timerInBetweenBeat <= perfectBufferTime)
        {
            //Debug.Log("Perfet After");
            //Perfect After
            return Timing.PERFECT;
        }
        
        return Timing.NULL;
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
        if (in_type == AkCallbackType.AK_MusicSyncBeat)
        {
            beatDuration = info.segmentInfo_fBeatDuration;

            if (!onceAtStart)
            {
                onceAtStart = true;
                stopAllMusicEvent.Post(gameObject);
                StartCoroutine(beforeStart());
                numberOfBeat = duration[0].duration / beatDuration;    //    stopper les x derniers beat en fct dde la time line ( check le nombre de beat dans la chanson et la time line)
                InstantiateBeat?.Invoke();

                //Window Rythm
                halfBeatTime = beatDuration / 2;

                switch (level)
                {
                    case Level.Easy:
                        bufferTime = halfBeatTime * (easyPercentage /100);
                        break;
                    case Level.Medium:
                        bufferTime = halfBeatTime * (mediumPercentage / 100);
                        break;
                    case Level.Hard:
                        bufferTime = halfBeatTime * (hardPercentage / 100);
                        break;
                }
                perfectBufferTime = bufferTime * .2f;
            }
            else
            {
                onMusicBeatDelegate?.Invoke();
            }

            timerInBetweenBeat = 0; //Reinitialize timer on beat
        }
        else if( in_type == AkCallbackType.AK_MusicSyncUserCue)
        {
            EndOfMusic?.Invoke();
        }
    }


    IEnumerator beforeStart()
    {
        if (!inMenu)
        {
            var beat = Timeline.Instance.beatToReach;

            for (int i = 0; i < beat + 1; i++)
            {
                InstantiateBeat?.Invoke();
                yield return new WaitForSeconds(beatDuration);
            }
            gameMusicEvent.Post(gameObject, (uint)0x2100, CallbackFunction);
        }else
        {
            menuMusicEvent.Post(gameObject, (uint)0x2100, CallbackFunction);

        }

    }

    public void StopAllMusic()
    {
        stopAllMusicEvent.Post(gameObject);
    }

}

public enum Timing
{
    NULL,
    BEFORE,
    AFTER,
    PERFECT,
    MISS,
}

public enum Level
{
    Easy,
    Medium,
    Hard,
}

public enum Track
{
    GTR,
    HARPE,
    SYNTHE,
}