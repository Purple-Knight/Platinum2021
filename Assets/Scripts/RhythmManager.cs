using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance { get { return _instance; } }
    private static RhythmManager _instance;

    public delegate void OnMusicBeat();
    public OnMusicBeat onMusicBeatDelegate;


    [Header("Music Selection")]
    public List<AK.Wwise.Event> eventMusic = new List<AK.Wwise.Event>();


    [Header("Beat")]
    int position;
    bool onceAtStart;

    public float beatDuration;
    [SerializeField] private float numberOfBeat;
    [SerializeField] private float timeBeforeStart;
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
        eventMusic[1].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        position = 1;
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
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        onMusicBeatDelegate?.Invoke();

        if (!onceAtStart)
        {
            onceAtStart = true;
            AkMusicSyncCallbackInfo info = (AkMusicSyncCallbackInfo)in_info;
            beatDuration = info.segmentInfo_fBeatDuration;
            eventMusic[3].Post(gameObject);
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
        eventMusic[1].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }
}
