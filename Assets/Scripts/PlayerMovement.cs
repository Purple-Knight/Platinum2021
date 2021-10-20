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
    [SerializeField] Color playerColor;
    [SerializeField] float deadZoneController;
    [SerializeField] internal float bufferTime;//---private -> internal---//
    float raycastDistance = .5f;
    float mvtHorizontal;
    float jump;
    float inputTimer;
    bool gotInput; //bool to start timer on input
    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    bool hasJumped; //player jumped
    bool wasInAir; //player was in the air the last beat
    bool buttonDown; //check if buttons stays down

    //serounding checks
    bool isOnFloor; // check if player is grounded
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
        Gravity();
        WallCollision();
        GetInput();
        if (gotInput || beatPassed)
        {
            inputTimer += Time.deltaTime;
        }
        if(inputTimer > bufferTime && beatPassed)
        {
            beatPassed = false;
            inputTimer = 0;
            hasMoved = false;
        }
    }

    public void GetInput()
    {
        //move horizontal 
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;

        if (inputHorizontal && !buttonDown && !hasMoved)
        {
            gotInput = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            if (beatPassed)
            {
                Move();
            }

        }
        else if (buttonDown && player.GetAxis("Move Horizontal") > -deadZoneController && player.GetAxis("Move Horizontal") < deadZoneController)
        {
            mvtHorizontal = 0;
            buttonDown = false;
        }

        //jump
        if (player.GetButton("Jump")  && !hasMoved) //appuier sur saut
        {
            gotInput = true;
            mvtHorizontal = 0;
            jump = 1;
            if (beatPassed) // apres le premier beat
            {
                Move();
            }

        }
        else //pas de input saut
        {
            jump = 0;
        }


    }

    public void Move()
    {
        int x = 0;
        int y = 0;
        if (!isOnFloor && (mvtHorizontal == 0 || wasInAir || !hasJumped)) // if for gravity 
        {
            y = -1;
            hasMoved = true;
            wasInAir = true;
        }
        else if (!isOnFloor && inputTimer < bufferTime && mvtHorizontal != 0 && !wasInAir) // move after jump
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                x = 1;
            }else if(mvtHorizontal < 0 && canGoLeft)
            {
                x = -1;
            }
            mvtHorizontal = 0;
            hasMoved = true;
            wasInAir = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && mvtHorizontal !=0) //move horizontal on floor
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                x = 1;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                x = -1;
            }
            mvtHorizontal = 0;
            hasMoved = true;
            HitResult();
        }
        else if (isOnFloor && inputTimer < bufferTime && jump > 0 )// jump
        {
            if (canjump)
            {
                y = 1;
            }
            jump = 0;
            hasMoved = true;
            hasJumped = true;
            HitResult();
        }
        transform.DOMove(new Vector2(transform.position.x + x, transform.position.y + y), .2f);
        beatPassed = true;
        gotInput = false;
        inputTimer = 0;
    }

    public void BeatReceived()
    {
        Move();
        Squeeeesh();
    }

    #region GravityAndCollisions
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
    #endregion

    #region feedback
    public void Squeeeesh()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(sprite.transform.DOScaleY(.8f, .1f)).Append(sprite.transform.DOScaleY(1, 0.1f)) ;
        seq.Play();
    }

    public void HitResult()
    {
        Sequence seqColor = DOTween.Sequence();

        if (inputTimer < bufferTime / 3 )
        {
            //Debug.Log("<color=green> Good </color>");
            seqColor.Append(sprite.DOColor(Color.green, .1f));
        }
        else if (inputTimer < bufferTime / 3 * 2 )
        {
            //Debug.Log("<color=yellow> Okay </color>");
            seqColor.Append(sprite.DOColor(Color.yellow, .1f));
        }
        else if (inputTimer < bufferTime   )
        {
            //Debug.Log("<color=orange> Early / Late</color>");
            seqColor.Append(sprite.DOColor(new Color(1, .5f ,0), .1f));
        }
        
        seqColor.Append(sprite.DOColor(playerColor, .1f));
    }

    #endregion

}
