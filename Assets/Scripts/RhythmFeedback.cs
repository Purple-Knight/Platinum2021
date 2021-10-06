using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmFeedback : MonoBehaviour
{
    private void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += DoStuff;
    }

    public void DoStuff()
    {

    }
}
