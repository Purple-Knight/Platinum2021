using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    public int playerID = 0;
    public SpriteRenderer sprite;
    private Player player; //Rewired player
    private RhythmManager rhythmManager;

    [SerializeField] Color playerColor;
    [SerializeField] float deadZoneController;
    float bufferTime;
    float halfBeatTime;
    bool gotInputThisBeat;
    float raycastDistance = .5f;

    float mvtHorizontal;
    float mvtVertical;

    //timer
    float inputTimer;
    float beatPassedTimer;

    bool beforeBeatTimer; //bool to start timer on input
    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    PlayerDir playerDir = PlayerDir.NULL; //direction the player want
    bool buttonDown;
    bool canMove = true;

    //serounding checks
    bool canGoUp;
    bool canGoDown; 
    bool canGoRight;
    bool canGoLeft;

    //lerp
    Vector2 targetPos;
    Vector2 lastPos;
    #endregion

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = true;
    private bool _timerDebug = false;
    private bool _boolDebug = false;
    [SerializeField] private Rect _guiDebugArea = new Rect(0, 20, 150, 150);

    private void Start()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onMusicBeatDelegate += BeatReceived;
        rhythmManager.InstantiateBeat.AddListener(InstantiateRhythm);

        player = ReInput.players.GetPlayer(playerID);
        sprite.color = playerColor;
        targetPos = transform.position;
        lastPos = targetPos;
    }

    private void Update()
    {
        WallCollision();

        GetInput();

        if (beforeBeatTimer)
        {
            inputTimer += Time.deltaTime;
        }
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;
        }
        if(beatPassedTimer > bufferTime && beatPassed)
        {
            hasMoved = false;
        }
        if (beatPassed && beatPassedTimer >= halfBeatTime)
        {
            beatPassed = false;
            beatPassedTimer = 0;
            gotInputThisBeat = false;
            
        }

        OtherPlayerOnNextTile();
    }


    public void GetInput()
    {
        //move
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;
        bool inputVertical = player.GetAxis("Move Vertical") < -deadZoneController || player.GetAxis("Move Vertical") > deadZoneController;


        if (inputHorizontal && !hasMoved && !beatPassed && !buttonDown && !gotInputThisBeat) // before a beat
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            playerDir = mvtHorizontal > 0? PlayerDir.RIGHT: PlayerDir.LEFT;
        }
        else if (inputHorizontal && !hasMoved && beatPassed && beatPassedTimer < bufferTime && !buttonDown  && ! !gotInputThisBeat) // after a beat
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
            buttonDown = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            playerDir = mvtHorizontal > 0 ? PlayerDir.RIGHT : PlayerDir.LEFT;
            Move();
        }else if (player.GetAxis("Move Horizontal") > -deadZoneController && player.GetAxis("Move Horizontal") < deadZoneController)
        {
            mvtHorizontal = 0;
        }
        
        if (inputVertical && !hasMoved && !beatPassed && !buttonDown && !gotInputThisBeat) //before a beat
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
            buttonDown = true;
            mvtVertical = player.GetAxis("Move Vertical");
            playerDir = mvtVertical > 0 ? PlayerDir.UP : PlayerDir.DOWN;
        }
        else if (inputVertical && !hasMoved && beatPassed && beatPassedTimer < bufferTime && !buttonDown && !gotInputThisBeat) //after a beat
        {
            gotInputThisBeat = true;
            beforeBeatTimer = true;
            buttonDown = true;
            mvtVertical = player.GetAxis("Move Vertical");
            playerDir = mvtVertical > 0 ? PlayerDir.UP : PlayerDir.DOWN;
            Move();
        }else if(player.GetAxis("Move Vertical") > -deadZoneController && player.GetAxis("Move Vertical") < deadZoneController)
        {
            mvtVertical = 0;
        }
        
        if(!inputHorizontal && !inputVertical)
        {
            buttonDown = false;
            playerDir = PlayerDir.NULL;
        }
    }

    public void Move()
    {
        lastPos = targetPos;
        if (inputTimer < bufferTime && mvtVertical != 0 && !hasMoved) //move vertical
        {
            if (mvtVertical > 0 && canGoUp)
            {
                targetPos.y = transform.position.y + 1;
                playerDir = PlayerDir.UP;
            }
            else if (mvtVertical < 0 && canGoDown)
            {
                targetPos.y = transform.position.y - 1;
                playerDir = PlayerDir.DOWN;
            }
            hasMoved = true;
        }
        else if (inputTimer < bufferTime && mvtHorizontal != 0 && !hasMoved) //move horizontal
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                targetPos.x = transform.position.x + 1;
                playerDir = PlayerDir.RIGHT;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                targetPos.x = transform.position.x - 1;
                playerDir = PlayerDir.LEFT;
            }
            hasMoved = true;
        }

        mvtVertical = 0;
        mvtHorizontal = 0;

        HitResult();
        transform.DOMove(targetPos, .2f);

        beforeBeatTimer = false;
    }

    public void BeatReceived()
    {
        beatPassed = true;
        Move();
        inputTimer = 0;
        //StartCoroutine(LerpMove());
        Squeeeesh();
    }

    #region Collisions
    public void WallCollision()
    {
        RaycastHit2D rayray;
        rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .5f), Vector2.up, raycastDistance, LayerMask.GetMask("Ground"));
        canGoUp = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .5f), Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));
        canGoDown = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x + .5f, transform.position.y ), Vector2.right, raycastDistance, LayerMask.GetMask("Ground"));
        canGoRight = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y ), Vector2.left, raycastDistance, LayerMask.GetMask("Ground"));
        canGoLeft = rayray.collider == null;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!canMove)
            {
                transform.DOComplete();
                transform.DOMove(lastPos, .1f);
                targetPos = lastPos;
            }
        }
    }

    public void OtherPlayerOnNextTile()
    {
        RaycastHit2D rayray;
        rayray = Physics2D.Raycast(Vector2.zero, Vector2.zero, 0, LayerMask.GetMask());
        switch (playerDir)
        {
            case PlayerDir.NULL:
                break;
            case PlayerDir.UP:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + .5f), Vector2.up, raycastDistance, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.DOWN:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - .5f), Vector2.down, raycastDistance, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.RIGHT:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x + .5f, transform.position.y), Vector2.right, raycastDistance, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.LEFT:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y), Vector2.left, raycastDistance, LayerMask.GetMask("Ground"));
                break;
        }
        if (rayray.collider != null)
            canMove = false;
        else
            canMove = true;
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
                //Debug.Log("<color=red> missed </color>");
                seqColor.Append(sprite.DOColor(Color.red, .2f));
            }

            seqColor.Append(sprite.DOColor(playerColor, .1f));
        }
    }

    #endregion

    /*
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

        }*/

    public void InstantiateRhythm()
    {
        bufferTime = rhythmManager.beatDuration / 3;
        halfBeatTime = rhythmManager.beatDuration / 2;
    }
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
            GUILayout.TextField("Timers \n" + "Input Timer : " + inputTimer + "\n" + "beat passed timer : " + beatPassedTimer);
        }
        if (_boolDebug)
        {
            GUILayout.TextField("Booleans \n" + "Has moved : " + hasMoved + "\n" + "Got Input this beat : " + gotInputThisBeat + "\n Before Beat Timer : "  + beforeBeatTimer + "\n Button down : " + buttonDown);
        }
        GUILayout.EndArea();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = canGoUp ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x, transform.position.y + .5f), Vector2.up));
        Gizmos.color = canGoDown ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x, transform.position.y - .5f), Vector2.down));
        Gizmos.color = canGoLeft ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x -.5f, transform.position.y), Vector2.left));
        Gizmos.color = canGoRight ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x + .5f, transform.position.y ), Vector2.right));

    }

    #endregion
}


enum PlayerDir
{
    NULL,
    UP,
    DOWN,
    RIGHT,
    LEFT,
}

