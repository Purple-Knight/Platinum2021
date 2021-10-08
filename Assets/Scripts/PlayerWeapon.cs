using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon;
    public Player player;

    float inputTimer = 0;
    // bools
    private bool gotInput = false;
    private bool beatPassed = false;

    [Header("TESING")]
    public Weapon testWeapon;

    private void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;
        player = ReInput.players.GetPlayer(GetComponent<PlayerMovement>().playerID); // moche...

        Pickup(testWeapon);
    }

    private void Update()
    {
        GetInput();
        if(gotInput)
        {
            inputTimer += Time.deltaTime;
        }
    }

    private void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !gotInput)
        {
            gotInput = true;
        }
    }

    private void FireWeapon()
    {
        if(gotInput)
        {
            if (inputTimer <= .2f)
                weapon.Use();
            else
                weapon.MissedBeat();
        }

        inputTimer = 0;
        gotInput = false;
    }

    public void Pickup(Weapon pick)
    {
        weapon = new Weapon(pick);
    }

    public void BeatReceived()
    {
        FireWeapon();
    }
}
