using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Null : Weapon
{
    public override void GetInput()
    {
        if (player.GetButtonDown("Fire") && !GotInput)
        {
            GotInput = true;
            playerTiming = RhythmManager.Instance.AmIOnBeat();

            if (playerTiming != Timing.MISS && playerTiming != Timing.NULL)
            {
                Fire();
            }
        }

        if (GotInput && RhythmManager.Instance.AmIOnBeat() == Timing.MISS)
        {
            GotInput = false;
        }
    }
}
