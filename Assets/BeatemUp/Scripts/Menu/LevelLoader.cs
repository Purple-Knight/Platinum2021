using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    // Rewired -------------------------------------
    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();

    //Ref ------------------------------------------
    public TextMeshProUGUI textLoad;

    public TextMeshProUGUI difficultyLevel;

    private bool isOk;
    
    private void Start()
    {
        checkController();
    }

    public void checkController()
    {
        joysticks = ReInput.controllers.GetJoysticks(); ///////////////////check connected disconected

        for (int i = 0; i < joysticks.Count; i++)
        {
            if(!players.Contains(ReInput.players.GetPlayer(i))) players.Add(ReInput.players.GetPlayer(i));
        }
        
        if(joysticks.Count == 0) // keyboard only, no controllers connected
        {
            players.Add(ReInput.players.GetPlayer(0));
        }
    }
    
    
    public void LoadLevel(string levelToLoad)
    {
        if(RhythmManager.Instance.bpm == BPM.BPM115) difficultyLevel.text = "Medium";
        else difficultyLevel.text = "Hard oh yeah !";
        
        
        StartCoroutine(LoadAsynchronously(levelToLoad));
    }

    IEnumerator LoadAsynchronously(string levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        
        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            textLoad.text = "LOADING : " + (int)(progress * 100) + "%";
            
            if (operation.progress >= 0.9f)
            {
                textLoad.text = "PRESS A TO CONTINUE";
                
                foreach (var item in players)
                {
                    if (item.GetButtonDown("Confirm"))
                    {
                        StartCoroutine((isOkSet()));
                    }
                }
            }

            if (isOk)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        
        
    }


    IEnumerator isOkSet()
    {
        GetComponent<MainMenu>().fadeGO.SetActive(true);
        //GetComponent<MainMenu>().fadeGO.GetComponent<Animator>().Play(("Fade Completed"));
        yield return new WaitForSeconds(0.5f);
        isOk = true;
    }
}
