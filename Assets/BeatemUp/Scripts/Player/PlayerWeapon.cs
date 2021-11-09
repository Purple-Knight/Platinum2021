using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;
    private PlayerManager playerManager;

    float beatPassedTimer = 0;
    Timing playerTiming;
    RhythmManager rhythmManager;
    // bools
    private bool gotInput = false;  // Fire Input received this beat
    private bool triggerDown = false; // Holding Fire Button
    private bool beatPassed = false;

    [Header("---New Weapon Hierarchy---")]
    public Weapon weapon;

    //Debug
    [SerializeField] private bool debug = false;
    [SerializeField] private bool debugGUI = false;
    [SerializeField] private Rect guiDebugArea = new Rect(0, 20, 150, 150);
    //private string tempCharges;

    [SerializeField] Transform aiming;

    private void Start()
    {
        aiming = transform.Find("Isometric Diamond");
        UpdateAimVisual(Vector2.right);

        rhythmManager = RhythmManager.Instance;
        rhythmManager.onMusicBeatDelegate += BeatReceived;

        playerManager = GetComponent<PlayerManager>();
        player = ReInput.players.GetPlayer(playerManager.playerMovement.playerID);

        //SwapWeaponStyle("0"); // Initialize Players w/ base Weapon
    }

    private void Update()
    {
        if (weapon != null)
            weapon.GetInput();
        
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;

            if(beatPassedTimer > rhythmManager.halfBeatTime)
            {
                beatPassed = false;
                gotInput = false;
            }
        }

        
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (weapon != null)
                weapon.Upgarde();
            else
                SwapWeaponStyle("0");
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (weapon != null)
                weapon.Downgrade();
        }
    }

    public void SwapWeaponStyle(string key)
    {
        Weapon swap = null;
        Vector2 direction = Vector2.right;

        if (weapon != null)
        {
            swap = WeaponLibrary.Instance.GetFromLibrary(key, weapon);
            direction = weapon.GetDirection;
        }
        else
            swap = WeaponLibrary.Instance.GetFromLibrary(key);

        if(swap != null)
        {
            swap.Init(player, playerManager, this, direction);
            weapon = swap;
        }
        else
        {
            Debug.Log("<color=red>No Weapon output received !</color>"); // Invalid Key sent / output 'null' received
        }

        playerManager.comboManager.CurrentWeaponRef(weapon); // Set weaponRef in ComboCounter
    }

    public void UpdateAimVisual(Vector2 lastDirection)
    {
        aiming.position = new Vector2(transform.position.x + (lastDirection.x * .7f), transform.position.y + (lastDirection.y * .7f));
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
        /*GUILayout.BeginHorizontal();
        GUILayout.TextField("Horizontal Aim: " + player.GetAxis("Aim Horizontal"));
        GUILayout.TextField("Vertical Aim : " + player.GetAxis("Aim Vertical"));
        GUILayout.EndHorizontal();*/

        //GUILayout.TextArea("PastBeat Timer : " + beatPassedTimer);

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }
}
