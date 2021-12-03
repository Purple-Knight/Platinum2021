
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Rewired;
using UnityEngine.Events;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    public int playerID = 0;
    public SpriteRenderer sprite;
    private PlayerManager playerManager;
    private Player player; //Rewired player
    private RhythmManager rhythmManager;
    public Animator playerAnimator;
    [SerializeField] private GameObject beatText;

    public Color playerColor;

    float halfBeatTime; 
    bool gotInputThisBeat = true; 

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
    public Vector2 gridSize;

    //public UnityEvent PlayerHit;
    [SerializeField] ParticleSystem impactParticles;
    Timing playerTiming;

    //Events 
    bool freeMovement = false;
    int maxNumOfMovement = 0;
    int currentNumOfSteps = 0;

     bool tpToWall = false;
     [SerializeField] int numbOfSteps = 0;

    #endregion

    [Header("Debug")]
    [SerializeField] private bool _guiDebug = true;
    [SerializeField] private Rect _guiDebugArea = new Rect(110, 20, 150, 150);

    public void InstantiateMovement()
    {
        rhythmManager = RhythmManager.Instance;
        rhythmManager.onMusicBeatDelegate += BeatReceived;
        rhythmManager.InstantiateBeat.AddListener(InstantiateRhythm);

        playerManager = GetComponent<PlayerManager>();
        gridSize = playerManager.GridSize;
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

            playerManager.comboManager.ResetComboValues(); // Call Combo Values Reset
        } 

        OtherPlayerOnNextTile();
    }


    public void GetInput()
    {
        //Is there an Input 
        bool inputHorizontal = player.GetAxis("Move Horizontal") !=0;
        bool inputVertical = player.GetAxis("Move Vertical") != 0;

        /*bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;
        bool inputVertical = player.GetAxis("Move Vertical") < -deadZoneController || player.GetAxis("Move Vertical") > deadZoneController;
        Debug.Log(player.GetAxis("Move Horizontal"));*/

        if ((inputHorizontal || inputVertical) && !gotInputThisBeat && !buttonDown)
        {
            if (!freeMovement)
            {
                playerTiming = rhythmManager.AmIOnBeat();
            }
            else
            {
                playerTiming = Timing.PERFECT;
                StartCoroutine(ResetFreeMovement());
            }

            if (playerTiming != Timing.MISS && playerTiming != Timing.NULL )
            {
                mvtVertical = player.GetAxis("Move Vertical");
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

                switch (playerDir)
                {
                    case PlayerDir.NULL:
                        break;
                    case PlayerDir.UP:
                        playerAnimator.SetFloat("DirectionHorizontal", 0);
                        playerAnimator.SetFloat("DirectionVertical", 1);
                        break;
                    case PlayerDir.DOWN:
                        playerAnimator.SetFloat("DirectionHorizontal", 0);
                        playerAnimator.SetFloat("DirectionVertical", -1);
                        break;
                    case PlayerDir.RIGHT:
                        playerAnimator.SetFloat("DirectionHorizontal", 1);
                        playerAnimator.SetFloat("DirectionVertical", 0);
                        break;
                    case PlayerDir.LEFT:
                        playerAnimator.SetFloat("DirectionHorizontal", -1);
                        playerAnimator.SetFloat("DirectionVertical", 0);
                        break;
                    default:
                        break;
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
                BeatTiming();
                playerAnimator.SetTrigger("BeatMissed");
            }

        }
        else if(!inputHorizontal && !inputVertical)
        {
            buttonDown = false;
            //playerDir = PlayerDir.NULL;
            mvtHorizontal = 0;
            mvtVertical = 0;
        }

/*        if (Input.GetKeyDown(KeyCode.W))
        {
            tpToWall = true;
        }*/
    }

    IEnumerator ResetFreeMovement()
    {
        yield return new WaitForSeconds(.3f);
        gotInputThisBeat = false;
        hasMoved = false;
    }

    public void Move()
    {
        lastPos = transform.position;
        targetPos = transform.position;


        if (mvtVertical != 0 && !hasMoved) //move vertical
        {
            if (tpToWall && mvtVertical > 0)
            {
                RaycastHit2D rayray;
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, gridSize.y / 2 + transform.position.y), Vector2.up, 20, LayerMask.GetMask("Ground", "Player"));
                targetPos = rayray.collider.transform.position;
                targetPos.y -= 1;
                
            }else if (tpToWall && mvtVertical < 0)
            {
                RaycastHit2D rayray;
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - gridSize.y), Vector2.down, 20, LayerMask.GetMask("Ground", "Player"));
                targetPos = rayray.collider.transform.position;
                targetPos.y += 1;
            }
            else if (mvtVertical > 0 && canGoUp)
            {
                if(numbOfSteps > 1)
                {
                    RaycastHit2D rayray;
                    rayray = Physics2D.Raycast(new Vector2(transform.position.x, gridSize.y / 2 + transform.position.y), Vector2.up, numbOfSteps * gridSize.y, LayerMask.GetMask("Ground"));
                    if(rayray.collider != null)
                    {
                        targetPos.y = rayray.collider.transform.position.y - gridSize.y;
                    }
                    else
                    {
                        targetPos.y = transform.position.y + numbOfSteps * gridSize.y;
                    }
                }
                else 
                {
                    targetPos.y = transform.position.y + gridSize.y;
                    
                }
                
                Squeeeesh(false);
            }
            else if (mvtVertical > 0 && !canGoUp) //if the player can't go up 
            {
                playerManager.comboManager.ResetComboValues(); // Call Combo Values Reset
                Squeeeesh(false);
            }
            else if (mvtVertical < 0 && canGoDown)
            {
                if (numbOfSteps > 1)
                {
                    RaycastHit2D rayray;
                    rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - gridSize.y), Vector2.down, numbOfSteps * gridSize.y, LayerMask.GetMask("Ground"));
                    if (rayray.collider != null)
                    {
                        targetPos.y = rayray.collider.transform.position.y + gridSize.y;
                    }
                    else
                    {
                        targetPos.y = transform.position.y - numbOfSteps;
                    }
                }
                else
                {
                    targetPos.y = transform.position.y - gridSize.y;
                }
                Squeeeesh(false);
            }
            else if (mvtVertical < 0 && !canGoDown) //if the player can't go down
            {
                playerManager.comboManager.ResetComboValues(); // Call Combo Values Reset
                Squeeeesh(false);
            }
            hasMoved = true;
        }
        else if (mvtHorizontal != 0 && !hasMoved) //move horizontal
        {
            if(tpToWall && mvtHorizontal > 0)
            {
                RaycastHit2D rayray;
                rayray = Physics2D.Raycast(new Vector2(transform.position.x + (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.right, 20, LayerMask.GetMask("Ground", "Player"));
                targetPos = rayray.collider.transform.position;
                targetPos.x -= 1;

            }else if (tpToWall && mvtHorizontal < 0)
            {
                RaycastHit2D rayray;
                rayray = Physics2D.Raycast(new Vector2(transform.position.x - (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.left, 20, LayerMask.GetMask("Ground", "Player"));
                targetPos = rayray.collider.transform.position;
                targetPos.x += 1;
            }
            else if (mvtHorizontal > 0 && canGoRight)
            {
                if (numbOfSteps > 1)
                {
                    RaycastHit2D rayray;
                    rayray = Physics2D.Raycast(new Vector2(transform.position.x + (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.right, numbOfSteps * gridSize.x, LayerMask.GetMask("Ground"));
                    if (rayray.collider != null)
                    {
                        targetPos.x = rayray.collider.transform.position.x - gridSize.x;
                    }
                    else
                    {
                        targetPos.x = transform.position.x + numbOfSteps;
                    }
                }
                else
                {
                    targetPos.x = transform.position.x + gridSize.x;
                }
                Squeeeesh(true);
            }
            else if (mvtHorizontal > 0 && !canGoRight) //if the player can't go right  
            {
                playerManager.comboManager.ResetComboValues(); // Call Combo Values Reset
                Squeeeesh(false);
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                if (numbOfSteps > 1)
                {
                    RaycastHit2D rayray;
                    rayray = Physics2D.Raycast(new Vector2(transform.position.x - (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.left, numbOfSteps * gridSize.x, LayerMask.GetMask("Ground"));
                    if (rayray.collider != null)
                    {
                        targetPos.x = rayray.collider.transform.position.x + gridSize.x;
                    }
                    else
                    {
                        targetPos.x = transform.position.x - numbOfSteps;
                    }
                }
                else
                {
                    targetPos.x = transform.position.x - gridSize.x;
                }
                Squeeeesh(true);
            }
            else if (mvtHorizontal < 0 && !canGoLeft) //if the player can't go left 
            {
                playerManager.comboManager.ResetComboValues(); // Call Combo Values Reset
                Squeeeesh(false);
            }
            hasMoved = true;
        }

        mvtVertical = 0;
        mvtHorizontal = 0;

        BeatTiming();
        if (freeMovement && DOTween.IsTweening(transform)) 
            transform.DOComplete();
        if (!freeMovement || (freeMovement && currentNumOfSteps < maxNumOfMovement))
        {
            playerAnimator.SetTrigger("Move");
            transform.DOMove(targetPos, .2f);
            currentNumOfSteps++;
        }

        playerManager.comboManager.Up();
    }

    public void BeatReceived()
    {
        beatPassed = true;
        if(sprite != null  && !DOTween.IsTweening(sprite.transform)) 
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
        rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, gridSize.y/2, LayerMask.GetMask("Ground"));
        canGoUp = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - gridSize.y), Vector2.down, gridSize.y/2, LayerMask.GetMask("Ground"));
        canGoDown = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x + (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.right, gridSize.x/2, LayerMask.GetMask("Ground"));
        canGoRight = rayray.collider == null;

        rayray = Physics2D.Raycast(new Vector2(transform.position.x - (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.left, gridSize.x/2, LayerMask.GetMask("Ground"));
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
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, gridSize.y / 2 + transform.position.y), Vector2.up, gridSize.y/2, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.DOWN:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - gridSize.y), Vector2.down, gridSize.y / 2, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.RIGHT:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x + (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.right, gridSize.x/2, LayerMask.GetMask("Player"));
                break;
            case PlayerDir.LEFT:
                rayray = Physics2D.Raycast(new Vector2(transform.position.x - (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.left, gridSize.x/2, LayerMask.GetMask("Player"));
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
        Instantiate(beatText, transform.position, Quaternion.identity).GetComponent<BeatTimingText>().InstantiateText(playerTiming);
        
    }

    #endregion

    public void InstantiateRhythm()
    {
        halfBeatTime = rhythmManager.beatDuration / 2;
    }
    
    public void StartFreeMovement(int maxNumOfSteps)
    {
        currentNumOfSteps = 0;
        maxNumOfMovement = maxNumOfSteps;
        freeMovement = true;
        StartCoroutine(ResetSteps());
    }
    
    public void EndFreeMovement()
    {
        freeMovement = false;
        currentNumOfSteps = 0;
        maxNumOfMovement = 0;
    }

    IEnumerator ResetSteps()
    {
        while (freeMovement)
        {
            yield return new WaitForSeconds(1);
            currentNumOfSteps = 0;
        }
    }

    #region Debug

    private void OnGUI()
    {
        if (!_guiDebug) return;

        GUILayout.BeginArea(_guiDebugArea);
        GUILayout.TextArea("Player " + playerID);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Tp to walls " + tpToWall))
        {
            tpToWall = !tpToWall;
        }

        GUILayout.EndHorizontal();


        GUILayout.EndArea();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = canGoUp ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x, transform.position.y), Vector2.up));
        Gizmos.color = canGoDown ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x, transform.position.y - gridSize.y), Vector2.down));
        Gizmos.color = canGoRight ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x + (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.right));
        Gizmos.color = canGoLeft ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(new Vector2(transform.position.x - (gridSize.x / 2), transform.position.y - (gridSize.y / 2)), Vector2.left));

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

