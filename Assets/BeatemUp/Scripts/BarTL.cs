using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarTL : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public float deleteTime;

    private Timeline tl;


    

    public void Start()
    {
        tl = Timeline.Instance;
        StopAllCoroutines();
        StartCoroutine(deletionNote());
    }



    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }


    public IEnumerator deletionNote()
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

        tl.allBar.Remove(this);

        DOTween.Kill(gameObject);

        Destroy(gameObject);
    }
}
