using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour     // Script on bullet GameObject, instantiated on Weapon Fire
{
    public BulletInfo info;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitInfo(BulletInfo _Info, Vector2 direction)
    {
        info = _Info;
        info.direction = direction;

        switch (info.type)
        {
            case BulletInfo.BulletType.Laser:
                //LineRenderer
                gameObject.AddComponent<LineRenderer>();
                break;
            case BulletInfo.BulletType.Projectile:
                //SpriteRender
                gameObject.AddComponent<SpriteRenderer>();
                break;
            default:
                break;
        }
    }

    private void DestroyBullet()
    {

    }
}
