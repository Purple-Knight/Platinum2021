using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Weapon : MonoBehaviour
{
    // Player
    protected PlayerWeapon playerWeapon;
    public int PlayerID { get => player.id; }

    // Inputs
    protected Player player;
    protected Timing playerTiming;
    protected bool gotInput;
    protected Vector2 lastDirection = Vector2.right;
        // Editor lockLastDirection  X  Y

    // Weapon vars
    public int ComboToUpgarde;
    public int weaponKey = 0;

    [Header("---Bullets---")] // Bullets
    public GameObject bulletPrefab;

    public List<BulletInfo> bullets;

    private void Update()
    {
        GetAxisInput();
    }

    public void Init(Player _player, PlayerWeapon _playerWeapon)
    {
        player = _player;
        playerWeapon = _playerWeapon;
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

            playerWeapon.UpdateAimVisual(lastDirection);
        }
    }

    public virtual void GetInput() { }
    public virtual void Fire()
    {
        foreach (BulletInfo info in bullets)
        {
            //PEW!
            Bullet blt = Instantiate(bulletPrefab, playerWeapon.transform.position, Quaternion.identity).GetComponent<Bullet>();
            blt.InitInfo(info, lastDirection);
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
public struct BulletInfo
{
    public int length;
    //public Vector2 positionOffset;
}