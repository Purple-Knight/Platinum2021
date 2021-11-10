using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarTL : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
     public float deleteTime;


    

    void Start()
    {
        StartCoroutine(deletionNote());
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }


    IEnumerator deletionNote()
    {
        var time = RhythmManager.Instance.beatDuration;
        var mySprite = GetComponent<SpriteRenderer>();
        Vector3 maScale = transform.localScale;

        yield return new WaitForSeconds(deleteTime);

        speed = 0;
        direction = Vector2.zero;

        transform.DOScale(maScale * 3, time);
        mySprite.DOFade(0, time);

        yield return new WaitForSeconds(time);

        DOTween.Kill(gameObject);

        Destroy(gameObject);
    }
}
