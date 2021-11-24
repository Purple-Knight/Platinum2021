using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] List<TextMeshProUGUI> playerScoresText;
    List<int> playerScores;
    GameManager gameManager;
    
    void Start()
    {
       
    }

    public void InstantiateScore()
    {
        gameManager = GameManager.Instance;
        for (int i = gameManager.players.Count; i < 4; i++)
        {
            playerScoresText[i].gameObject.SetActive(false);
            playerScoresText[i].text = "P" + (i + 1) + " : ";
        }

        playerScores = new List<int>() { 0, 0, 0, 0 };
        gameManager.PlayerWon.AddListener(UpdateScores);
    }
    public void UpdateScores(int i)
    {
        playerScores[i]++;
        playerScoresText[i].text = "P" + (i +1) + " : " + playerScores[i];
    }
}
