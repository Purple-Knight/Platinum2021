using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    [SerializeField] List<TextMeshProUGUI> playerScoresText;
    [SerializeField] List<Image> playerScoresImages;
    [SerializeField] List<Sprite> characterSprites;
    List<int> playerScores;
    GameManager gameManager;
    
    void Start()
    {
       
    }

    public void InstantiateScore()
    {
        gameManager = GameManager.Instance;
        for (int i = 0; i < gameManager.players.Count; i++)
        {
            playerScoresImages[i].sprite = characterSprites[gameManager.players[i].CharacterID];
            playerScoresText[i].text = "P" + (i + 1) + " : 0";

        }
        for (int i = gameManager.players.Count; i < 4; i++)
        {
            playerScoresText[i].gameObject.SetActive(false);
            playerScoresImages[i].gameObject.SetActive(false);
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
