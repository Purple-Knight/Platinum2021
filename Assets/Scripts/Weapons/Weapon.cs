using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : ScriptableObject
{
    //WeaponType
    public Bullet bullet;

    // Charge
    public int chargeBeats = 1;
    private protected int chargeLevel = 0;

    //Reload
    public int waitBeforeReload;

    #region Structs...

    #endregion

    #region Constructors
    public Weapon(Weapon W)
    {
        bullet = W.bullet;
        chargeBeats = W.chargeBeats;
        waitBeforeReload = W.waitBeforeReload;
    }
    #endregion

    public virtual void Use()
    {
        Debug.Log("Try Shoot...");

        if (chargeLevel < chargeBeats)
            Charge();
        else
            Fire();
    }

    public virtual void Charge()
    {
        chargeLevel++;
    }

    public virtual void Fire()
    {
        Debug.Log("FIRE!");
        ResetCharge();
    }

    public virtual void MissedBeat()
    {
        ResetCharge();
    }

    private protected void ResetCharge() { chargeLevel = 0; }
}

[System.Serializable]
public class Bullet
{
    [System.Serializable]
    public enum BulletType { Laser, Projectile }

    public BulletType type;

    private Vector2 direction = Vector2.right;
    public int maxTileLength = 0; // 0 = Infini ??

    public bool ignoreWalls = false;
    // public int power;

    public Bullet(BulletType _type, Vector2 _direction, bool _ignoresWalls)
    {
        type = _type;
        direction = _direction;
        ignoreWalls = _ignoresWalls;
    }
}