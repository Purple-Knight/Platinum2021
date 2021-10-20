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
    Vector2 lastDirection;

    // bools
    private bool gotInput = false;
    private bool beatPassed = false;

    [Header("DEBUG")]
    public Weapon testWeapon;
    [SerializeField] private Rect guiDebugArea = new Rect(0, 20, 150, 150);
    public bool debugGUI = false;

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

        if (player.GetAxisRaw("Aim Horizontal") != 0 || player.GetAxisRaw("Aim Vertical") != 0)
        {
            float x = player.GetAxis("Aim Horizontal");
            float y = player.GetAxis("Aim Vertical");

            if (Mathf.Abs(x) >= Mathf.Abs(y))
                y = 0;   
            else
                x = 0;

            lastDirection.x = (x == 0) ? x : Mathf.Sign(x);
            lastDirection.y = (y == 0) ? y : Mathf.Sign(y);

            if (weapon != null) weapon.lastDirection = lastDirection;
        }
    }

    private void GetInput()
    {
        if(player.GetButtonDown("Fire") && !gotInput)
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


    //----------------
    public void OnGUI()
    {
        if (!debugGUI) return;

        //GUILayout.BeginHorizontal();

        GUILayout.BeginArea(guiDebugArea);
        GUILayout.TextField("Last Direction : " + lastDirection);
        GUILayout.TextField("Horizontal Aim: " + player.GetAxis("Aim Horizontal"));
        GUILayout.TextField("Vertical Aim : " + player.GetAxis("Aim Vertical"));
        GUILayout.EndArea();
    }
}
