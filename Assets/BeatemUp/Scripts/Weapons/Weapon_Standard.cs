using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Standard : Weapon
{
    public override void GetInput()
    {
        if (player.GetButtonDown("Fire") && !gotInput)
        {
            gotInput = true;
            playerTiming = RhythmManager.Instance.AmIOnBeat();

            if (playerTiming != Timing.MISS && playerTiming != Timing.NULL)
            {
                Fire();
                playerManager.playerAnimator.SetTrigger("Fire");
            }
        }

        if (gotInput && RhythmManager.Instance.AmIOnBeat() == Timing.MISS)
        {
            gotInput = false;
        }
    }
}
