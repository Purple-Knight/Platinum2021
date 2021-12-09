using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance { get { return _instance; } }
    private static VictoryManager _instance;

    [SerializeField] GameObject victoryCanvas;
    [SerializeField]TextMeshProUGUI playerNameText;
    [SerializeField]TextMeshProUGUI playerNumberText;
    [SerializeField] List<Image> playerImage;
    [SerializeField] List<Sprite> characterSprites;
    private bool isVictoryScreenActive = false;
    private List<Player> players = new List<Player>();
    PlayersData playersData;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        IList<Joystick> joysticks = ReInput.controllers.GetJoysticks();

        for(int i = 0; i < joysticks.Count; i++)
        {
            if (!players.Contains(ReInput.players.GetPlayer(i)))
                players.Add(ReInput.players.GetPlayer(i));
        }
        playersData = SaveData.Load();
    }

    private void Update()
    {
        if (isVictoryScreenActive)
        {
            foreach (Player player in players)
            {
                if (player.GetButton("Continue"))
                {
                    GameManager.Instance.LoadScene("TestLevelGen");
                }
                else if (player.GetButton("Back"))
                {
                    RhythmManager.Instance.StartMenu();
                    GameManager.Instance.LoadScene("Menu");
                }
            }
        }
    }
    public void InstantiateVictoryScene(int[] winners)
    {
        List<int> winnerOrder = new List<int>();
        List<int> allPlayers = new List<int>();
        for (int i = 0; i < winners.Length; i++)
        {
            allPlayers.Add(i);
        }
        for (int i = 0; i < winners.Length; i++)
        {
            int highestPlayer = 0;
            for (int j = 1; j < allPlayers.Count; j++)
            {
                if (winners[allPlayers[highestPlayer]] < winners[allPlayers[j]])
                {
                    highestPlayer = allPlayers[j];
                }
            }
            winnerOrder.Add(allPlayers[highestPlayer]);
            allPlayers.RemoveAt(highestPlayer);
        }
        isVictoryScreenActive = true;
        RhythmManager.Instance.StopAllMusic();
        victoryCanvas.SetActive(true);
        playerNameText.text = "Victory Player " + (winnerOrder[0] +1);
        for (int i = 0; i < winnerOrder.Count; i++)
        {
            playerImage[i].sprite = characterSprites[playersData.allPlayerData[winnerOrder[i]].myCharID];

        }
        for (int i = winnerOrder.Count; i < 3; i++)
        {
            playerImage[i].gameObject.SetActive(false);
        }

        /*playerNumberText.text = "Player " + playerNumber;
        playerImage.color = playerColor;*/
    }
}
