using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BeatTimingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI beatText;
    [SerializeField] float beatTextShowTime = .3f;
    public void InstantiateText(Timing playerTiming)
    {
        beatText.rectTransform.localPosition = Vector3.zero;
        Sequence seqColor = DOTween.Sequence();
        switch (playerTiming)
        {
            case Timing.PERFECT:
                beatText.text = "Perfect !";
                beatText.color = new Color(0, 1, 0);
                break;
            case Timing.BEFORE:
                beatText.text = "Early";
                beatText.color = new Color(1, .4f, 0);
                break;
            case Timing.AFTER:
                beatText.text = "Late";
                beatText.color = new Color(1, .4f, 0);
                break;
            case Timing.MISS:
                beatText.text = "Missed";
                beatText.color = new Color(1, 0, 0);
                break;
            case Timing.NULL:

                break;

        }
        seqColor.Append(beatText.rectTransform.DOLocalMoveY(75, beatTextShowTime)).SetEase(Ease.OutExpo);
        seqColor.Insert(beatTextShowTime / 2, beatText.DOFade(0, beatTextShowTime / 2));
        seqColor.OnComplete(DestroyAfterAnim);
    }

    private void DestroyAfterAnim()
    {
        Destroy(this.gameObject);
    }
}
