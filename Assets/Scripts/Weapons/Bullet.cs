using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour     // Script on bullet GameObject, instantiated on Weapon Fire
{
    public BulletInfo info;
    public LayerMask hitLayer;

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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, info.direction, 50, hitLayer);
        if(hit.collider != null)
        {
            Debug.Log("HIT : " + hit.transform.name + " ; " + info.direction.x);
            Debug.DrawLine(transform.position, hit.point, Color.yellow, .5f);
        }
        else Debug.Log("NOTHING HIT.");

    }

    private void DestroyBullet()
    {

    }
}
