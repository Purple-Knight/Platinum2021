using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        StartCoroutine(Delay(sceneName));
    }

    IEnumerator Delay(string sceneName)
    {
        yield return new WaitForSeconds(.5f);
        RhythmManager.Instance.eventMusic[1].Post(RhythmManager.Instance.gameObject);
        SceneManager.LoadScene(sceneName);
    }
}
