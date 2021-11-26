using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour     // Script on bullet GameObject, instantiated on Weapon Fire
{
    public BulletInfo info;
    public LayerMask hitLayer;

    public AnimationCurve laserWidth;

    public Material mat;

    private void Start()
    {
        Destroy(gameObject, .3f);
    }

    public void InitInfo(BulletInfo _Info, in Vector2 direction)
    {
        info = _Info;
        float length = (info.length > 0) ? info.length : 50;
        Vector2 spawnPos = new Vector2(transform.position.x + .5f * direction.x, transform.position.y + .5f * direction.y);
        Vector2 endPos = spawnPos;

        RaycastHit2D hit = Physics2D.Raycast(spawnPos, direction, length, hitLayer);
        if (hit.collider != null)
        {
            endPos = hit.point;
        }
        else
        {
            endPos = new Vector2(spawnPos.x + length * direction.x, spawnPos.y + length * direction.y);
        }

        InitLaser(spawnPos, endPos, direction);
    }

    private void InitLaser(Vector2 startPos, Vector2 endPos, Vector2 direction)
    {
        //LineRenderer
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();

        lr.SetPositions(new Vector3[] { startPos, endPos });
        lr.widthCurve = laserWidth;
        lr.materials = new Material[] { mat };

       RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, Vector2.Distance(startPos, endPos), LayerMask.GetMask("Player"));   // Cast Players hit  (add Player layerMask)
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider != null)
            {
                //Debug.Log("hit player :" + hit.collider.name);
                IDamageable hitObject;
                if(hit.collider.gameObject.TryGetComponent<IDamageable>(out hitObject))
                    hitObject.OnHit();
                else
                    Debug.LogError("NO Interface Found");
            }
        }
    }
}
