using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;

public class PlayerMovement : MonoBehaviour
{
    public int playerID = 0;
    private Player player;
    float mvtHorizontal;
    int jump;
    float inputTimer;
    float afterBeatTimer;
    [SerializeField] float bufferTime;
    bool buttonPressed;
    bool beatPassed;

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;
    }

    private void Update()
    {
        GetInput();
        if (buttonPressed)
        {
            inputTimer += Time.deltaTime;
        }
        if(beatPassed)
        {
            afterBeatTimer += Time.deltaTime;
        }if(afterBeatTimer > bufferTime)
        {
            beatPassed = false;
            afterBeatTimer = 0;
        }
    }

    public void GetInput()
    {

        
        if(player.GetAxis("Move Horizontal") !=0 && !beatPassed)
        {
            buttonPressed = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");

        }
        else if(player.GetAxis("Move Horizontal") != 0 && beatPassed && afterBeatTimer < bufferTime)
        {
            mvtHorizontal = player.GetAxis("Move Horizontal");
            Move();
            beatPassed = false;
            afterBeatTimer = 0;
        }
        else if(Input.GetAxis("Horizontal") > -.1f && Input.GetAxis("Horizontal") < .1f)
        {
            mvtHorizontal = 0;
        }
        //jump = Input.GetAxis("Vertical") > 0?  1 : 0;
        

    }

    public void Move()
    {
        if (inputTimer < bufferTime && mvtHorizontal !=0)
        {
            int i = mvtHorizontal > 0 ? 1 : -1;
            transform.DOMoveX(transform.position.x + i, .2f);
            buttonPressed = false;
        }else
        {
            beatPassed = true;
        }
        inputTimer = 0;
    }

    public void BeatReceived()
    {
        Debug.Log("beat received");
        Move();
    }
}
