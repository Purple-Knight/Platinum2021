using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Timeline : MonoBehaviour
{
    public GameObject endTimeline;
    public GameObject bar;

    bool canBegin;
    public float beatToReach;

    public GameObject echo;


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

            createEcho();
        }
        else
        {
            canBegin = true;
        }
    }


    void createEcho()
    {
        var echoO = Instantiate(echo, transform.position, transform.rotation);
        var colora = gameObject.GetComponent<SpriteRenderer>();

        echoO.GetComponent<SpriteRenderer>().color = new Color(colora.color.r, colora.color.g, colora.color.b, 0);
        echoO.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        echoO.transform.localScale = transform.localScale * 2;

        echoO.GetComponent<SpriteRenderer>().DOFade(1, RhythmManager.Instance.beatDuration);
        echoO.transform.DOScale(transform.localScale, RhythmManager.Instance.beatDuration);

        Destroy(echoO, RhythmManager.Instance.beatDuration);
    }
}
