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
    [SerializeField] float bufferTime;
    float raycastDistance = .5f;
    float mvtHorizontal;
    [SerializeField] float jump;
    float inputTimer;
    float beatPassedTimer;
    bool gotInput; //bool to start timer on input
    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    [SerializeField] bool hasJumped; //player jumped
    bool wasInAir; //player was in the air the last beat
    bool buttonDown; //check if buttons stays down
    [SerializeField] bool jumpButtonDown; //check if buttons stays down

    //serounding checks
    bool isOnFloor; // check if player is grounded
    bool canGoRight;
    bool canGoLeft;
    bool canGoDiagonalRight;
    bool canGoDiagonalLeft;
    bool canjump;

    //lerp
    Vector2 lastPos;
    Vector2 targetPos;

    private void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        RhythmManager.Instance.onMusicBeatDelegate += BeatReceived;
        sprite.color = playerColor;
        lastPos = transform.position;
        targetPos = transform.position;
    }

    private void Update()
    {
        Gravity();
        WallCollision();
        GetInput();
        if (gotInput)
        {
            inputTimer += Time.deltaTime;
        }
        if ( beatPassed)
        {
            beatPassedTimer += Time.deltaTime;
        }
        if(beatPassedTimer > bufferTime && beatPassed)
        {
            beatPassed = false;
            beatPassedTimer = 0;
            hasMoved = false;
            hasJumped = false;
            lastPos = targetPos;
        }
    }


    public void GetInput()
    {
        //move
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;

        if (inputHorizontal && !buttonDown && !hasMoved && !beatPassed)
        {
            gotInput = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
        }
        else if (inputHorizontal && !buttonDown && !hasMoved && beatPassed && beatPassedTimer < bufferTime)
        {
            gotInput = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            Move();
        }
        else if (buttonDown && player.GetAxis("Move Horizontal") > -deadZoneController && player.GetAxis("Move Horizontal") < deadZoneController)
        {
            mvtHorizontal = 0;
            buttonDown = false;
        }

        //jump
        bool inputVertical = player.GetAxis("Move Vertical") < -deadZoneController || player.GetAxis("Move Vertical") > deadZoneController;

        if (inputVertical && !jumpButtonDown && !hasJumped && !beatPassed) //appuier sur saut
        {
            jumpButtonDown = true;
            gotInput = true;
            jump = 1;
        }
        else if (inputVertical && !jumpButtonDown && !hasJumped && beatPassed && beatPassedTimer < bufferTime) // apres le premier beat
        {
            jumpButtonDown = true;
            gotInput = true;
            jump = 1;
            Move();
        }
        else if (jumpButtonDown && player.GetAxis("Move Vertical") > -deadZoneController && player.GetAxis("Move Vertical") < deadZoneController)
        {
            jump = 0;
            jumpButtonDown = false;
        }


    }

    public void Move()
    {
        if (!isOnFloor && (mvtHorizontal == 0 || wasInAir || !hasJumped) && !hasMoved) // if for gravity 
        {
            targetPos.y =  transform.position.y -1;
            hasMoved = true;
            wasInAir = true;

        }
        else if (!isOnFloor && inputTimer < bufferTime && mvtHorizontal != 0 && !wasInAir && !hasMoved) // move after jump
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                targetPos.x = transform.position.x + 1;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                targetPos.x = transform.position.x -1;

            }
            hasMoved = true;
            mvtHorizontal = 0;
            wasInAir = true;
            //HitResult();
        }
        else if(isOnFloor && inputTimer <bufferTime && mvtHorizontal != 0 && jump!=0 && !hasJumped && !hasMoved) //diagonal
        {
            if (mvtHorizontal > 0 && canGoDiagonalRight  && canjump)
            {
                targetPos.x = transform.position.x + 1;
            }
            else if (mvtHorizontal < 0 && canGoDiagonalLeft && canjump)
            {
                targetPos.x = transform.position.x - 1;
            }
            if (canjump)
            {
                targetPos.y = transform.position.y + 1;
            }
            jump = 0;
            mvtHorizontal = 0;
            hasMoved = true;
            hasJumped = true;
        }
        else if (isOnFloor && inputTimer < bufferTime && mvtHorizontal != 0 && !hasMoved) //move horizontal on floor
        {
            if (mvtHorizontal > 0 && canGoRight )
            {
               targetPos.x = transform.position.x + 1;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                targetPos.x = transform.position.x -1;
            }
            mvtHorizontal = 0;
            hasMoved = true;
           // HitResult();
        }else if (isOnFloor && inputTimer < bufferTime && jump > 0 && !hasJumped)// jump
        {
            if (canjump)
            {
                targetPos.y = transform.position.y + 1;
            }
            jump = 0;
            hasJumped = true;
        }

        HitResult();

        //diagonale with tweening
       /* if (DOTween.IsTweening(transform)) //check if currently tweening
        {
            DOTween.Complete(transform);
            transform.DOMove(new Vector2(transform.position.x + x, transform.position.y + y), .2f);
        }
        else
        {
            transform.DOMove(new Vector2(transform.position.x + x, transform.position.y + y), .2f);

        }*/
        
        gotInput = false;
    }

    public void BeatReceived()
    {
        beatPassed = true;
        Move();
        inputTimer = 0;
        StartCoroutine(LerpMove());
        Squeeeesh();
    }

    #region GravityAndCollisions
    //raycast O.O *u* hello there :)))) watcha ray casting on?
    public void Gravity()
    {
        RaycastHit2D rayrayFall = Physics2D.Raycast(transform.position, Vector2.down,1, LayerMask.GetMask("Ground"));

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
        RaycastHit2D rayray = Physics2D.Raycast(transform.position, Vector2.up, 1, LayerMask.GetMask("Ground"));
        if (rayray.collider != null)
        {
            canjump = false;
        }
        else
        {
            canjump = true;
        }
        rayray = Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Ground"));
        if (rayray.collider != null)
        {
            canGoRight = false;
        }
        else
        {
            canGoRight = true;
        }
        rayray = Physics2D.Raycast(transform.position, new Vector2(1, 1), 1, LayerMask.GetMask("Ground"));
        if (rayray.collider != null)
        {
            canGoDiagonalRight = false;
        }
        else
        {
            canGoDiagonalRight = true;
        }
        rayray = Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Ground"));
        if (rayray.collider != null)
        {
            canGoLeft = false;
        }
        else
        {
            canGoLeft = true;
        }
        
         
        rayray = Physics2D.Raycast(transform.position, new Vector2(-1,1) , 1, LayerMask.GetMask("Ground"));
        if(rayray.collider != null)
        {
            canGoDiagonalLeft = false;
        }
        else
        {
            canGoDiagonalLeft = true;
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
        if (inputTimer != 0)
        {
            Sequence seqColor = DOTween.Sequence();

            if (inputTimer < bufferTime / 3)
            {
                //Debug.Log("<color=green> Good </color>");
                seqColor.Append(sprite.DOColor(Color.green, .1f));
            }
            else if (inputTimer < bufferTime / 3 * 2)
            {
                //Debug.Log("<color=yellow> Okay </color>");
                seqColor.Append(sprite.DOColor(Color.yellow, .1f));
            }
            else if (inputTimer < bufferTime)
            {
                //Debug.Log("<color=orange> Early / Late</color>");
                seqColor.Append(sprite.DOColor(new Color(1, .5f, 0), .1f));
            }
            else if (inputTimer > bufferTime)
            {
                Debug.Log("<color=red> missed </color>");
                seqColor.Append(sprite.DOColor(Color.red, .2f));
            }

            seqColor.Append(sprite.DOColor(playerColor, .1f));
        }
    }

    #endregion

    IEnumerator LerpMove()
    {
        float t =0;
        while (t <= 1)
        {
            yield return new WaitForEndOfFrame();
            transform.position = Vector2.Lerp(lastPos, targetPos, t);
            t += 1/bufferTime * Time.deltaTime;
            if( t > 1)
            {
                t = 1;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = canjump ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.up));
        Gizmos.color = !isOnFloor ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.down));
        Gizmos.color = canGoLeft ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.left));
        Gizmos.color = canGoRight ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.right));
        Gizmos.color = canGoDiagonalRight ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, new Vector2(1, 1)));
        Gizmos.color = canGoDiagonalLeft ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, new Vector2(-1, 1)));
    }
}

