using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeelGood : MonoBehaviour
{

    public bool playOnAwake;
    public float timeToDo;

    //Position
    public bool changePos;
    public List<valueNeed> posNeed = new List<valueNeed>();

    //Scale
    public bool changeScale;
    public List<valueNeed> scaleNeed = new List<valueNeed>();
   

    private Transform trans;
    private Transform refTrans;
    private RectTransform transR;
    private RectTransform refTransR;

    void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += feelGood;
        RhythmManager.Instance.InstantiateBeat.AddListener(feelGood);

        if (GetComponent<RectTransform>())
        {
            transR = GetComponent<RectTransform>();
            refTransR = transR;

        }
        else if (GetComponent<Transform>()) { 

            trans = GetComponent<Transform>();
            refTrans = trans;
        }
        else
        {
            Debug.LogError("Il manque un truc la....");
        }
    }

    void feelGood()
    {
        if (trans) feelTrans();
        else feelRectT();
    }


    void feelTrans()
    {
        if (playOnAwake)
        {
            var timeForAction = scaleNeed.Count + 1;
            if (changePos)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeed.Count; i++)
                {
                    sequence.Append(trans.transform.DOLocalMove(new Vector2(trans.localPosition.x + posNeed[i].xPourcentScale, trans.localPosition.y + posNeed[i].yPourcentScale), timeToDo / timeForAction));
                }

                sequence.Append(trans.transform.DOLocalMove(new Vector2(refTrans.position.x, refTrans.position.y), timeToDo / timeForAction));
                sequence.Play();
            }


            if (changeScale)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < scaleNeed.Count; i++)
                {
                    sequence.Append(trans.transform.DOScale(new Vector2(trans.localScale.x * (scaleNeed[i].xPourcentScale / 100), trans.localScale.y * (scaleNeed[i].yPourcentScale / 100)), timeToDo / timeForAction));
                }

                sequence.Append(trans.transform.DOScale(new Vector2(refTrans.localScale.x, refTrans.localScale.y), timeToDo / timeForAction));
                sequence.Play();
            }
        }
    }
    

    void feelRectT()
    {
        if (playOnAwake)
        {
            var timeForAction = scaleNeed.Count + 1;
            if (changePos)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeed.Count; i++)
                {
                    sequence.Append(transR.transform.DOLocalMove(new Vector2(transR.localPosition.x + posNeed[i].xPourcentScale, transR.localPosition.y + posNeed[i].yPourcentScale), timeToDo / timeForAction));
                }

                sequence.Append(transR.transform.DOLocalMove(new Vector2(refTransR.localPosition.x, refTransR.localPosition.y), timeToDo / timeForAction));
                sequence.Play();
            }


            if (changeScale)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < scaleNeed.Count; i++)
                {
                    sequence.Append(transR.transform.DOScale(new Vector2(transR.localScale.x * (scaleNeed[i].xPourcentScale / 100), transR.localScale.y * (scaleNeed[i].yPourcentScale / 100)), timeToDo / timeForAction));
                }

                sequence.Append(transR.transform.DOScale(new Vector2(refTransR.localScale.x, refTransR.localScale.y), timeToDo / timeForAction));
                sequence.Play();
            }
        }

    }


    private void OnDestroy()
    {
        RhythmManager.Instance.onMusicBeatDelegate -= feelGood;
        DOTween.Kill(gameObject);
    }
}


[System.Serializable]
public struct valueNeed
{
    public float xPourcentScale;
    public float yPourcentScale;

}


