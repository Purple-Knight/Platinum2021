using UnityEngine;
using UnityEditor;

public class MusicVisualiser : EditorWindow
{
    RhythmManager rhythmManager;
    
    public AK.Wwise.Event playMusic;
    public AkWwiseProjectData wwiseData;
    System.Collections.Generic.List<AkAmbient> list  = new System.Collections.Generic.List<AkAmbient>();
    int index = 0;
    string[] options = new string[] { "Crypt", "Lady", "Tutel" };
    bool DoOnce = false;
    private static readonly System.Collections.Generic.List<AkEvent> akEvents = new System.Collections.Generic.List<AkEvent>();
    [MenuItem("Window/Music Visualiser")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MusicVisualiser window = (MusicVisualiser)EditorWindow.GetWindow(typeof(MusicVisualiser));
        window.Show();
        
    }

    private void OnGUI()
    {
        GUILayout.Label("This is the music visualiser... I don't know what I'm doing (:   ");
        
        if (rhythmManager == null) {
            rhythmManager = FindObjectOfType<RhythmManager>();
            //playMusic.Post(rhythmManager.gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, InstantiatBeat);
            uint i;
            AkSoundEngine.LoadBank("MusicBank", out i);
            AkSoundEngine.SetOfflineRendering(true);
            
        }

        //index = EditorGUILayout.Popup(index, options);

        /*if (GUILayout.Button("Play music"))
        {
            playMusic = akEvents[index].;
            playMusic.playingId Post(FindObjectOfType<AkGameObj>().gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, InstantiatBeat);
        }*/

        if(GUILayout.Button("Play Music"))
        {

            rhythmManager.eventMusic[0].Post( rhythmManager.gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, InstantiatBeat);
            Debug.Log("Time Stamp : " + AkSoundEngine.GetTimeStamp());
        }
    }

    public void InstantiatBeat()
    {
        Debug.Log("I think it worked O.O");
    }
}
