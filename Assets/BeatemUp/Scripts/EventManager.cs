using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public AK.Wwise.RTPC playBackSpeedRTPC;

    public void SetPlaybackSpeed(int speed)
    {
        playBackSpeedRTPC.SetGlobalValue( speed);
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
