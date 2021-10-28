using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon_old weapon;
    private Player player;

    float inputTimer = 0;
    float beatPassedTimer = 0;
    internal PlayerMovement pMov;
    Vector2 lastDirection = Vector2.right;
    Timing playerTiming;
    RhythmManager rhythmManager;
    // bools
    private bool gotInput = false;  // Fire Input received this beat
    private bool triggerDown = false; // Holding Fire Button
    private bool beatPassed = false;

    [Header("---New Weapon Hierarchy---")]
    public Weapon currentWeapon;

    //Debug
    [SerializeField] private bool debug = false;
    [SerializeField] private Weapon_old testWeapon;
    [SerializeField] private bool debugGUI = false;
    [SerializeField] private Rect guiDebugArea = new Rect(0, 20, 150, 150);
    private string tempCharges;

    [SerializeField] Transform aiming;

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onMusicBeatDelegate += BeatReceived;

        pMov = GetComponent<PlayerMovement>();
        player = ReInput.players.GetPlayer(pMov.playerID);
        aiming.position = new Vector2(transform.position.x + (lastDirection.x * .7f), transform.position.y + (lastDirection.y * .7f));
        Pickup(testWeapon);
        tempCharges = "" + weapon.chargeBeats;
        weapon.lastDirection = lastDirection;
    }

    private void Update()
    {
        //GetInput();
        currentWeapon.GetInput();
        
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;

            if(beatPassedTimer > rhythmManager.halfBeatTime)
            {
                beatPassed = false;
                gotInput = false;
            }
            /*if(beatPassedTimer > rhythmManager.bufferTime)   // after Post-Beat Window
            {
                playerTiming = rhythmManager.AmIOnBeat();
                if (gotInput && inputTimer > pMov.bufferTime)
                    triggerDown = false;

                if (!gotInput && triggerDown)   // Holding down (Charging weapon)
                    weapon.Use(triggerDown);

            }*/
        }
    }

    private void GetInput()
    {

        if (player.GetButtonDown("Fire") && !gotInput)
        {
            gotInput = true;
            triggerDown = true;
            playerTiming = rhythmManager.AmIOnBeat();

            if (playerTiming != Timing.MISS && playerTiming != Timing.NULL)
            {
                FireWeapon();
            }
        }
        /*else if(player.GetButtonUp("Fire") && !gotInput)
        {
            gotInput = true;
            triggerDown = false;
            playerTiming = rhythmManager.AmIOnBeat();

            if ( playerTiming != Timming.MISS && playerTiming != Timming.NULL)
            {
                FireWeapon();
            }
            else weapon.MissedBeat(); // Reset Charges (input Down & Up in one beat)
        }*/

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

            aiming.position = new Vector2(transform.position.x + (lastDirection.x * .7f), transform.position.y + (lastDirection.y * .7f));

            if (weapon != null) weapon.lastDirection = lastDirection;
        }
    }

    private void FireWeapon()
    {
        if (weapon == null) return;

        weapon.Use(triggerDown);

    }

    public void Pickup(Weapon_old pick)
    {
        weapon = Instantiate(pick); //Instance of ScriptableObject
        weapon.pMov = pMov.transform;
    }

    public void BeatReceived()
    {
        beatPassed = true;
        beatPassedTimer = 0;
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
