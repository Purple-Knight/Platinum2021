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
    RhythmManager rhythmManager;
    float raycastDistance = .5f;
    float mvtHorizontal;
    float jump;
    float inputTimer;
    float beatPassedTimer;
    float halfBeatTime;
    bool gotInputThisBeat;
    bool beforeBeatTimer; //bool to start timer on input
    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    bool hasJumped; //player jumped
    bool wasInAir; //player was in the air the last beat
    bool buttonDown; //check if buttons stays down
    bool jumpButtonDown; //check if buttons stays down

    //serounding checks
    bool isOnFloor; // check if player is grounded
    bool canGoRight;
    bool canGoLeft;
    bool canGoDiagonalRight;
    bool canGoDiagonalLeft;
    bool canjump;
    bool canFall;

    //lerp
    Vector2 lastPos;
    Vector2 targetPos;

    bool doOnce = false;
    [Header("Debug")]
    [SerializeField] private bool _guiDebug = true;
    private bool _timerDebug = false;
    private bool _boolDebug = false;
    [SerializeField] private Rect _guiDebugArea = new Rect(0, 20, 150, 150);

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onMusicBeatDelegate += BeatReceived;

        player = ReInput.players.GetPlayer(playerID);
        sprite.color = playerColor;
        lastPos = transform.position;
        targetPos = transform.position;
    }

    private void Update()
    {
        Gravity();
        WallCollision();
        GetInput();
        if (beforeBeatTimer)
        {
            inputTimer += Time.deltaTime;
        }
        if ( beatPassed)
        {
            beatPassedTimer += Time.deltaTime;
        }
        if(beatPassedTimer > bufferTime && beatPassed)
        {
            hasMoved = false;
            hasJumped = false;
        }
        if(beatPassed && beatPassedTimer >= halfBeatTime)
        {
            beatPassed = false;
            beatPassedTimer = 0;
            gotInputThisBeat = false;
            lastPos = targetPos;
        }
    }


    public void GetInput()
    {
        //move
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;

        if (inputHorizontal && !buttonDown && !hasMoved && !beatPassed)
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
        }
        else if (inputHorizontal && !buttonDown && !hasMoved && beatPassed && beatPassedTimer < bufferTime && !gotInputThisBeat)
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
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
            beforeBeatTimer = true;
            jump = player.GetAxis("Move Vertical");
        }
        else if (inputVertical && !jumpButtonDown && !hasJumped && beatPassed && beatPassedTimer < bufferTime) // apres le premier beat
        {
            jumpButtonDown = true;
            beforeBeatTimer = true;
            jump = player.GetAxis("Move Vertical");
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
            targetPos.y = transform.position.y - 1;
            hasMoved = true;
            wasInAir = true;

        } else if (isOnFloor && canFall && jump < -deadZoneController && !hasMoved)
        {
            targetPos.y = transform.position.y - 1;
            hasMoved = true;
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
        else if(isOnFloor && inputTimer <bufferTime && mvtHorizontal != 0 && jump > 0 && !hasJumped && !hasMoved) //diagonal
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
               
        beforeBeatTimer = false;
    }

    public void BeatReceived()
    {
        if (!doOnce)
        {
            doOnce = true;
            bufferTime = rhythmManager.beatDuration / 3;
            halfBeatTime = rhythmManager.beatDuration / 2;
            Debug.Log(bufferTime + "  --  " + halfBeatTime);
        }

        beatPassed = true;
        Move();
        inputTimer = 0;
        StartCoroutine(LerpMove());
        Squeeeesh();
    }
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

    #region GravityAndCollisions
    //raycast O.O *u* hello there :)))) watcha ray casting on?
    public void Gravity()
    {
        RaycastHit2D rayrayFall = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance , LayerMask.GetMask("Ground"));

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

        if (rayrayFall.collider != null && rayrayFall.collider.CompareTag("OneWayPlatform"))
        {
            canFall = true;
        }else
        {
            canFall = false;
        }
    }

    public void WallCollision()
    {
        RaycastHit2D rayray = Physics2D.Raycast(transform.position, Vector2.up, raycastDistance, LayerMask.GetMask("Ground"));
        if(rayray.collider != null && !rayray.collider.CompareTag("OneWayPlatform"))
        {
            canjump = false;
        }
        else
        {
            canjump = true;
        }

        rayray = Physics2D.Raycast(transform.position, Vector2.right, raycastDistance, LayerMask.GetMask("Ground"));
        canGoRight = rayray.collider == null ;

        rayray = Physics2D.Raycast(transform.position, new Vector2(1, 1), raycastDistance, LayerMask.GetMask("Ground"));
        if (rayray.collider != null && !rayray.collider.CompareTag("OneWayPlatform"))
        {
            canGoDiagonalRight = false;
        }
        else
        {
            canGoDiagonalRight = true;
        }

        rayray = Physics2D.Raycast(transform.position, Vector2.left, raycastDistance, LayerMask.GetMask("Ground"));
        canGoLeft = rayray.collider == null;
        
        rayray = Physics2D.Raycast(transform.position, new Vector2(-1,1) , raycastDistance, LayerMask.GetMask("Ground"));
        if (rayray.collider != null && !rayray.collider.CompareTag("OneWayPlatform"))
        {
            canGoDiagonalLeft = false;
        }
        else
        {
           canGoDiagonalLeft= true;
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



    #region Debug
    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Timers"))
        {
            _timerDebug = !_timerDebug;
        }
        if (GUILayout.Button("bool"))
        {
            _boolDebug = !_boolDebug;
        }

        GUILayout.BeginArea(_guiDebugArea);

        if (_timerDebug)
        {
            GUILayout.TextField("Timers \n" + "Input Timer : " + inputTimer +"\n" + "beat passed timer : " + beatPassedTimer);
        }
        if (_boolDebug)
        {
            GUILayout.TextField("Booleans \n" + "Has moved : " + hasMoved + "\n" + "Has Jumped : " + hasJumped + "\n" + "Was in air : " + wasInAir + "\n" + "Got Input this beat :" + gotInputThisBeat);
        }
        GUILayout.EndArea();
    }

    private void OnDrawGizmosSelected()
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
    #endregion
}

