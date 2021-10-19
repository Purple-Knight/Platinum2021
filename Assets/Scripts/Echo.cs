using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Echo : MonoBehaviour
{
    [HideInInspector]
    public Transform scale;

    void Start()
    {
        StartCoroutine(tween());
    }

    IEnumerator tween()
    {
        GetComponent<SpriteRenderer>().DOFade(1, RhythmManager.Instance.beatDuration);
        transform.DOScale(scale.localScale, RhythmManager.Instance.beatDuration);

        yield return new WaitForSeconds(RhythmManager.Instance.beatDuration);

        DOTween.Kill(gameObject);
        Destroy(gameObject);
    }
}
