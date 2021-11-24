using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public AK.Wwise.RTPC playBackSpeedRTPC;

    public void SetPlaybackSpeed(int speed)
    {
        bool up;
        if (playBackSpeedRTPC.GetGlobalValue() > speed) up = true;
        else up = false;

        playBackSpeedRTPC.SetGlobalValue(speed);
        Timeline.Instance.up = up;
        Timeline.Instance.actu = true;
    }
    
    public void PlaybackSpeedFast()
    {
        playBackSpeedRTPC.SetGlobalValue( 100);
    }
    
    public void PlaybackSpeedSlow(int speed)
    {
        playBackSpeedRTPC.SetGlobalValue(0);
    }
    
    public void PlaybackSpeedOriginal(int speed)
    {
        playBackSpeedRTPC.SetGlobalValue(50);
    }
}
