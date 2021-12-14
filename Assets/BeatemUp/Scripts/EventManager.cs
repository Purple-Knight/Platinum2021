using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventManager : MonoBehaviour
{
    public AK.Wwise.RTPC playBackSpeedRTPC;
    float timer = 0;
    bool speedUp = false;
    bool slowDown = false;
    bool resetSpeed = false;
    int previousValue;
    [SerializeField] float speed;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
    }

    private void Update()
    {
        timer += Time.deltaTime * speed /10;
        if (speedUp)
        {
            playBackSpeedRTPC.SetGlobalValue(Mathf.Lerp(50, 100, timer));
            if(playBackSpeedRTPC.GetGlobalValue() >= 99)
            {
                playBackSpeedRTPC.SetGlobalValue(100);
                speedUp = false;
                previousValue = 100;
            }
        }else if(slowDown)
        {
            playBackSpeedRTPC.SetGlobalValue(Mathf.Lerp(50, 0, timer));
            if (playBackSpeedRTPC.GetGlobalValue() <= 1)
            {
                playBackSpeedRTPC.SetGlobalValue(0);
                slowDown = false;
                previousValue = 0;
            }
        }
        else if (resetSpeed)
        {
            playBackSpeedRTPC.SetGlobalValue(Mathf.Lerp(previousValue, 50, timer));
            if (playBackSpeedRTPC.GetGlobalValue() ==50)
            {
                resetSpeed = false;
            }
        }
    }


    public void StartEvent()
    {
        if(RhythmManager.Instance.level == Level.Medium)
        {
            speedUp = true;
            text.text = "Event : Speed Up!";
        }
        else
        {
            slowDown = true;
            text.text = "Event : Slow Down...";

        }
    }

    public void EndEvent()
    {
        resetSpeed = true;
    }

    public void SetPlaybackSpeed(int speed)
    {
        playBackSpeedRTPC.SetGlobalValue( speed);
    }
    
    public void PlaybackSpeedFast()
    {
        playBackSpeedRTPC.SetGlobalValue( 100);
    }
    
    public void PlaybackSpeedSlow()
    {
        playBackSpeedRTPC.SetGlobalValue(0);
    }
    
    public void PlaybackSpeedOriginal()
    {
        playBackSpeedRTPC.SetGlobalValue(50);
    }
}
