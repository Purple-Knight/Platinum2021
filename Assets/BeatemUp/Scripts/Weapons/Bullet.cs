using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour     // Script on bullet GameObject, instantiated on Weapon Fire
{
    public LayerMask hitLayer;

    [Range(.55f, .95f)]
    public float tilePositionOffset = .7f;
    public AnimationCurve laserWidth;

    public Material mat;
    Vector2 endPos;
    Vector2 startPos;
    float timer = 0;
    [SerializeField] float speed = 10;
    LineRenderer lr;

    private void Awake()
    {
        Destroy(gameObject, .3f);
        lr = gameObject.AddComponent<LineRenderer>();
    }
    private void Update()
    {
        timer += Time.deltaTime * speed;
        Vector2 i = Vector2.Lerp(startPos, endPos, timer);
        lr.SetPositions(new Vector3[] { startPos,i});
    }

    public void InitInfo(BulletInfo _Info, in Vector2 direction, PlayerManager _playerManager)
    {
        float length = (_Info.length > 0) ? _Info.length : 50;
        startPos = new Vector2(transform.position.x + (_playerManager.GridSize.x * tilePositionOffset) * direction.x, transform.position.y + (_playerManager.GridSize.y * tilePositionOffset) * direction.y);
        //Vector2 endPos;
        float grid = direction.x == 0 ? _playerManager.GridSize.y : _playerManager.GridSize.x;
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, (length - tilePositionOffset) * grid, hitLayer);
        if (hit.collider != null)
        {
            endPos = hit.point;
        }
        else
        {
            endPos = new Vector2(startPos.x + (length - tilePositionOffset) *_playerManager.GridSize.x * direction.x, startPos.y + (length - tilePositionOffset) * _playerManager.GridSize.y * direction.y);
        }

        float widthFactor = (direction.x == 0) ? _playerManager.GridSize.x : _playerManager.GridSize.y;
        InitLaser(direction, widthFactor);
    }

    private void InitLaser( Vector2 direction, float widthFactor)
    {
        //LineRenderer
        

        lr.SetPositions(new Vector3[] { startPos, startPos });
        lr.widthCurve = laserWidth;
        lr.widthCurve = new AnimationCurve(new Keyframe(0, laserWidth.Evaluate(0) * widthFactor), new Keyframe(1, laserWidth.Evaluate(1) * widthFactor));
        lr.materials = new Material[] { mat };
        //lr.sortingOrder = 1;

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
