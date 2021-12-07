using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    /*[HideInInspector]*/ public bool isOk;
    public void LoadLevel(string levelToLoad)
    {
        StartCoroutine(LoadAsynchronously(levelToLoad));
    }

    IEnumerator LoadAsynchronously(string levelToLoad)
    {
        yield return null;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        
        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    operation.allowSceneActivation = true;
                }
            }


        }
        
        yield return null;
        
    }
}
