using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    public int playerID = 0;
    public SpriteRenderer sprite;
    private Player player; //Rewired player
    private RhythmManager rhythmManager;

    public Color playerColor;
    [SerializeField] float deadZoneController;

    float halfBeatTime; 
    bool gotInputThisBeat; 
    float raycastDistance = .5f;

    float mvtHorizontal;
    float mvtVertical;

    //timer
    float beatPassedTimer;

    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    [SerializeField] PlayerDir playerDir = PlayerDir.NULL; //direction the player want
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

    //public UnityEvent PlayerHit;
    [SerializeField] ParticleSystem impactParticles;
    Timming playerTiming;

    #endregion

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = true;
    private bool _boolDebug = false;
    [SerializeField] private Rect _guiDebugArea = new Rect(0, 20, 150, 150);

    public void InstantiateMovement()
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
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;
        }
        if (beatPassed && beatPassedTimer >= halfBeatTime) //only one input per beat
        {
            beatPassed = false;
            gotInputThisBeat = false;
            beatPassedTimer = 0;
            hasMoved = false;
            
        } 

        OtherPlayerOnNextTile();
    }


    public void GetInput()
    {
        //Is there an Input 
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;
        bool inputVertical = player.GetAxis("Move Vertical") < -deadZoneController || player.GetAxis("Move Vertical") > deadZoneController;
        
       
        if (inputHorizontal || inputVertical) 
        {
            playerTiming = rhythmManager.AmIOnBeat();

            if (playerTiming != Timming.MISS && playerTiming != Timming.NULL && !buttonDown && !gotInputThisBeat)
            {
                mvtVertical =player.GetAxis("Move Vertical");
                mvtHorizontal = player.GetAxis("Move Horizontal");

                if (Mathf.Abs(mvtVertical) > Mathf.Abs(mvtHorizontal)) 
                {
                    playerDir = mvtVertical > 0 ? PlayerDir.UP : PlayerDir.DOWN;
                    mvtHorizontal = 0;
                }
                else
                {
                    playerDir = mvtHorizontal > 0 ? PlayerDir.RIGHT : PlayerDir.LEFT;
                    mvtVertical = 0;
                }
                
                buttonDown = true;
                gotInputThisBeat = true;
                Move();
            } 
            else
            {
                buttonDown = true;
                gotInputThisBeat = true;
                //playerDir = PlayerDir.NULL;
                mvtHorizontal = 0;
                mvtVertical = 0;
            }
        }
        else
        {
            buttonDown = false;
            //playerDir = PlayerDir.NULL;
            mvtHorizontal = 0;
            mvtVertical = 0;
        }
    }

    public void Move()
    {
        lastPos = transform.position;
        targetPos = transform.position;


        if (mvtVertical != 0 && !hasMoved) //move vertical
        {
            if (mvtVertical > 0 && canGoUp)
            {
                targetPos.y = transform.position.y + 1;
                Squeeeesh(false);
            }
            else if (mvtVertical < 0 && canGoDown)
            {
                targetPos.y = transform.position.y - 1;
                Squeeeesh(false);
            }
            hasMoved = true;
        }
        else if (mvtHorizontal != 0 && !hasMoved) //move horizontal
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                targetPos.x = transform.position.x + 1;
                sprite.flipX = true;
                Squeeeesh(true);
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                targetPos.x = transform.position.x - 1;
                sprite.flipX = false;
                Squeeeesh(true);
            }
            hasMoved = true;
        }

        mvtVertical = 0;
        mvtHorizontal = 0;

        BeatTiming();
        transform.DOMove(targetPos, .2f);

        
    }

    public void BeatReceived()
    {
        beatPassed = true;
        if(!DOTween.IsTweening(sprite.transform)) 
            Squeeeesh(true);
    }


    public void ResetPositions()
    {
        targetPos = transform.position;
        lastPos = transform.position;
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
                Instantiate(impactParticles, transform.position, Quaternion.identity);
                transform.DOComplete();
                targetPos = lastPos;
                Sequence seq = DOTween.Sequence();
                seq.Append(transform.DOMove(targetPos, .1f));
                seq.Insert(0, sprite.transform.DOScale(1.15f, .1f));
                seq.Insert(0, sprite.transform.DOScale(.85f, .1f));
                seq.Append(sprite.transform.DOScale(1, .2f).SetEase(Ease.OutElastic));
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
                rayray = Physics2D.Raycast(new Vector2(transform.position.x - .5f, transform.position.y), Vector2.left, raycastDistance, LayerMask.GetMask("Player"));
                break;
        }
        if (rayray.collider != null)
            canMove = false;
        else
            canMove = true;
    }
    #endregion

    #region feedback
    public void Squeeeesh(bool squeeshY)
    {
        Sequence seq = DOTween.Sequence();
        if(squeeshY)
            seq.Append(sprite.transform.DOScaleY(.8f, .1f)).Append(sprite.transform.DOScaleY(1, 0.1f));
        else
            seq.Append(sprite.transform.DOScaleX(.8f, .1f)).Append(sprite.transform.DOScaleX(1, 0.1f));
        seq.Play();
    }

    public void BeatTiming()
    {
        Sequence seqColor = DOTween.Sequence();
        switch (playerTiming)
        {
            case Timming.PERFECT:
                seqColor.Append(sprite.DOColor(new Color(0,1,0,playerColor.a), .1f));
                break;
            case Timming.BEFORE:
                seqColor.Append(sprite.DOColor(new Color(0, .4f, .4f, playerColor.a), .1f));
                break;
            case Timming.AFTER:
                seqColor.Append(sprite.DOColor(new Color(0, .4f, .4f, playerColor.a), .1f));
                break;
            case Timming.MISS:
                seqColor.Append(sprite.DOColor(new Color(1, 0, 0, playerColor.a), .1f));
                break;
            case Timming.NULL:
                
                break;
            
        } 
        seqColor.Append(sprite.DOColor(playerColor, .1f));
        
    }

    #endregion

    public void InstantiateRhythm()
    {
        halfBeatTime = rhythmManager.beatDuration / 2;
    }
    #region Debug

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginArea(_guiDebugArea);
        GUILayout.TextArea("Player " + playerID);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("bool"))
        {
            _boolDebug = !_boolDebug;
        }

        GUILayout.EndHorizontal();

        if (_boolDebug)
        {
            GUILayout.TextField("Booleans \n" + "Has moved : " + hasMoved + "\n" + "Got Input this beat : " + gotInputThisBeat + "\n Button down : " + buttonDown +  "\n Vertical mvt : " + mvtVertical );
        }
        GUILayout.EndArea();
    }

    private void OnDrawGizmosSelected()
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

