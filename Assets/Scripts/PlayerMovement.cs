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
    bool hasMoved;
    public SpriteRenderer sprite;

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
        }
        if(afterBeatTimer > bufferTime)
        {
            beatPassed = false;
            afterBeatTimer = 0;
            hasMoved = false;
        }
    }

    public void GetInput()
    {

        
        if(player.GetAxis("Move Horizontal") !=0 && !beatPassed)
        {
            buttonPressed = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");

        }
        else if(player.GetAxis("Move Horizontal") != 0 && beatPassed && afterBeatTimer < bufferTime && !hasMoved)
        {
            mvtHorizontal = player.GetAxis("Move Horizontal");
            Move();
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
            hasMoved = true;
            HitResult();
        }/*
        else if(!hasMoved)
        {
            beatPassed = true;
        }*/
        beatPassed = true;
        inputTimer = 0;
    }

    public void BeatReceived()
    {
        Move();
        Squeeeesh();
    }

    public void Squeeeesh()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(.8f, .1f)).Append(transform.DOScaleY(1, .1f));
        seq.Play();
    }

    public void HitResult()
    {
        Sequence seqColor = DOTween.Sequence();

        if (inputTimer < bufferTime / 3 || afterBeatTimer < bufferTime / 3)
        {
            Debug.Log("<color=green> Good </color>");
            seqColor.Append(sprite.DOColor(Color.green, .1f));
        }
        else if (inputTimer < bufferTime / 3 * 2 || afterBeatTimer < bufferTime / 3 * 2)
        {
            Debug.Log("<color=yellow> Okay </color>");
            seqColor.Append(sprite.DOColor(Color.yellow, .1f));
        }
        else
        {
            Debug.Log("<color=orange> Early / Late</color>");
            seqColor.Append(sprite.DOColor(new Color(1, .5f ,0), .1f));
        }
        
        seqColor.Append(sprite.DOColor(Color.white, .1f));
    }
}
