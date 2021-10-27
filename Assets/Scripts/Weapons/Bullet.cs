using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour     // Script on bullet GameObject, instantiated on Weapon Fire
{
    public BulletInfo info;
    public LayerMask hitLayer;

    public AnimationCurve laserWidth;

    BoxCollider2D col;

    public Material mat;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();

        Destroy(gameObject, .3f);
    }

    public void InitInfo(BulletInfo _Info, Vector2 direction)
    {
        info = _Info;
        info.direction = direction;
        Vector2 spawnPos = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);

        RaycastHit2D hit = Physics2D.Raycast(spawnPos, info.direction, 50, hitLayer);
        if (hit.collider != null)
        {
            //Debug.Log("HIT : " + hit.transform.name + " ; " + info.direction.x);
            Debug.DrawLine(spawnPos, hit.point, Color.yellow, .5f);
        }
        else
        {
            Debug.LogWarning("NOTHING HIT.");
            return;
        }

        switch (info.type)
        {
            case BulletInfo.BulletType.Laser:
                InitLaser(spawnPos, hit.point, direction);
                break;
            case BulletInfo.BulletType.Projectile:
                //SpriteRender
                gameObject.AddComponent<SpriteRenderer>();
                break;
            default:
                break;
        }

    }

    private void InitLaser(Vector2 startPos, Vector2 endPos, Vector2 direction)
    {
        //LineRenderer
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        lr.SetPositions(new Vector3[] { startPos, endPos });
        lr.widthCurve = laserWidth;
        lr.materials = new Material[] { mat };

       RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, endPos.x - startPos.x, LayerMask.GetMask("Player"));   // Cast Players hit  (add Player layerMask)
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider != null)
            {
                Debug.Log("hit player :" + hit.collider.name);
                IDamageable hitObject;
                if(hit.collider.gameObject.TryGetComponent<IDamageable>(out hitObject))
                    hitObject.OnHit();
                else
                    Debug.LogError("NO Interface Found");
            }
        }
    }

}
