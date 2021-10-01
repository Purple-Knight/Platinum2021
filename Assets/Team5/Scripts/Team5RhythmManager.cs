using UnityEngine;

public class Team5RhythmManager : MonoBehaviour
{
    //public AK.Wwise.RTPC musicSpeedRTPC;
    //public float musicSpeed = 0f;

    public delegate void OnMusicBeat();

    public OnMusicBeat onMusicBeatDelegate;

    public AK.Wwise.Event eventMusic1;
    public AK.Wwise.Event eventMusic2;
    public AK.Wwise.Event eventMusic3;

    // Start is called before the first frame update
    void Start()
    {
        eventMusic1.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
    }

    private void Update()
    {
       // musicSpeedRTPC.SetGlobalValue(musicSpeed);

        if (Input.GetKeyDown(KeyCode.Keypad1)) {
            eventMusic1.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        } else if (Input.GetKeyDown(KeyCode.Keypad2)) {
            eventMusic2.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        } else if (Input.GetKeyDown(KeyCode.Keypad3)) {
            eventMusic3.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, CallbackFunction);
        }
    }

    void CallbackFunction(object in_cookie, AkCallbackType in_type, object in_info)
    {
        onMusicBeatDelegate?.Invoke();
    }

}
