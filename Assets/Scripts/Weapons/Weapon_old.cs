using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Weapon", menuName = "Weapon")]
[System.Serializable]
public class Weapon_old : ScriptableObject
{
    //WeaponType
    public BulletInfo bulletInfo;

    // Bullet
    public GameObject bulletPrefab;

    // Charge
    [SerializeField] private bool noChargeTime;
    public int chargeBeats = 1;             // charge Time needed
    [SerializeField] private protected int chargeLevel = 0;  // current Charge

    //Reload
    [SerializeField] private bool noReload;
    public int waitBeforeReload;
    private int currentReloadBeats;

    //Player Ref
    internal Vector2 lastDirection;
    [SerializeField] internal Transform pMov;

    #region Structs...

    #endregion

    public virtual void Use(bool triggerDown)
    {
        //Debug.Log("used");
        if(triggerDown && chargeBeats == 0 && chargeLevel == 0) // Button Down
        {
            Fire();
            //chargeLevel++;
            //Debug.Log("Try Shoot...");
        }else if(triggerDown) // Button Down
        {
            Charge();
            //Debug.Log("Try Shoot...");
        }
        else    // Button Up
        {
            if (chargeLevel == chargeBeats)
                Fire();
            else if(chargeLevel > 0)
                MissedBeat();
        }
    }

    public virtual void Charge()
    {
        chargeLevel++;
        //Debug.Log("------------------Charge Level = " + chargeLevel);
    }

    public virtual void Fire()
    {
        //Debug.Log("FIRE! " + chargeLevel + " charges");

        //Instantiate Bullet
        //Debug.Log(lastDirection);
        Bullet blt = Instantiate(bulletPrefab, pMov.position, Quaternion.identity).GetComponent<Bullet>();
        blt.InitInfo(bulletInfo, lastDirection);

        ResetCharge();
    }

    public virtual void MissedBeat()
    {
        //Debug.Log("missed");

        ResetCharge();
    }

    private protected void ResetCharge() { chargeLevel = 0; }

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