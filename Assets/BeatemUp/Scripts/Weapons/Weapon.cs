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
    protected bool gotInput;
    protected Vector2 lastDirection;
    protected float lastX = 1;
    protected float lastY = 1;

    // Weapon vars
    public int ComboToUpgarde;
    public int weaponKey = 0;

    [Header("---Bullets---")] // Bullets
    public GameObject bulletPrefab;

    public List<BulletInfo> bullets;

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
        if (player.GetAxisRaw("Aim Horizontal") != 0 || player.GetAxisRaw("Aim Vertical") != 0) // Last Aim Direction
        {
            float x = player.GetAxis("Aim Horizontal");
            float y = player.GetAxis("Aim Vertical");

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
        foreach (BulletInfo info in bullets)
        {
            Bullet blt = Instantiate(bulletPrefab, playerWeapon.transform.position, Quaternion.identity).GetComponent<Bullet>();

            Vector2 bulletDirection;
            if (info.lockOnX) bulletDirection = Vector2.right * lastX;
            else if (info.lockOnY) bulletDirection = Vector2.up * lastY;
            else if (info.lockDirection) bulletDirection = info.Direction;
            else bulletDirection = lastDirection;

            blt.InitInfo(info, bulletDirection);

            if (playerManager.comboManager != null) playerManager.comboManager.Keep(); //-------------
        }
    }

    public void Upgarde()
    {
        playerWeapon.SwapWeaponStyle((weaponKey + 1).ToString());
    }

    public void Downgrade()
    {
        playerWeapon.SwapWeaponStyle((weaponKey - 1).ToString());
    }
}

[System.Serializable]
public class BulletInfo
{
    public enum BulletDirection { Left, Right, Up, Down }

    public int length;
    public bool lockDirection;
    public BulletDirection direction;
    public Vector2 positionOffset;
    public bool lockOnX;
    public bool lockOnY;

    public Vector2 Direction
    {
        get
        {
            switch (direction)
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
    } 
}