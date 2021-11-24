using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Timeline : MonoBehaviour
{
    public static Timeline Instance { get { return _instance; } }
    private static Timeline _instance;

    [Header("Object")]
    public GameObject endTimeline;
    public GameObject bar;
    public GameObject echo;

    [Header("Variables")]
    public float beatToReach;
    public float multiplicatorSpeed;

    bool canBegin;
    int actualBeat;
    float numberOfBeat;
    public int newNOB;

    [Header("RythmChange")]
    public List<BarTL> allBar = new List<BarTL>();
    public bool actu;
    public bool up;
    


    [Header("Debug")]
    public bool guiDebug;
    [SerializeField] private Rect guiDebugArea = new Rect(0, 20, 150, 150);
    bool beatBool;


    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        RhythmManager.Instance.onMusicBeatDelegate += SendBar;
        RhythmManager.Instance.InstantiateBeat.AddListener(SendBar);
    }

    private void Update()
    {
        
    }

    public void SendBar()
    {
        var booltest = false;
        if (actu)
        {
            actu = false;
            actualiseAllBar(up);
            booltest = true;
        }

        if (!booltest || (booltest && up)) {
            if (canBegin && actualBeat < (numberOfBeat))
            {
                var lastBar = Instantiate(bar, transform.position, transform.rotation);
                lastBar.name += actualBeat;

                var direction = endTimeline.transform.position - transform.position;
                var distance = Vector2.Distance(transform.position, endTimeline.transform.position);

                var time = beatToReach * RhythmManager.Instance.beatDuration;

                var speed = (distance / time) * multiplicatorSpeed;

                var barScript = lastBar.GetComponent<BarTL>();
                barScript.direction = direction.normalized;
                barScript.speed = speed;
                barScript.deleteTime = time;

                allBar.Add(barScript);

                actualBeat++;

                createEcho();
            }
            else
            {
                canBegin = true;
                numberOfBeat = RhythmManager.Instance.numberOfBeat;
            }
        }
    }

    public void actualiseAllBar(bool plus)
    {
        //List<BarTL> listla = new List<BarTL>();

        for (int i = 0; i < allBar.Count; i++)
        {
            var lastBar = allBar[i];

            //if (plus) listla.Add(lastBar);
                
                var direction = endTimeline.transform.position - lastBar.transform.position;
                var distance = Vector2.Distance(lastBar.transform.position, endTimeline.transform.position);

                var time = (RhythmManager.Instance.beatDuration) * (allBar.Count - (allBar.Count - i));

                var speed = (distance / time) * multiplicatorSpeed;

                var barScript = lastBar.GetComponent<BarTL>();
                barScript.direction = direction.normalized;
                barScript.speed = speed;
                barScript.deleteTime = time;
                barScript.Start();
        }

        /*if (listla.Count != 0)
        {
            foreach (var item in listla)
            {
                allBar.Remove(item);
            }
        }*/
    }

    /*public void actualiseAllBar(bool _plus)
    {
        if (!_plus) {
            for (int i = 0; i < allBar.Count; i++)
            {
                var lastBar = allBar[i];

                if (i > beatToReach - 1)
                {
                    Destroy(lastBar.gameObject);
                }
                else
                {
                    var direction = endTimeline.transform.position - lastBar.transform.position;
                    var distance = Vector2.Distance(lastBar.transform.position, endTimeline.transform.position);

                    var time = (RhythmManager.Instance.beatDuration) * (allBar.Count - (allBar.Count - i));

                    var speed = (distance / time) * multiplicatorSpeed;

                    var barScript = lastBar.GetComponent<BarTL>();
                    barScript.direction = direction.normalized;
                    barScript.speed = speed;
                    barScript.deleteTime = time;
                    barScript.Start();

                }
            }


        }
        else
        {
            for (int i = allBar.Count; i <= beatToReach; i++)
            {
                SendBar();
            }

            for (int i = 0; i < allBar.Count; i++)
            {
                var lastBar = allBar[i];

                    var direction = endTimeline.transform.position - lastBar.transform.position;
                    var distance = Vector2.Distance(lastBar.transform.position, endTimeline.transform.position);

                    var time = (RhythmManager.Instance.beatDuration) * (allBar.Count - (allBar.Count - i));

                    var speed = (distance / time) * multiplicatorSpeed;

                    var barScript = lastBar.GetComponent<BarTL>();
                    barScript.direction = direction.normalized;
                    barScript.speed = speed;
                    barScript.deleteTime = time;
                    barScript.Start();

            }
        }
    }*/


    void createEcho()
    {
        var echoO = Instantiate(echo, transform.position, transform.rotation);
        var colora = gameObject.GetComponent<SpriteRenderer>();

        echoO.GetComponent<SpriteRenderer>().color = new Color(colora.color.r, colora.color.g, colora.color.b, 0);
        echoO.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        echoO.transform.localScale = transform.localScale * 2;

        echoO.GetComponent<Echo>().scale = transform;
    }




    private void OnGUI()
    {
        if (!guiDebug) return;

        GUILayout.BeginArea(guiDebugArea);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Beat"))
        {
            beatBool = !beatBool;
        }
        
        GUILayout.EndHorizontal();

        if (beatBool)
        {
            GUILayout.TextField("Actual Beat : " + actualBeat + "\nBeat in Total : " + numberOfBeat);
        }

        GUILayout.EndArea();
    }
}
