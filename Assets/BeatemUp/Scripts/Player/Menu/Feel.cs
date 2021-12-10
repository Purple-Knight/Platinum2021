using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;

public class Feel : MonoBehaviour
{
    public bool launch;
    public bool onOff = true;
    public float timeToDo;

    //Position
    public bool changePos;
    public List<valueNeed> posNeed = new List<valueNeed>();
    public List<valueNeed> posNeedBack = new List<valueNeed>();

    //Scale
    public bool changeScale;
    public List<valueNeed> scaleNeed = new List<valueNeed>();
    public List<valueNeed> scaleNeedBack = new List<valueNeed>();

    private Transform trans;
    public Vector3 refTrans;
    public Vector3 refTransScale;

    private RectTransform transR;
    private Vector3 refTransR;
    private Vector3 refTransRScale;


    void Start()
    {

        if (GetComponent<RectTransform>())
        {
            transR = GetComponent<RectTransform>();
            refTransR = transR.localPosition;
            refTransRScale = transR.localScale;

        }
        else if (GetComponent<Transform>())
        {

            trans = GetComponent<Transform>();
            refTrans = trans.localPosition;
            refTransScale = trans.localScale;
        }
        else
        {
            Debug.LogError("Il manque un truc la....");
        }
    }


    private void Update()
    {
        if (launch)
        {
            launch = false;
            feelGood();
        }
    }

    void feelGood()
    {
        if (trans) feelTrans();
        else feelRectT();
    }

    void feelTrans()
    {
        if (onOff)
        {
            if (changePos)
            {
                var timeForAction = posNeed.Count;
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeed.Count; i++)
                {
                    sequence.Append(trans.transform.DOLocalMove(new Vector2(refTrans.x + posNeed[i].xPourcentScale, refTrans.y + posNeed[i].yPourcentScale), timeToDo / timeForAction));
                }

                sequence.Play();
            }


            if (changeScale)
            {
                var timeForAction = scaleNeed.Count;
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < scaleNeed.Count; i++)
                {
                    sequence.Append(trans.transform.DOScale(new Vector2(refTransScale.x * (scaleNeed[i].xPourcentScale / 100), refTransScale.y * (scaleNeed[i].yPourcentScale / 100)), timeToDo / timeForAction));
                }

                sequence.Play();
            }

        } else
        {
            if (changePos)
            {
                var timeForAction = posNeedBack.Count;
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeedBack.Count; i++)
                {
                    sequence.Append(trans.transform.DOLocalMove(new Vector2(refTrans.x + posNeedBack[i].xPourcentScale, refTrans.y + posNeedBack[i].yPourcentScale), timeToDo / timeForAction));
                }

                //sequence.Append(trans.transform.DOLocalMove(new Vector2(refTrans.localPosition.x, refTrans.localPosition.y), timeToDo / timeForAction));
                sequence.Play();
            }


            if (changeScale)
            {
                var timeForAction = scaleNeed.Count;
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < scaleNeedBack.Count; i++)
                {
                    sequence.Append(trans.transform.DOScale(new Vector2(refTransScale.x * (scaleNeedBack[i].xPourcentScale / 100), refTransScale.y * (scaleNeedBack[i].yPourcentScale / 100)), timeToDo / timeForAction));
                }

                sequence.Play();
            }
        }


        onOff = !onOff;
    }

    void feelRectT()
    {
        if (onOff)
        {
            //Debug.Log("On");
            var timeForAction = scaleNeed.Count + 1;
            if (changePos)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeed.Count; i++)
                {
                    sequence.Append(transR.transform.DOLocalMove(new Vector2(refTransR.x + posNeed[i].xPourcentScale, refTransR.y + posNeed[i].yPourcentScale), timeToDo / timeForAction));
                    //Debug.Log("test");
                }

                sequence.Play();
            }


            if (changeScale)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < scaleNeed.Count; i++)
                {
                    sequence.Append(transR.transform.DOScale(new Vector2(refTransRScale.x * (scaleNeed[i].xPourcentScale / 100), refTransRScale.y * (scaleNeed[i].yPourcentScale / 100)), timeToDo / timeForAction));
                }

                sequence.Play();
            }
        }
        else
        {
            
            //Debug.Log("Off");
            var timeForAction = scaleNeedBack.Count + 1;
            if (changePos)
            {
                Sequence sequence = DOTween.Sequence();

                for (int i = 0; i < posNeedBack.Count; i++)
                {
                    sequence.Append(transR.transform.DOLocalMove(new Vector2(refTransR.x + posNeedBack[i].xPourcentScale, refTransR.y + posNeedBack[i].yPourcentScale), timeToDo / timeForAction));
                }

                //sequence.Append(transR.transform.DOLocalMove(new Vector2(refTransR.localPosition.x, refTransR.localPosition.y), timeToDo / timeForAction));
                sequence.Play();
            }


            if (changeScale)
            {
                Sequence sequence = DOTween.Sequence();

                for(int i = 0; i < scaleNeedBack.Count; i++)
                {
                    sequence.Append(transR.transform.DOScale(new Vector2(refTransRScale.x * (scaleNeedBack[i].xPourcentScale / 100), refTransRScale.y * (scaleNeedBack[i].yPourcentScale / 100)), timeToDo / timeForAction));
                    
                }
                
                //sequence.Append(transR.transform.DOScale(new Vector2(refTransR.localScale.x, refTransR.localScale.y), timeToDo / timeForAction));
                sequence.Play();
            }
        }

        onOff = !onOff;
    }

}
