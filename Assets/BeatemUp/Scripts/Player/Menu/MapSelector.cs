using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapSelector : MonoBehaviour
{

    /*// Instance -------------------------------------
    private static RotateMenu _instance = null;
    public static RotateMenu Instance { get => _instance; }*/

    public delegate void onMenuMove();
    public onMenuMove callBack;

    public ROTATING_STATE rotatingState;

    [Header("List")]
    public List<Transform> refPoints = new List<Transform>();
    public List<GameObject> GoList = new List<GameObject>();
    List<GameObject> copieGoList;

    List<GameObject> SubPoints = new List<GameObject>();


    [Header("Prefab")]
    public GameObject prefabSub;

    [Header("Variables")]
    [SerializeField] float animTime;
    [SerializeField] int mainPoint;

    [SerializeField] List<Vector3> sizeSelected = new List<Vector3>();
    [SerializeField] private Vector3 sizeBack;

    public bool useAlpha;
    public float alpha = 1;

    //Float
    float distance = 0;
    float refDistance = 0;
    float subDistance = 0;
    public int actualID = 0;



    public enum ROTATING_STATE
    {
        LOOP,
        LINE,
        LINEOFFSET,
    }

    public void Awake()
    {
        //_instance = this;

    }

    private void Start()
    {
        copyWValue();

        var copyValue = actualID;
        mainPoint = Mathf.Clamp(mainPoint, 0, copieGoList.Count - 1);

        calculateDistance();

        createPoints();

        setGameObject();

        actualID = copyValue;

        if (useAlpha)
        {
            upValue();
            downValue();
        }
    }

    void copyWValue()
    {
        List<GameObject> forCopy = new List<GameObject>();



        for (int i = actualID; i < GoList.Count; i++)
        {
            forCopy.Add(GoList[i]);
        }
        for (int i = 0; i < actualID; i++)
        {
            forCopy.Add(GoList[i]);
        }

        copieGoList = new List<GameObject>(forCopy);

        if (rotatingState == ROTATING_STATE.LINEOFFSET)
        {
            for (int i = 0; i < forCopy.Count - 1; i++)
            {
                if (i <= copieGoList.Count)
                {
                    var pref = Instantiate(prefabSub, transform.position, transform.rotation);
                    copieGoList.Add(pref);
                }

            }
        }
    }

    void calculateDistance() // Distance + SubDistance 
    {
        for (int i = 0; i < refPoints.Count; i++)
        {
            if (i < refPoints.Count - 1)
            {
                distance += Vector3.Distance(refPoints[i].transform.position, refPoints[i + 1].transform.position);
            }
            else
            {
                if (rotatingState == ROTATING_STATE.LOOP) distance += Vector3.Distance(refPoints[i].transform.position, refPoints[0].transform.position);
            }
        }

        subDistance = distance / copieGoList.Count;

        if (rotatingState == ROTATING_STATE.LOOP) refDistance = 1 / (float)refPoints.Count;
        else if (rotatingState == ROTATING_STATE.LINE) refDistance = 1 / (float)(refPoints.Count - 0.5f);
        else if (rotatingState == ROTATING_STATE.LINEOFFSET) refDistance = 1 / (float)refPoints.Count;
    }

    void createPoints()
    {
        var test = Instantiate(prefabSub, refPoints[0].transform.position, refPoints[0].transform.rotation, transform);
        test.name = "point " + 0;
        SubPoints.Add(test);

        for (int i = 1; i < copieGoList.Count; i++)
        {
            float pourcent = (subDistance * i) / distance;

            for (int j = 1; j < refPoints.Count; j++)
            {
                //Debug.Log(i + " " + pourcent + " " + (1 / (float)refPoints.Count) * j);

                if (rotatingState == ROTATING_STATE.LINEOFFSET)
                {
                    findGoodPosition(pourcent, refDistance * (j - 1), refPoints[j - 1].transform.position, refPoints[j].transform.position, i);
                    break;
                }

                if (pourcent <= (refDistance * j) || (rotatingState == ROTATING_STATE.LINE && j == refPoints.Count - 1))
                {

                    findGoodPosition(pourcent, refDistance * (j - 1), refPoints[j - 1].transform.position, refPoints[j].transform.position, i);
                    break;

                }
                else if (rotatingState != ROTATING_STATE.LINE && j == refPoints.Count - 1 && pourcent > refDistance * j)
                {
                    findGoodPosition(pourcent, refDistance * j, refPoints[j].transform.position, refPoints[0].transform.position, i);
                    break;
                }
            }
        }
    }


    void findGoodPosition(float pourcent, float minBound, Vector3 position1, Vector3 position2, int number)
    {
        var aPlusb = (position2 - position1);
        var ab = Vector3.Distance(position1, position2);
        var acPourcent = (pourcent - minBound);
        var ac = (acPourcent * ab) / refDistance;
        var acDANSab = ac / ab;
        var finale = (position1 + ((float)acDANSab) * (aPlusb));


        var test = Instantiate(prefabSub, transform.position, gameObject.transform.rotation, gameObject.transform);
        test.transform.position = finale;
        test.name = "point " + number;
        SubPoints.Add(test);
    }

    void setGameObject()
    {
        foreach (var item in copieGoList)
        {
            var id = copieGoList.IndexOf(item);

            if (id == 0) id = mainPoint;
            else id += mainPoint;

            while (id > copieGoList.Count - 1)
            {
                id -= copieGoList.Count;
            }

            item.transform.position = SubPoints[id].transform.position;
        }

        moveToGO();
        /*downValue();
        upValue();*/

    }


    public void moveToGO()
    {
        foreach (var item in copieGoList)
        {
            var id = copieGoList.IndexOf(item);

            id += mainPoint;

            while (id > copieGoList.Count - 1)
            {
                id -= copieGoList.Count;
            }


            item.transform.SetSiblingIndex(Mathf.Abs(id + 1 - SubPoints.Count));

            item.transform.DOKill();

            Color colorProv = item.GetComponent<Image>().color;
            float m_Hue;
            float m_Saturation;
            float m_Value;
            Color.RGBToHSV(colorProv, out m_Hue, out m_Saturation, out m_Value);

            if (id == mainPoint)
            {
                item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 1);
                var sequence = DOTween.Sequence();

                for (int i = 0; i < sizeSelected.Count; i++)
                {
                    sequence.Append(item.transform.DOScale(sizeSelected[i], animTime / sizeSelected.Count));
                }

                sequence.Play();
            }
            else
            {
                item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 0.5f);
                item.transform.DOScale(new Vector3(sizeBack.x, sizeBack.y, sizeBack.z), animTime);
            }
        }


        for (int i = copieGoList.Count - 1; i > copieGoList.Count / 2; i--)
        {
            copieGoList[i].transform.SetSiblingIndex(Mathf.Abs(i + (SubPoints.Count / 2) - SubPoints.Count));
        }
    }

    void moveToGO(bool up)
    {
        foreach (var item in copieGoList)
        {
            var id = copieGoList.IndexOf(item);

            id += mainPoint;

            while (id > copieGoList.Count - 1)
            {
                id -= copieGoList.Count;
            }


            //item.transform.SetSiblingIndex(Mathf.Abs(id + 1 - SubPoints.Count));

            item.transform.DOKill();

            if (rotatingState == ROTATING_STATE.LINE)
            {
                if (!up && id == SubPoints.Count - 1) item.transform.position = SubPoints[SubPoints.Count - 1].transform.position;

                else if (up && id == 0) item.transform.position = SubPoints[0].transform.position;

                else item.transform.DOMove(SubPoints[id].transform.position, animTime);

            }
            else item.transform.DOMove(SubPoints[id].transform.position, animTime);


            Color colorProv = item.GetComponent<Image>().color;
            float m_Hue;
            float m_Saturation;
            float m_Value;
            Color.RGBToHSV(colorProv, out m_Hue, out m_Saturation, out m_Value);

            if (id == mainPoint)
            {
                
                item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 1);

                var sequence = DOTween.Sequence();

                for (int i = 0; i < sizeSelected.Count; i++)
                {
                    sequence.Append(item.transform.DOScale(sizeSelected[i], animTime / sizeSelected.Count));
                }

                sequence.Play();
            }
            else
            {
                if(useAlpha) item.GetComponent<Image>().color = new Color(colorProv.r, colorProv.g, colorProv.b, alpha);
                else item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 0.5f);
                item.transform.DOScale(new Vector3(1f, 1f, 1f), animTime);
            }
        }

        StartCoroutine(timeToChangeOrder());

        /*for (int i = copieGoList.Count / 2; i< copieGoList.Count - 1; i++)
        {
            copieGoList[i].transform.SetSiblingIndex(Mathf.Abs(i + (SubPoints.Count / 2) - SubPoints.Count));
        }*/

        changeValueNumber(up);
    }





    public void upValue()
    {
        if (rotatingState != ROTATING_STATE.LINEOFFSET || actualID != 0)
        {

            List<GameObject> laCopie = new List<GameObject>();

            foreach (var item in copieGoList)
            {
                laCopie.Add(item);
            }

            for (int i = 0; i < copieGoList.Count; i++)
            {
                if (i != 0) copieGoList[i] = laCopie[i - 1];
                else copieGoList[i] = laCopie[copieGoList.Count - 1];
            }

            moveToGO(true);
        }
    }

    public void downValue()
    {
        if (rotatingState != ROTATING_STATE.LINEOFFSET || actualID != GoList.Count - 1)
        {

            List<GameObject> laCopie = new List<GameObject>();

            foreach (var item in copieGoList)
            {
                laCopie.Add(item);
            }

            for (int i = 0; i < copieGoList.Count; i++)
            {
                if (i != copieGoList.Count - 1) copieGoList[i] = laCopie[i + 1];
                else copieGoList[i] = laCopie[0];
            }

            moveToGO(false);
        }
    }


    void changeValueNumber(bool up)
    {
        switch (up)
        {
            case true:
                if (actualID == 0)
                {
                    actualID = copieGoList.Count - 1;
                }
                else
                {
                    actualID--;
                }
                break;
            case false:
                if (actualID == copieGoList.Count - 1)
                {
                    actualID = 0;
                }
                else
                {
                    actualID++;
                }
                break;
        }

        callBack?.Invoke();

    }

    IEnumerator timeToChangeOrder()
    {
        yield return new WaitForSeconds(animTime / 8);

        foreach (var item in copieGoList)
        {
            var id = copieGoList.IndexOf(item);

            id += mainPoint;

            while (id > copieGoList.Count - 1)
            {
                id -= copieGoList.Count;
            }

            item.transform.SetSiblingIndex(Mathf.Abs(id + 1 - SubPoints.Count));

        }
        for (int i = copieGoList.Count / 2; i < copieGoList.Count - 1; i++)
        {
            copieGoList[i].transform.SetSiblingIndex(Mathf.Abs(i + (SubPoints.Count / 2) - SubPoints.Count));
        }
    }
}