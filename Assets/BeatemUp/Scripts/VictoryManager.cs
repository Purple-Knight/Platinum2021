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
    [SerializeField] Image playerImage;
    [SerializeField] List<Sprite> characterSprites;
    private bool isVictoryScreenActive = false;
    private List<Player> players = new List<Player>();
    [SerializeField] AK.Wwise.Event stopMusic;


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
                    GameManager.Instance.LoadScene("Menu");
                }
            }
        }
    }
    public void InstantiateVictoryScene(string playerName, int playerNumber, int playerCharacterId, Color playerColor)
    {
        isVictoryScreenActive = true;
        stopMusic.Post(RhythmManager.Instance.gameObject);
        victoryCanvas.SetActive(true);
        playerNameText.text = playerName;
        playerNumberText.text = "Player " + playerNumber;
        playerImage.sprite = characterSprites[playerCharacterId];
        playerImage.color = playerColor;
    }
}
