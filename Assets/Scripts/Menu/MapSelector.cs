using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapSelector : MonoBehaviour
{

    // Instance -------------------------------------
    private static MapSelector _instance = null;
    public static MapSelector Instance { get => _instance; }



    [Header("List")]
    public List<Transform> refPoints = new List<Transform>();
    public List<GameObject> GOList = new List<GameObject>();

    List<GameObject> SubPoints = new List<GameObject>();
    List<Transform> refSub = new List<Transform>();


    [Header("Prefab")]
    public GameObject prefabSub;

    [Header("Variables")]
    [SerializeField] float animTime;

    //Float
    float distance = 0;
    float quartDistance = 0;
    float subDistance = 0;

    public void Awake()
    {
        _instance = this;

    }

    private void Start()
    {
        calculateDistance();

        createPoints();

        setGameObject();
    }

    void calculateDistance() // Distance + SubDistance 
    {
        for (int i = 0; i < refPoints.Count; i++)
        {
            if (i < refPoints.Count - 1)
            {
                distance +=Vector3.Distance(refPoints[i].transform.position, refPoints[i + 1].transform.position);
            }
            else
            {
                distance += Vector3.Distance(refPoints[i].transform.position, refPoints[0].transform.position);
            }
        }

        subDistance = distance / GOList.Count;
        quartDistance = distance / 4;

    }

    void createPoints()
    {
        refSub.Add(refPoints[0]);


        var test = Instantiate(prefabSub, refPoints[0].transform.position, refPoints[0].transform.rotation, transform);
        test.name = "point " + 0;
        SubPoints.Add(test);
        refSub.Add(test.transform);

        for (int i = 1; i < GOList.Count; i++)
        {
            float pourcent = (subDistance * i)/ distance;

            if(pourcent <= 0.25)
            {
                findGoodPosition(pourcent, 0f, refPoints[0].transform.position, refPoints[1].transform.position, i);
            }
            else if (pourcent <= 0.50)
            {
                if (!refSub.Contains(refPoints[1])) refSub.Add(refPoints[1]);
                findGoodPosition(pourcent, 0.25f, refPoints[1].transform.position, refPoints[2].transform.position, i);

            }
            else if (pourcent <= 0.75)
            {
                if (!refSub.Contains(refPoints[2])) refSub.Add(refPoints[2]);
                findGoodPosition(pourcent, 0.5f, refPoints[2].transform.position, refPoints[3].transform.position, i);
            }
            else
            {
                if (!refSub.Contains(refPoints[3])) refSub.Add(refPoints[3]);
                findGoodPosition(pourcent, 0.75f, refPoints[3].transform.position, refPoints[0].transform.position, i);
            }
        }

        if (!refSub.Contains(refPoints[1])) refSub.Add(refPoints[1]);
        if (!refSub.Contains(refPoints[2])) refSub.Add(refPoints[2]);
        if (!refSub.Contains(refPoints[3])) refSub.Add(refPoints[3]);
    }


    void findGoodPosition(float pourcent, float minBound,Vector3 position1, Vector3 position2, int number)
    {
        var aPlusb = (position2 - position1);
        var ab = Vector3.Distance(position1, position2);
        var acPourcent = (pourcent - minBound);
        var ac = (acPourcent * ab) / 0.25;
        var acDANSab = ac / ab;
        var finale = (position1 + ((float)acDANSab) * (aPlusb));


        var test = Instantiate(prefabSub, transform.position, gameObject.transform.rotation, gameObject.transform);
        test.transform.position = finale;
        test.name = "point " + number;
        SubPoints.Add(test);
        refSub.Add(test.transform);
    }


    void setGameObject()
    {
        foreach (var item in GOList)
        {
            var id = GOList.IndexOf(item);

            item.transform.position = SubPoints[id].transform.position;
        }

        moveToGO();
    }

    void moveToGO()
    {
        foreach (var item in GOList)
        {
            var id = GOList.IndexOf(item);

            item.transform.DOMove(SubPoints[id].transform.position, animTime);

            item.transform.SetSiblingIndex(Mathf.Abs(id + 1 - SubPoints.Count));




            Color colorProv = item.GetComponent<Image>().color;
            float m_Hue;
            float m_Saturation;
            float m_Value;
            Color.RGBToHSV(colorProv, out m_Hue, out m_Saturation, out m_Value);

            if (id == 0) item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 1);
            else item.GetComponent<Image>().color = Color.HSVToRGB(m_Hue, m_Saturation, 0.5f);
        }


        for (int i = GOList.Count - 1; i > GOList.Count / 2; i--)
        {
            GOList[i].transform.SetSiblingIndex(Mathf.Abs(i + (SubPoints.Count / 2) - SubPoints.Count));
        }
    }





    public void upValue()
    {
        List<GameObject> laCopie = new List<GameObject>();

        foreach (var item in GOList)
        {
            laCopie.Add(item);
        }

        for (int i = 0; i < GOList.Count; i++)
        {
            if (i != 0) GOList[i] = laCopie[i - 1];
            else GOList[i] = laCopie[GOList.Count - 1];
        }

        moveToGO();
    }

    public void downValue()
    {
        List<GameObject> laCopie = new List<GameObject>();

        foreach (var item in GOList)
        {
            laCopie.Add(item);
        }

        for (int i = 0; i < GOList.Count; i++)
        {
            if (i != GOList.Count - 1) GOList[i] = laCopie[i + 1];
            else GOList[i] = laCopie[0];
        }

        moveToGO();
    }
}
