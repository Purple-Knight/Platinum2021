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
    float jump;
    float inputTimer;
    float afterBeatTimer;
    [SerializeField] float bufferTime;
    bool buttonPressed;
    bool beatPassed;
    bool hasMoved;
    [SerializeField]  bool hasJumped;
    public SpriteRenderer sprite;
    [SerializeField] bool isOnFloor;
    [SerializeField] bool wasInAir;
    [SerializeField] float raycastDistance;

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;
    }

    private void Update()
    {
        GetInput();
        Gravity();
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
        //move horizontal
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

        //jump
        if(player.GetAxis("Jump") > 0 && !beatPassed)
        {
            buttonPressed = true;
            jump = 1;

        }
        else if (player.GetAxis("Jump") > 0 && beatPassed && afterBeatTimer < bufferTime && !hasMoved)
        {
            jump = 1;
            mvtHorizontal = 0;
            Move();
            afterBeatTimer = 0;
        }
        else if (player.GetAxis("Jump") < .1f)
        {
            jump = 0;
        }

    }

    public void Move()
    {
        if (!isOnFloor && (mvtHorizontal == 0 || wasInAir || !hasJumped)) // if for gravity 
        {
            Debug.Log("gravity");
            transform.DOMoveY(transform.position.y - 1, .2f);
            hasMoved = true;
            wasInAir = true;
        }
        else if (!isOnFloor && inputTimer < bufferTime && mvtHorizontal != 0 && !wasInAir) // move after jump
        {
            int i = mvtHorizontal > 0 ? 1 : -1;
            transform.DOMoveX(transform.position.x + i, .2f);
            buttonPressed = false;
            hasMoved = true;
            wasInAir = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && mvtHorizontal !=0) //move horizontal on floor
        {
            int i = mvtHorizontal > 0 ? 1 : -1;
            transform.DOMoveX(transform.position.x + i, .2f);
            buttonPressed = false;
            hasMoved = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && jump > 0 )// jump
        {
            transform.DOMoveY(transform.position.y + jump, .2f);
            jump = 0;
            buttonPressed = false;
            hasMoved = true;
            hasJumped = true;
            HitResult();
        }
        /*
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

    //raycast O.O *u* hello there :)))) watcha ray casting on?
    public void Gravity()
    {
        RaycastHit2D rayray = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));

        if( rayray.collider != null)
        {
            isOnFloor = true;
            if(wasInAir)
                hasJumped = false;
            wasInAir = false;
        }
        else
        {
            isOnFloor = false;
        }
    }

    #region feedback
    public void Squeeeesh()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(.8f, .1f)).Append(transform.DOScaleY(1, 0.1f)) ;
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
        else if (inputTimer < bufferTime  || afterBeatTimer < bufferTime )
        {
            Debug.Log("<color=orange> Early / Late</color>");
            seqColor.Append(sprite.DOColor(new Color(1, .5f ,0), .1f));
        }
        
        seqColor.Append(sprite.DOColor(Color.white, .1f));
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance));
    }
}
