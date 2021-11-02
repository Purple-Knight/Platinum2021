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
        }
        else
        {
            onMusicBeatDelegate?.Invoke();   
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
        }

            eventMusic[idToLaunch].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

}
