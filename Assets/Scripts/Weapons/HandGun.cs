using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test HandGun", menuName = "Weapons/Test Handgun")]
public class HandGun : Weapon
{
    public HandGun() : base(null) { }

    public override void Use()
    {
        base.Use();
    }

    public override void Charge()
    {
        base.Charge();
    }

    public override void Fire()
    {
        base.Fire();
    }

    public override void MissedBeat()
    {
        base.MissedBeat();
    }
}
