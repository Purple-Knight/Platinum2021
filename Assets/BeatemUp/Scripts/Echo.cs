using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    [HideInInspector] public Transform scale;
    [HideInInspector] public RectTransform scaleR;

    void Start()
    {
        StartCoroutine(tween());
    }

    IEnumerator tween()
    {
        if (GetComponent<RectTransform>())
        {
            GetComponent<Image>().DOFade(1, RhythmManager.Instance.beatDuration);
            var rect = GetComponent<RectTransform>();
            rect.DOScale(scaleR.localScale, RhythmManager.Instance.beatDuration);
        }
        else
        {
            GetComponent<SpriteRenderer>().DOFade(1, RhythmManager.Instance.beatDuration);
            transform.DOScale(scale.localScale / 2, RhythmManager.Instance.beatDuration);
        }

        yield return new WaitForSeconds(RhythmManager.Instance.beatDuration);

        DOTween.Kill(gameObject);
        Destroy(gameObject);
    }
}
