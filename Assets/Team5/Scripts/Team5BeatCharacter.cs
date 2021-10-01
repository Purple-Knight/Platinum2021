using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Team5BeatCharacter : MonoBehaviour
{
    public Team5RhythmManager rhythmManager;

    public float xMin = -10f;
    public float xMax = 10f;
    public float yMin = -10f;
    public float yMax = 10f;

    public float moveStep = 5f;
    public float moveDuration = 0.1f;
    public AnimationCurve jumpStepCurve;

    private Vector3 _nextPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _nextPosition = transform.position;
        rhythmManager.onMusicBeatDelegate += OnMusicBeat;
    }

    private void OnDestroy()
    {
        rhythmManager.onMusicBeatDelegate -= OnMusicBeat;
    }

    private void OnMusicBeat()
    {
        StopAllCoroutines();
        switch (Random.Range(0, 4)) {
            case 0:
                StartCoroutine(MoveBeat(1f, 0f));
                break;

            case 1:
                StartCoroutine(MoveBeat(-1, 0f));
                break;

            case 2:
                StartCoroutine(MoveBeat(0, 1f));
                break;

            case 3:
                StartCoroutine(MoveBeat(0, -1f));
                break;
        }
    }

    IEnumerator MoveBeat(float dirX, float dirY)
    {
        Vector3 origin = _nextPosition;
        _nextPosition = origin + new Vector3(dirX * moveStep, dirY * moveStep, 0f);
        _nextPosition.x = Mathf.Clamp(_nextPosition.x, xMin, xMax);
        _nextPosition.y = Mathf.Clamp(_nextPosition.y, yMin, yMax);

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dirX);
        transform.localScale = scale;

        float timer = 0f;
        while (timer <= moveDuration) {
            timer += Time.deltaTime;
            float ratio = timer / moveDuration;
            float jumpY = jumpStepCurve.Evaluate(ratio);
            transform.position = Vector3.Lerp(origin, _nextPosition, ratio) + new Vector3(0f, jumpY, 0f);
            yield return null;
        }

        transform.position = _nextPosition;
    }
}
