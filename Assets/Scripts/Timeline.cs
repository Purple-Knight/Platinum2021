using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    public GameObject endTimeline;
    public GameObject bar;

    bool canBegin;
    public float beatToReach;

    void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += SendBar;
    }

    void SendBar()
    {
        if (canBegin)
        {
            var lastBar = Instantiate(bar, transform.position, transform.rotation);

            var direction = endTimeline.transform.position - transform.position;
            var distance = Vector2.Distance(transform.position, endTimeline.transform.position);

            var time = beatToReach * RhythmManager.Instance.beatDuration;
            
            var speed = distance / time;

            var barScript = lastBar.GetComponent<BarTL>();
            barScript.direction = direction.normalized;
            barScript.speed = speed;
            barScript.deleteTime = time;
        }
        else
        {
            canBegin = true;
        }
    }
}
