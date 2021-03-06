using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Weapon : MonoBehaviour
{
    // Player
    protected PlayerWeapon playerWeapon;
    protected PlayerManager playerManager;

    // Inputs
    protected Player player;
    protected Timing playerTiming;
    protected bool GotInput { get => playerManager.GotInputThisBeat; set => playerManager.GotInputThisBeat = value; }
    protected Vector2 lastDirection;
    protected float lastX = 1;
    protected float lastY = 1;

    // Weapon Var
    public int ComboToUpgrade; // Upgrade limit
    public int ComboToDowngrade; // Downgrade limit
    public int weaponKey = 0;

    [Header("---Bullets---")] // Bullets
    public GameObject bulletPrefab;
    public List<BulletInfo> bullets;

    [Header("")]
    public Color comboTextColor = new Color(1, 1, 1, 1);

    public Vector2 GetDirection { get => lastDirection; }
    public Vector2 PlayerPosistion { get => playerManager.transform.position; }
    public int CharacterID { get => playerManager.CharacterID; }

    private void Update()
    {
        GetAxisInput();
    }

    public void Init(Player _player, PlayerManager _playerManager, PlayerWeapon _playerWeapon, Vector2 _direction)
    {
        player = _player;
        playerManager = _playerManager;
        playerWeapon = _playerWeapon;
        lastDirection = _direction;
    }

    public void GetAxisInput()
    {
        float x = (player.GetAxisRaw("Move Horizontal") != 0) ? player.GetAxis("Move Horizontal") : player.GetAxis("Aim Horizontal");
        float y = (player.GetAxisRaw("Move Vertical") != 0) ? player.GetAxis("Move Vertical") : player.GetAxis("Aim Vertical");

        if (x != 0 || y != 0) // Last Aim Direction
        {
            //x = player.GetAxis("Aim Horizontal");
            //y = player.GetAxis("Aim Vertical");

            if (Mathf.Abs(x) >= Mathf.Abs(y))
                y = 0;
            else
                x = 0;

            lastDirection.x = (x == 0) ? x : Mathf.Sign(x);
            lastDirection.y = (y == 0) ? y : Mathf.Sign(y);

            if (x != 0) lastX = lastDirection.x;
            if (y != 0) lastY = lastDirection.y;

            playerWeapon.UpdateAimVisual(lastDirection);
        }
    }

    public virtual void GetInput() { }
    public virtual void Fire()
    {
        if (bullets.Count <= 0) return;

        playerManager.CurrentSwitch.SetValue(RhythmManager.Instance.gameObject);
        RhythmManager.Instance.PlayShotSound();

        playerManager.playerAnimator.SetTrigger("Fire");

        playerManager.Vibrations(1, 10, .1f);
        foreach (BulletInfo info in bullets)
        {
            //Locked Directions
            Vector2 bulletDirection;
            if (info.lockDirection) bulletDirection = info.getLockedDirection;
            else if (info.lockOnY) bulletDirection = Vector2.up * lastY;
            else if (info.lockOnX) bulletDirection = Vector2.right * lastX;
            else
            {
                //Angle Offset 
                bulletDirection = info.getOffsetDirection(lastDirection);
            }

            Vector2 bulletPosition = PlayerPosistion;
            bulletPosition.y -= .25f;
            //bulletDirection.x = Mathf.RoundToInt(bulletDirection.x);
            //bulletDirection.y = Mathf.RoundToInt(bulletDirection.y);
            //Position Offset
            bulletPosition += info.getPositionOffset(bulletDirection) * playerManager.GridSize;


            //Spawn
            Bullet blt = Instantiate(bulletPrefab, bulletPosition, Quaternion.identity).GetComponent<Bullet>();
            blt.InitInfo(info, bulletDirection, playerManager);

            if (playerManager.comboManager != null) playerManager.comboManager.Keep(); //-------------
        }
    }

    public void Upgarde()
    {
        playerWeapon.SwapWeaponStyle((weaponKey + 1).ToString());
        playerManager.comboManager.WeaponSwapFeedback(true);
        /*
                Debug.Log("up  : " +weaponKey + " -- " +weaponKey * 33);
                switch (playerManager.CharacterID)
                {
                    case 0:
                        RhythmManager.Instance.setTrackVolume(Track.GTR, weaponKey * 33);
                        break;
                    case 1:
                        RhythmManager.Instance.setTrackVolume(Track.SYNTHE, weaponKey * 33);
                        break;
                    case 2:
                        RhythmManager.Instance.setTrackVolume(Track.HARPE, weaponKey * 33);
                        break;
                    default:
                        break;
                }*/
    }

    public void Downgrade()
    {
        playerWeapon.SwapWeaponStyle((weaponKey - 1).ToString());
        playerManager.comboManager.WeaponSwapFeedback(false);

/*        Debug.Log("down  : " +weaponKey + " -- " + weaponKey * 33);

        switch (playerManager.CharacterID)
        {
            case 0:
                RhythmManager.Instance.setTrackVolume(Track.GTR, weaponKey * 33);
                break;
            case 1:
                RhythmManager.Instance.setTrackVolume(Track.SYNTHE, weaponKey * 33);
                break;
            case 2:
                RhythmManager.Instance.setTrackVolume(Track.HARPE, weaponKey * 33);
                break;
            default:
                break;
        }*/
    }
}

[System.Serializable]
public class BulletInfo
{
    public enum BulletDirection { Left, Up, Right, Down }
    public enum DirectionOffset { Forward, RightSide, Backward, LeftSide }

    public int length;

    public Vector2 positionOffset;
    public DirectionOffset inputDirectionOffset = DirectionOffset.Forward;
    
    public bool lockDirection;
    public BulletDirection lockIntoDirection;
    
    public bool lockOnX;
    public bool lockOnY;

    private Vector2 EnumToVector2(BulletDirection enumDir)
    {
        switch (enumDir)
        {
            case BulletInfo.BulletDirection.Left:
                return Vector2.left;
            case BulletInfo.BulletDirection.Right:
                return Vector2.right;
            case BulletInfo.BulletDirection.Up:
                return Vector2.up;
            case BulletInfo.BulletDirection.Down:
                return Vector2.down;
            default:
                break;
        }
        Debug.Log("<color=red>Invalid Direction !!!</color>");
        return Vector2.zero;
    }

    private BulletDirection Vector2ToEnum(Vector2 dir)
    {
        if (dir == Vector2.left) return BulletDirection.Left;
        else if (dir == Vector2.up) return BulletDirection.Up;
        else if (dir == Vector2.right) return BulletDirection.Right;
        else /*(dir == Vector2.down)*/ return BulletDirection.Down;
    }

    public Vector2 getLockedDirection { get => EnumToVector2(lockIntoDirection); }

    public Vector2 getOffsetDirection(Vector2 input)
    {
        if (inputDirectionOffset == DirectionOffset.Forward) return input;

        int output = (int)(Vector2ToEnum(input)) + (int)inputDirectionOffset;
        if (output >= 4) output -= 4;
        return (EnumToVector2((BulletDirection)output));
    }

    public Vector2 getPositionOffset(Vector2 inputDir)
    {
        switch (Vector2ToEnum(inputDir))
        {
            case BulletDirection.Left:
                return new Vector2(-positionOffset.x, -positionOffset.y);
            case BulletDirection.Up:
                return new Vector2(-positionOffset.y, positionOffset.x);
            case BulletDirection.Down:
                return new Vector2(positionOffset.y, -positionOffset.x);
            default: // case Right;
                return positionOffset;
        }
    }
}