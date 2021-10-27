using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon;
    private Player player;

    float inputTimer = 0;
    float beatPassedTimer = 0;
    internal PlayerMovement pMov;
    Vector2 lastDirection = Vector2.right;

    // bools
    private bool gotInput = false;  // Fire Input received this beat
    private bool triggerDown = false; // Holding Fire Button
    private bool beatPassed = false;

    //Debug
    [SerializeField] private bool debug = false;
    [SerializeField] private Weapon testWeapon;
    [SerializeField] private bool debugGUI = false;
    [SerializeField] private Rect guiDebugArea = new Rect(0, 20, 150, 150);
    private string tempCharges;

    [SerializeField] Transform aiming;

    private void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;

        pMov = GetComponent<PlayerMovement>();
        player = ReInput.players.GetPlayer(pMov.playerID);
        aiming.position = new Vector2(.7f, 0);

        Pickup(testWeapon);
        tempCharges = "" + weapon.chargeBeats;
    }

    private void Update()
    {
        GetInput();
        if(gotInput && !beatPassed)
        {
            inputTimer += Time.deltaTime;
        }
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;

            if(beatPassedTimer > pMov.bufferTime)   // after Post-Beat Window
            {
                if (gotInput && inputTimer > pMov.bufferTime)
                    triggerDown = false;

                if (!gotInput && triggerDown)   // Holding down (Charging weapon)
                    weapon.Use(triggerDown);

                inputTimer = 0;
                beatPassed = false;
                gotInput = false;
            }
        }

        if (player.GetAxisRaw("Aim Horizontal") != 0 || player.GetAxisRaw("Aim Vertical") != 0) // Last Aim Direction
        {
            float x = player.GetAxis("Aim Horizontal");
            float y = player.GetAxis("Aim Vertical");

            if (Mathf.Abs(x) >= Mathf.Abs(y))
                y = 0;   
            else
                x = 0;

            lastDirection.x = (x == 0) ? x : Mathf.Sign(x);
            lastDirection.y = (y == 0) ? y : Mathf.Sign(y);

            aiming.position = new Vector2(transform.position.x + (lastDirection.x * .7f), transform.position.y + (lastDirection.y * .7f)) ;

            if (weapon != null) weapon.lastDirection = lastDirection;
        }
    }

    private void GetInput()
    {
        /*
        if(!triggerDown && player.GetButtonDown("Fire") && !gotInput) // Button Down
        {
            triggerDown = true;
            gotInput = true;

            if (beatPassed) // Missed exact Beat
                FireWeapon();
        }
        else if(!triggerDown && player.GetButtonDown("Fire") && beatPassed && !gotInput)
        {
            gotInput = true;
            triggerDown = true;
            FireWeapon();
        }*/

        /*else if(triggerDown && player.GetButtonUp("Fire") && !gotInput) // Button Up
        {
            gotInput = true;
            triggerDown = false;

            if (beatPassed) // Missed exact Beat
                FireWeapon();
        }*/

        if (player.GetButtonDown("Fire"))
        {
            triggerDown = true;

            if (!gotInput)
            {
                gotInput = true;

                if (beatPassed)
                    FireWeapon();
            }
        }
        else if(player.GetButtonUp("Fire"))
        {
            triggerDown = false;

            if (!gotInput)
            {
                gotInput = true;

                if (beatPassed)
                    FireWeapon();
            }
            else weapon.MissedBeat(); // Reset Charges (input Down & Up in one beat)
        }
    }

    private void FireWeapon()
    {
        if (weapon == null) return;

        if(gotInput) // Input Up && Down
        {
            if ((inputTimer <= pMov.bufferTime && inputTimer != 0) || (beatPassedTimer <= pMov.bufferTime && beatPassed))
            {
                weapon.Use(triggerDown);
            }
            else if (inputTimer > pMov.bufferTime)
            {
                weapon.MissedBeat();
                //Debug.Log(inputTimer + "; " + pMov.bufferTime);
            }
        }

        beatPassedTimer = 0;
    }

    public void Pickup(Weapon pick)
    {
        weapon = Instantiate(pick); //Instance of ScriptableObject
        weapon.pMov = pMov;
    }

    public void BeatReceived()
    {
        FireWeapon();
        beatPassed = true;
    }


    //----------------
    public void OnGUI()
    {
        if (!debugGUI) return;

        GUILayout.BeginArea(guiDebugArea);
        GUILayout.TextArea("Input : " + gotInput);
        //GUILayout.TextField("Last Direction : " + lastDirection);
        /*GUILayout.BeginHorizontal();
        GUILayout.TextField("Horizontal Aim: " + player.GetAxis("Aim Horizontal"));
        GUILayout.TextField("Vertical Aim : " + player.GetAxis("Aim Vertical"));
        GUILayout.EndHorizontal();*/

        //GUILayout.TextArea("Input Timer : " + inputTimer);
        //GUILayout.TextArea("PastBeat Timer : " + beatPassedTimer);

        GUILayout.BeginHorizontal();
        GUILayout.TextArea("Num of charge Beat (int) : ");
        tempCharges = GUILayout.TextField(tempCharges);
        weapon.chargeBeats = int.Parse(tempCharges);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Replace Weapon"))
            Pickup(testWeapon);

        GUILayout.EndArea();
    }
}
