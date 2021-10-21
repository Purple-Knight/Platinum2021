using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Weapon", menuName = "Weapon")]
[System.Serializable]
public class Weapon : ScriptableObject
{
    //WeaponType
    public BulletInfo bulletInfo;

    // Bullet
    public GameObject bulletPrefab;

    // Charge
    public int chargeBeats = 1;             // charge Time needed
    private protected int chargeLevel = 0;  // current Charge

    //Reload
    public int waitBeforeReload;

    //Player Ref
    internal Vector2 lastDirection;
    [SerializeField] internal PlayerMovement pMov;

    #region Structs...

    #endregion

    public virtual void Use(bool triggerDown)
    {
        if(triggerDown) // Button Down
        {
            Charge();
            //Debug.Log("Try Shoot...");
        }
        else    // Button Up
        {
            if (chargeLevel == chargeBeats)
                Fire();
            else
                MissedBeat();
        }
    }

    public virtual void Charge()
    {
        chargeLevel++;
    }

    public virtual void Fire()
    {
        Debug.Log("FIRE! " + chargeLevel + " charges");

        //Instantiate Bullet
        Bullet blt = Instantiate(bulletPrefab, pMov.transform.position, Quaternion.identity).GetComponent<Bullet>();
        blt.InitInfo(bulletInfo, lastDirection);

        ResetCharge();
    }

    public virtual void MissedBeat()
    {
        ResetCharge();
    }

    private protected void ResetCharge() { chargeLevel = 0; Debug.Log("Reset Charges ."); }
}

[System.Serializable]
public class BulletInfo
{
    [System.Serializable]
    public enum BulletType { Laser, Projectile }

    public BulletType type;

    public Vector2 direction = Vector2.right;
    public int maxTileLength = 0; // 0 = Infini ??

    public bool ignoreWalls = false;
    // public int power;    // pas pertinent si one shot...

    public BulletInfo(BulletType _type, Vector2 _direction, bool _ignoresWalls) // direction pertinent à init ?? -> Dépend direction Tir (player)
    {
        type = _type;
        direction = _direction;
        ignoreWalls = _ignoresWalls;
    }
}