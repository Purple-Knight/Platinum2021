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

    public float numberOfBeat;
    [SerializeField] private float timeBeforeStart;

    public float beatDuration;
    public List<Song> duration = new List<Song>();

    public UnityEvent InstantiateBeat;


    [Header("Beat Window")]
    [SerializeField] private float perfectBufferTime;
    [SerializeField] private float bufferTime;
    [SerializeField] private float halfBeatTime;

    [SerializeField] private float timerInBetweenBeat = 0;


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
        eventMusic[idToLaunch].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }



    private void Update()
    {
        // musicSpeedRTPC.SetGlobalValue(musicSpeed);

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            eventMusic[0].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            eventMusic[1].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            eventMusic[2].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }

        timerInBetweenBeat += Time.deltaTime;
    }

    public Timming AmIOnBeat()
    {
        if (timerInBetweenBeat >= (beatDuration - perfectBufferTime))
        {
            //Perfect before
            return Timming.PERFECT;
        }
        else if (timerInBetweenBeat >= (bufferTime * 2))
        {
            //Before
            return Timming.BEFORE;
        }
        else if (timerInBetweenBeat >= bufferTime && timerInBetweenBeat <= (bufferTime * 2))
        {
            //Miss
            return Timming.MISS;
        }
        else if (timerInBetweenBeat <= bufferTime)
        {
            //After
            return Timming.AFTER;
        }
        else if (timerInBetweenBeat <= perfectBufferTime)
        {
            //Perfect After
            return Timming.PERFECT;
        }
        
        return Timming.NULL;
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
        beatDuration = info.segmentInfo_fBeatDuration;

        if (!onceAtStart)
        {
            onceAtStart = true;
            eventMusic[1].Post(gameObject);
            StartCoroutine(beforeStart());
            numberOfBeat = duration[idToLaunch].duration / beatDuration;    //    stopper les x derniers beat en fct dde la time line ( check le nombre de beat dans la chanson et la time line)
            InstantiateBeat?.Invoke();

            //Window Rythm
            bufferTime = beatDuration / 3;
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
        var beat = Timeline.Instance.beatToReach;

        for (int i = 0; i < beat + 1; i++)
        {
            Timeline.Instance.SendBar();
            yield return new WaitForSeconds(beatDuration);
        }
        eventMusic[idToLaunch].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);

    }

}

public enum Timming
{
    NULL,
    BEFORE,
    AFTER,
    PERFECT,
    MISS,
}
