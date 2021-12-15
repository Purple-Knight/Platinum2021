using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

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

    public void SendBar()
    {
        if (canBegin && actualBeat < (numberOfBeat))
        {
            var lastBar = Instantiate(bar, transform.position, transform.rotation);

            lastBar.transform.parent = transform;

            var direction = endTimeline.transform.position - transform.position;
            var distance = Vector2.Distance(transform.position, endTimeline.transform.position);

            var time = beatToReach * RhythmManager.Instance.beatDuration;
            
            var speed = (distance / time) * multiplicatorSpeed;

            var barScript = lastBar.GetComponent<BarTL>();
            barScript.Init(endTimeline.transform.position, direction.normalized, speed, time);

            actualBeat++;

            createEcho();
        }
        else
        {
            canBegin = true;
            numberOfBeat = RhythmManager.Instance.numberOfBeat;
        }
    }


    void createEcho()
    {
        var echoO = Instantiate(echo, transform.position, transform.rotation);
        if (echoO.GetComponent<SpriteRenderer>())
        {
            var colora = gameObject.GetComponent<SpriteRenderer>();

            echoO.GetComponent<SpriteRenderer>().color = new Color(colora.color.r, colora.color.g, colora.color.b, 0);
            echoO.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

            echoO.transform.localScale = transform.localScale * .7f;

            echoO.GetComponent<Echo>().scale = transform;
        }
        else
        {
            echoO.transform.parent = transform;
            var colora = gameObject.GetComponent<Image>();

            echoO.GetComponent<Image>().color = new Color(colora.color.r, colora.color.g, colora.color.b, 0);
            echoO.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;

            echoO.transform.localScale = transform.localScale * .7f;

            echoO.GetComponent<Echo>().scaleR = GetComponent<RectTransform>();
        }
    }


    private void OnDestroy()
    {
        RhythmManager.Instance.onMusicBeatDelegate -= SendBar;
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
