using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;

    float beatPassedTimer = 0;
    internal PlayerMovement pMov;
    Timing playerTiming;
    RhythmManager rhythmManager;
    // bools
    private bool gotInput = false;  // Fire Input received this beat
    private bool triggerDown = false; // Holding Fire Button
    private bool beatPassed = false;

    [Header("---New Weapon Hierarchy---")]
    public Weapon weapon;
    public Weapon newWeaponTarget;

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

        pMov = GetComponent<PlayerMovement>();
        player = ReInput.players.GetPlayer(pMov.playerID);

        Debug.Log(player.id);
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

        if(player.id == 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
                SwapWeaponStyle("00");

            if (Input.GetKeyDown(KeyCode.R))
                SwapWeaponStyle("01");
        } else if(player.id == 1)
        {
            if (Input.GetKeyDown(KeyCode.T))
                SwapWeaponStyle("00");

            if (Input.GetKeyDown(KeyCode.Y))
                SwapWeaponStyle("01");
        }else if(player.id == 2)
        {
            if (Input.GetKeyDown(KeyCode.U))
                SwapWeaponStyle("00");

            if (Input.GetKeyDown(KeyCode.I))
                SwapWeaponStyle("01");
        }
    }

    public void SwapWeaponStyle(string key)
    {
        if(weapon != null)
            weapon = WeaponLibrary.Instance.GetFromLibrary(key, weapon);
        else
            weapon = WeaponLibrary.Instance.GetFromLibrary(key);
        weapon.Init(player, this);
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
        if(weapon != null)
        {

        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Replace Weapon"))
        {
            //SwapWeaponStyle(newWeaponTarget);
        }

        GUILayout.EndArea();
    }
}
