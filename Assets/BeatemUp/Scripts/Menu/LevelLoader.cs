using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using TMPro;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    // Rewired -------------------------------------
    public IList<Joystick> joysticks;
    private List<Player> players = new List<Player>();

    //Ref ------------------------------------------
    public TextMeshProUGUI textLoad;

    public Image difficultyLevel;
    public GameObject pressA;

    private bool isOk;

    public List<Sprite> txtList = new List<Sprite>();

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
        if(RhythmManager.Instance.bpm == BPM.BPM115) difficultyLevel.sprite = txtList[0];
        else difficultyLevel.sprite = txtList[1];

        StartCoroutine(LoadAsynchronously(levelToLoad));
    }

    IEnumerator LoadAsynchronously(string levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        
        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            textLoad.text = "";//"LOADING : " + (int)(progress * 100) + "%";
            
            if (operation.progress >= 0.9f)
            {
                pressA.SetActive(true);
                //textLoad.text = "PRESS A TO CONTINUE";
                
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
