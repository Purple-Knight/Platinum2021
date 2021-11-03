using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FeelGood : MonoBehaviour
{
    public float timeToDo;

    public bool changeScale;
    public float xPourcentScale;
    public float yPourcentScale;

    private Transform trans;
    private Transform refTrans;
    private RectTransform transR;
    private Transform refTransR;

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
        
    }
    

    void feelRectT()
    {
        if (changeScale)
        {
            Debug.Log(transR.localScale.x * (xPourcentScale / 100));
            Sequence sequence = DOTween.Sequence();

            sequence.Append(transR.transform.DOScale(new Vector2(transR.localScale.x * (xPourcentScale / 100), transR.localScale.y * (yPourcentScale / 100)), timeToDo / 2));
            sequence.Append(transR.transform.DOScale(new Vector2(refTransR.localScale.x, refTransR.localScale.y), timeToDo / 2));

            sequence.Play();
        }
    }


    private void OnDestroy()
    {
        RhythmManager.Instance.onMusicBeatDelegate -= feelGood;
    }
}
