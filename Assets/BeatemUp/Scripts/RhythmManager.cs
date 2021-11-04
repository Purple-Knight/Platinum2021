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
    public List<AK.Wwise.Event> eventMusic = new List<AK.Wwise.Event>();
    [SerializeField ]private int idToLaunch;


    [Header("Beat")]
    bool onceAtStart;
    public bool inMenu;

    public float numberOfBeat;
    [SerializeField] private float timeBeforeStart;

    public float beatDuration;
    public List<Song> duration = new List<Song>();

    public UnityEvent InstantiateBeat;


    [Header("Beat Window")]
    [SerializeField] private float perfectBufferTime;
    [SerializeField] public float bufferTime;
    [SerializeField] public float halfBeatTime;

    [SerializeField] private float timerInBetweenBeat = 0;

    [SerializeField] Level level;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }



    void Start()
    {
        StartCoroutine(delayStart());
    }

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(timeBeforeStart);
        eventMusic[idToLaunch].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBar, CallbackFunction);
    }



    private void Update()
    {
        // musicSpeedRTPC.SetGlobalValue(musicSpeed);

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            eventMusic[0].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
            onceAtStart = false;
            idToLaunch = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            eventMusic[1].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
            onceAtStart = false;
            idToLaunch = 1;

        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            eventMusic[2].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBar, CallbackFunction);
            onceAtStart = false;
            idToLaunch = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            eventMusic[3].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
            onceAtStart = false;
            idToLaunch = 3;
        }

        timerInBetweenBeat += Time.deltaTime;
    }

    public Timing AmIOnBeat()
    {
        if (timerInBetweenBeat >= (beatDuration - perfectBufferTime))
        {
            //Perfect before
            return Timing.PERFECT;
        }
        else if (timerInBetweenBeat >= (bufferTime * 2))
        {
            //Before
            return Timing.BEFORE;
        }
        else if (timerInBetweenBeat >= bufferTime && timerInBetweenBeat <= (bufferTime * 2))
        {
            //Miss
            return Timing.MISS;
        }
        else if (timerInBetweenBeat <= bufferTime)
        {
            //After
            return Timing.AFTER;
        }
        else if (timerInBetweenBeat <= perfectBufferTime)
        {
            //Perfect After
            return Timing.PERFECT;
        }
        
        return Timing.NULL;
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
        beatDuration = info.segmentInfo_fBeatDuration;
        Debug.Log(info.segmentInfo_iActiveDuration);

        if (!onceAtStart)
        {
            onceAtStart = true;
            eventMusic[1].Post(gameObject);
            StartCoroutine(beforeStart());
            numberOfBeat = duration[idToLaunch].duration / beatDuration;    //    stopper les x derniers beat en fct dde la time line ( check le nombre de beat dans la chanson et la time line)
            InstantiateBeat?.Invoke();

            //Window Rythm
            bufferTime = beatDuration / 3;
            switch (level)
            {
                case Level.Easy:
                    bufferTime += (bufferTime / 3) * 2;
                    break;
                case Level.Medium:
                    bufferTime += (bufferTime / 3);
                    break;
            }
            halfBeatTime = beatDuration / 2;
            perfectBufferTime = beatDuration / 6;
        }
        else
        {
            onMusicBeatDelegate?.Invoke();   
        }

        timerInBetweenBeat = 0; //Reinitialize timer on beat

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
        }

            eventMusic[idToLaunch].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

}

public enum Timing
{
    NULL,
    BEFORE,
    AFTER,
    PERFECT,
    MISS
}

public enum Level
{
    Easy,
    Medium,
    Hard

}