using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public abstract class Weapon : MonoBehaviour
{
    /// Inputs
    protected Player player;
    protected Timing playerTiming;
    protected bool gotInput;
    protected Vector2 lastDirection = Vector2.right;

    // Weapon vars
    public int ComboToUpgarde;
    public Weapon previousWeapon;
    public Weapon nextWeapon;

    [Header("---Bullets---")]
    public GameObject bltPrefab;

    public List<BulletInfo> bullets;

    public void Init(Player _player)
    {
        player = _player;
    }

    private void Update()
    {
        
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

            //aiming.position = new Vector2(transform.position.x + (lastDirection.x * .7f), transform.position.y + (lastDirection.y * .7f)); // Cursor

        }
    }

    public virtual void GetInput() { }
    public virtual void Fire() { } // foreach BulletInfo in bullets
}

[System.Serializable]
public struct BulletInfo
{
    public int length;
    public Vector2 positionOffset;
}