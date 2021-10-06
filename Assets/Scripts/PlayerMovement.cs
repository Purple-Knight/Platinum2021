using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;

public class PlayerMovement : MonoBehaviour
{
    public int playerID = 0;
    public SpriteRenderer sprite;
    private Player player;
    [SerializeField] float deadZoneController;
    [SerializeField] float bufferTime;
    [SerializeField] float raycastDistance;
    [SerializeField] Color playerColor;
    float mvtHorizontal;
    float jump;
    float inputTimer;
    float afterBeatTimer;
    bool buttonPressed;
    bool beatPassed;
    bool hasMoved;
    bool hasJumped;
    bool isOnFloor;
    bool wasInAir;
    bool buttonDown;
    bool canGoRight;
    bool canGoLeft;
    bool canjump;

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;
        sprite.color = playerColor;
    }

    private void Update()
    {
        GetInput();
        Gravity();
        WallCollision();
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
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || Input.GetAxis("Horizontal") > deadZoneController;

        if (inputHorizontal && !beatPassed && !buttonDown)
        {
            buttonPressed = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");

        }
        else if (inputHorizontal && beatPassed && afterBeatTimer < bufferTime && !hasMoved && !buttonDown)
        {
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            Move();
            afterBeatTimer = 0;
        }
        else if (buttonDown && player.GetAxis("Move Horizontal") > -deadZoneController && Input.GetAxis("Horizontal") < deadZoneController)
        {
            mvtHorizontal = 0;
            buttonDown = false;
        }

        //jump
        if (player.GetButtonDown("Jump") && !beatPassed)
        {
            buttonPressed = true;
            jump = 1;

        }
        else if (player.GetButtonDown("Jump") && beatPassed && afterBeatTimer < bufferTime && !hasMoved)
        {
            jump = 1;
            mvtHorizontal = 0;
            Move();
            afterBeatTimer = 0;
        }
        else
        {
            jump = 0;
        }


    }

    public void Move()
    {
        if (!isOnFloor && (mvtHorizontal == 0 || wasInAir || !hasJumped)) // if for gravity 
        {
            transform.DOMoveY(transform.position.y - 1, .2f);
            hasMoved = true;
            wasInAir = true;
        }
        else if (!isOnFloor && inputTimer < bufferTime && mvtHorizontal != 0 && !wasInAir) // move after jump
        {
            int i = 0;
            if (mvtHorizontal > 0 && canGoRight)
            {
                i = 1;
            }else if(mvtHorizontal < 0 && canGoLeft)
            {
                i = -1;
            }
            transform.DOMoveX(transform.position.x + i, .2f);
            buttonPressed = false;
            hasMoved = true;
            wasInAir = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && mvtHorizontal !=0) //move horizontal on floor
        {
            int i = 0;
            if (mvtHorizontal > 0 && canGoRight)
            {
                i = 1;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                i = -1;
            }
            transform.DOMoveX(transform.position.x + i, .2f);
            buttonPressed = false;
            hasMoved = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && jump > 0 )// jump
        {
            if (canjump)
            {
                transform.DOMoveY(transform.position.y + jump, .2f);
            }
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
        RaycastHit2D rayrayFall = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));

        if( rayrayFall.collider != null)
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

    public void WallCollision()
    {
        RaycastHit2D rayrayRight = Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Ground"));
        if (rayrayRight.collider != null)
        {
            canGoRight = false;
        }
        else
        {
            canGoRight = true;
        }
        RaycastHit2D rayrayLeft = Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Ground"));
        if (rayrayLeft.collider != null)
        {
            canGoLeft = false;
        }
        else
        {
            canGoLeft = true;
        }
        RaycastHit2D rayrayJump = Physics2D.Raycast(transform.position, Vector2.up, 1, LayerMask.GetMask("Ground"));
        if (rayrayJump.collider != null)
        {
            canjump = false;
        }
        else
        {
            canjump = true;
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
            //Debug.Log("<color=green> Good </color>");
            seqColor.Append(sprite.DOColor(Color.green, .1f));
        }
        else if (inputTimer < bufferTime / 3 * 2 || afterBeatTimer < bufferTime / 3 * 2)
        {
            //Debug.Log("<color=yellow> Okay </color>");
            seqColor.Append(sprite.DOColor(Color.yellow, .1f));
        }
        else if (inputTimer < bufferTime  || afterBeatTimer < bufferTime )
        {
            //Debug.Log("<color=orange> Early / Late</color>");
            seqColor.Append(sprite.DOColor(new Color(1, .5f ,0), .1f));
        }
        
        seqColor.Append(sprite.DOColor(playerColor, .1f));
    }

    #endregion

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x +1, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x -1, transform.position.y));
    }*/
}
