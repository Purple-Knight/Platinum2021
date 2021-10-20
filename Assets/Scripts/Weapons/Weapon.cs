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
    public int chargeBeats = 1;
    [SerializeField] private protected int chargeLevel = 0;

    //Reload
    public int waitBeforeReload;

    //Player Ref
    [SerializeField] internal PlayerMovement pMov;

    #region Structs...

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

        //Instantiate Bullet
        Bullet blt = Instantiate(bulletPrefab, pMov.transform.position, Quaternion.identity).GetComponent<Bullet>();
        blt.InitInfo(bulletInfo, Mathf.Sign(pMov.mvtHorizontal) * Vector2.right);

        ResetCharge();
    }

    public virtual void MissedBeat()
    {
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