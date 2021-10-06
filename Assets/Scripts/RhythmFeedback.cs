using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RhythmFeedback : MonoBehaviour
{
    private void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += DoStuff;
    }

    public void DoStuff()
    {
        //transform.d
    }
}
