using System.Collections;
using UnityEngine;

public class Team5BeatWall : MonoBehaviour
{
    public enum BeatAlternatingMode
    {
        Odd = 0,
        Even
    }

    public RhythmManager rhythmManager;
    private SpriteRenderer _spriteRenderer;
    public Color beatAnimColor;
    public float beatAnimDuration = 0.1f;

    private int _nbBeats = 0;
    public BeatAlternatingMode beatAlternatingMode = BeatAlternatingMode.Even;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rhythmManager.onMusicBeatDelegate += OnMusicBeat;
    }

    private void OnDestroy()
    {
        rhythmManager.onMusicBeatDelegate -= OnMusicBeat;
    }

    private void OnMusicBeat()
    {
        ++_nbBeats;

        switch (beatAlternatingMode) {
            case BeatAlternatingMode.Even:
                if (_nbBeats % 2 == 0) {
                    StopAllCoroutines();
                    StartCoroutine(ChangeColorBeat());
                }
                break;

            case BeatAlternatingMode.Odd:
                if (_nbBeats % 2 == 1) {
                    StopAllCoroutines();
                    StartCoroutine(ChangeColorBeat());
                }
                break;
        }
    }

    public IEnumerator ChangeColorBeat()
    {
        Color startColor = beatAnimColor;
        Color endColor = Color.white;

        float timer = 0f;
        while (timer <= beatAnimDuration) {
            timer += Time.deltaTime;
            yield return null;

            float ratio = timer / beatAnimDuration;
            _spriteRenderer.color = Color.Lerp(startColor, endColor, ratio);
        }

        _spriteRenderer.color = endColor;
    }
}
