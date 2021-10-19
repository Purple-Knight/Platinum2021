using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get { return _instance; } }
    private static RhythmManager _instance;
    //public AK.Wwise.RTPC musicSpeedRTPC;
    //public float musicSpeed = 0f;

    public delegate void OnMusicBeat();

    public OnMusicBeat onMusicBeatDelegate;

    [Header("Music Selection")]
    public AK.Wwise.Event eventMusic1;
    public AK.Wwise.Event eventMusic2;
    public AK.Wwise.Event eventMusic3;

    public AK.Wwise.Event eventStop;

    [Header("Beat")]
    int position;

    bool onceAtStart;

    public float beatDuration;
    public float numberOfBeat;
    public float timeBeforeStart;
    public List<Song> duration = new List<Song>();


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
        eventMusic2.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        position = 1;
    }

    private void Update()
    {
        // musicSpeedRTPC.SetGlobalValue(musicSpeed);

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            eventMusic1.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            eventMusic2.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            eventMusic3.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        onMusicBeatDelegate?.Invoke();

        if (!onceAtStart)
        {
            onceAtStart = true;
            AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
            beatDuration = info.segmentInfo_fBeatDuration;
            eventStop.Post(gameObject);
            StartCoroutine(beforeStart());

            numberOfBeat = duration[position].duration / beatDuration;    //    stopper les x derniers beat en fct dde la time line ( check le nombre de beat dans la chanson et la time line)
        }
        else
        {
            AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
            beatDuration = info.segmentInfo_fBeatDuration;

            /*AkDurationCallbackInfo durInfo = (AkDurationCallbackInfo)in_info;
            Debug.Log(durInfo.fDuration.ToString());*/

        }

    }

    IEnumerator beforeStart()
    {
        var beat = Timeline.Instance.beatToReach;

        for (int i = 0; i < beat; i++)
        {
            Timeline.Instance.SendBar();
            yield return new WaitForSeconds(beatDuration);
        }
        eventMusic2.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }
}


[System.Serializable]
public class Song
{
    public string songName;
    public float duration;
}
