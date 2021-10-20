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

    [SerializeField] Color playerColor;
    [SerializeField] float deadZoneController;
    [SerializeField] float bufferTime;
    float raycastDistance = .5f;

    [SerializeField] float mvtHorizontal;
    [SerializeField] float mvtVertical;

    //timer
    float inputTimer;
    float beatPassedTimer;

    bool gotInput; //bool to start timer on input
    bool beatPassed; // bool true is rhythm missed
    bool hasMoved; // player moved, to block double movement
    PlayerDir playerDir = PlayerDir.NULL; //direction the player want

    //serounding checks
    bool canGoUp;
    bool canGoDown; 
    bool canGoRight;
    bool canGoLeft;

    //lerp
    Vector2 lastPos;
    Vector2 targetPos;
    #endregion

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
        WallCollision();
        GetInput();

        if (gotInput)
        {
            inputTimer += Time.deltaTime;
        }
        if (beatPassed)
        {
            beatPassedTimer += Time.deltaTime;
        }
        if(beatPassedTimer > bufferTime && beatPassed)
        {
            beatPassed = false;
            beatPassedTimer = 0;
            hasMoved = false;
            lastPos = targetPos;
        }
    }


    public void GetInput()
    {
        //move
        bool inputHorizontal = player.GetAxis("Move Horizontal") < -deadZoneController || player.GetAxis("Move Horizontal") > deadZoneController;
        bool inputVertical = player.GetAxis("Move Vertical") < -deadZoneController || player.GetAxis("Move Vertical") > deadZoneController;


        if (inputHorizontal && !hasMoved && !beatPassed) // before a beat
        {
            gotInput = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
        }
        else if (inputHorizontal && !hasMoved && beatPassed && beatPassedTimer < bufferTime) // after a beat
        {
            gotInput = true;
            mvtHorizontal = player.GetAxis("Move Horizontal");
            Move();
        }
        
        if (inputVertical && !hasMoved && !beatPassed) //before a beat
        {
            gotInput = true;
            mvtVertical = player.GetAxis("Move Vertical");
        }
        else if (inputVertical && !hasMoved && beatPassed && beatPassedTimer < bufferTime) //after a beat
        {
            gotInput = true;
            mvtVertical = player.GetAxis("Move Vertical");
            Move();
        }
    }

    public void Move()
    {
        if (inputTimer < bufferTime && mvtVertical != 0 && !hasMoved) //move vertical
        {
            if (mvtVertical > 0 && canGoUp)
            {
                targetPos.y = transform.position.y + 1;
            }
            else if (mvtVertical < 0 && canGoDown)
            {
                targetPos.y = transform.position.y - 1;
            }
            mvtVertical = 0;
            hasMoved = true;
        }

        if (inputTimer < bufferTime && mvtHorizontal != 0 && !hasMoved) //move horizontal
        {
            if (mvtHorizontal > 0 && canGoRight)
            {
                targetPos.x = transform.position.x + 1;
            }
            else if (mvtHorizontal < 0 && canGoLeft)
            {
                targetPos.x = transform.position.x - 1;
            }
            mvtHorizontal = 0;
            hasMoved = true;
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

    #region Collisions
    public void WallCollision()
    {
        RaycastHit2D rayray;
        rayray = Physics2D.Raycast(transform.position, Vector2.up, 1, LayerMask.GetMask("Ground"));
        canGoUp = rayray.collider == null;

        rayray = Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("Ground"));
        canGoDown = rayray.collider == null;

        rayray = Physics2D.Raycast(transform.position, Vector2.right, 1, LayerMask.GetMask("Ground"));
        canGoRight = rayray.collider == null;

        rayray = Physics2D.Raycast(transform.position, Vector2.left, 1, LayerMask.GetMask("Ground"));
        canGoLeft = rayray.collider == null;
        
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
        Gizmos.color = canGoUp ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.up));
        Gizmos.color = canGoDown ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.down));
        Gizmos.color = canGoLeft ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.left));
        Gizmos.color = canGoRight ? Color.green : Color.red;
        Gizmos.DrawRay(new Ray(transform.position, Vector2.right));

    }
}


enum PlayerDir
{
    NULL,
    UP,
    DOWN,
    RIGHT,
    LEFT,
}

