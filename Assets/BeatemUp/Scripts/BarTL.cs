using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BarTL : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public float deleteTime;
    public Sprite endSprite;

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
        SpriteRenderer mySprite = null;
        Image mySpriteR = null;
        
        if (GetComponent<SpriteRenderer>())
        {
            mySprite = GetComponent<SpriteRenderer>();
            
            Vector3 maScale = transform.localScale;

            yield return new WaitForSeconds(deleteTime);
            mySprite.sprite = endSprite;

            speed = 0;
            direction = Vector2.zero;

            transform.DOScale(maScale * 2, time);
            mySprite.DOFade(0, time);
        }
        else
        {
            mySpriteR = GetComponent<Image>();
            
            Vector3 maScale = GetComponent<RectTransform>().localScale;

            yield return new WaitForSeconds(deleteTime);
            mySpriteR.sprite = endSprite;

            speed = 0;
            direction = Vector2.zero;

            GetComponent<RectTransform>().transform.DOScale(maScale * 2, time);
            mySpriteR.DOFade(0, time);
        }

        

        yield return new WaitForSeconds(time);

        DOTween.Kill(gameObject);

        Destroy(gameObject);
    }
}
