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
    bool canBegin;
    public float beatToReach;
    public float multiplicatorSpeed;

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
    }

    public void SendBar()
    {
        if (canBegin && actualBeat < (numberOfBeat - 1))
        {
            var lastBar = Instantiate(bar, transform.position, transform.rotation);

            var direction = endTimeline.transform.position - transform.position;
            var distance = Vector2.Distance(transform.position, endTimeline.transform.position);

            var time = beatToReach * RhythmManager.Instance.beatDuration;
            
            var speed = (distance / time) * multiplicatorSpeed;

            var barScript = lastBar.GetComponent<BarTL>();
            barScript.direction = direction.normalized;
            barScript.speed = speed;
            barScript.deleteTime = time;

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
        var colora = gameObject.GetComponent<SpriteRenderer>();

        echoO.GetComponent<SpriteRenderer>().color = new Color(colora.color.r, colora.color.g, colora.color.b, 0);
        echoO.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;

        echoO.transform.localScale = transform.localScale * 2;

        echoO.GetComponent<Echo>().scale = transform;
    }




    private void OnGUI()
    {
        if (!guiDebug) return;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Beat"))
        {
            beatBool = !beatBool;
        }
        GUILayout.BeginArea(guiDebugArea);

        if (beatBool)
        {
            GUILayout.TextField("Beat\n" + "Actual Beat : " + actualBeat + "\nBeat in Total : " + numberOfBeat);
        }

        GUILayout.EndArea();
    }
}
