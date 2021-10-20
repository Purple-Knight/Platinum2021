using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon;
    public Player player;

    float inputTimer = 0;
    internal PlayerMovement pMov;
    // bools
    private bool gotInput = false;
    private bool beatPassed = false;

    [Header("TESING")]
    public Weapon testWeapon;

    private void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;

        pMov = GetComponent<PlayerMovement>();
        player = ReInput.players.GetPlayer(pMov.playerID);

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
        if(gotInput && weapon != null)
        {
            if (inputTimer <= pMov.bufferTime)
            {
                weapon.Use();
            }
            else
            {
                weapon.MissedBeat();
                Debug.Log(inputTimer + "; " + pMov.bufferTime);
            }
        }

        inputTimer = 0;
        gotInput = false;
    }

    public void Pickup(Weapon pick)
    {
        weapon = Instantiate(pick); //Instance of ScriptableObject
        weapon.pMov = pMov;
    }

    public void BeatReceived()
    {
        FireWeapon();
    }
}
