using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;

    public List<PlayerManager> players;
    int numOfPlayerAlive;
    List<bool> playersAlive;
    int[] playerWins;
    [SerializeField] List<Vector2> spawnPoints;

    public UnityEvent<int> PlayerWon;

    PlayersData playersData;
    [SerializeField] GameObject playerPrefab;

    public CameraManager camera;
    public LevelGenerator levelGen;
    public ScoreManager scoreManager;
    EventManager eventManager;
    bool hasEvent = false;
    [SerializeField] Image winnerImage;
    [SerializeField] Animator winnerAnim;
    [SerializeField] List<Sprite> victoryImages;

    [SerializeField] Animator endGameAnim;
    int currentRound = 0;

    void Start()
    {
        _instance = this;

        eventManager = GetComponent<EventManager>();
        RhythmManager.Instance.StartGame();
        playersData = SaveData.Load();
        spawnPoints =  levelGen.SpawnNextMap();
        SpawnPlayer();
        camera.SetStartPos(levelGen.transform.position);
        camera.ResetCamera();
        RhythmManager.Instance.EndOfMusic.AddListener(EndGame);
        scoreManager.InstantiateScore();
        BlockAllPlayers();
        endGameAnim.SetTrigger("StartGame");
    }


    public void SpawnPlayer()
    {
        playersAlive = new List<bool>();
        players = new List<PlayerManager>();
        numOfPlayerAlive = playersData.numberOfPlayer;
        //Debug.Log(" before" + playerWins.Length);
        playerWins = new int[numOfPlayerAlive];
        Debug.Log(playerWins.Length);

        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            APlayerData data = playersData.allPlayerData[i];
            PlayerManager playerManager = Instantiate(playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerManager>();
            playerManager.InstantiatePlayer(data.playerControllerID, i , data.myColorID, data.myCharID);
            playersAlive.Add(true);
            players.Add(playerManager.GetComponent<PlayerManager>());
            players[i].playerHealth.PlayerDied.AddListener(CheckPlayerAlive);
            players[i].playerMovement.pauseGame.AddListener(PauseGame);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        BlockAllPlayers();
        RhythmManager.Instance.PauseGame();
        PauseMenu.Instance.OpenMenu();
    }
    public void UnpauseGame()
    {
        Time.timeScale = 1;
        FreeAllPlayers();
        RhythmManager.Instance.UnpauseGame();
    }

    public void CheckPlayerAlive(int playerID )
    {
        numOfPlayerAlive--;
        playersAlive[playerID] = false;
        //players[playerID].transform.position = deathRoom.position;
        

        if (numOfPlayerAlive == 1)
        {
            int playerAlive = 0;
            for (int i = 0; i < playersAlive.Count; i++)
            {
                if (playersAlive[i])
                {
                    playerAlive = i;
                }
                players[i].playerMovement.enabled = false;
            }
            PlayerWon.Invoke(playerAlive);
            playerWins[playerAlive]++;
            if (hasEvent)
            {
                hasEvent = false;
                eventManager.EndEvent();
            }
            players[playerAlive].BlockPlayerInput();
            winnerImage.sprite = victoryImages[playersData.allPlayerData[playerAlive].myCharID];
            winnerAnim.SetTrigger("Win");
            RhythmManager.Instance.Victory();
            StartCoroutine(NextRound());
        }

    }

    public void ResetPlayers()
    {
        playersAlive.Clear();
        playersAlive = new List<bool>();
        numOfPlayerAlive = playersData.numberOfPlayer;
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].transform.position = spawnPoints[i];
            players[i].ResetPlayer();
            playersAlive.Add(true);
            
        }
    }
    
    public void ResetPlayersBeforeEvent()
    {
        playersAlive.Clear();
        playersAlive = new List<bool>();
        numOfPlayerAlive = playersData.numberOfPlayer;
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].transform.position = spawnPoints[i];
            playersAlive.Add(true);
            players[i].ResetColor();

        }
    }

    public void ResetPlayerAfterAnim()
    {
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].ResetPlayer();
        }
    }

    IEnumerator NextRound()
    {
        yield return new WaitForSecondsRealtime(3);
        winnerAnim.SetTrigger("End");
        spawnPoints = levelGen.SpawnNextMap();
        camera.SetStartPos(levelGen.transform.position);
        ResetPlayersBeforeEvent();
        camera.ResetCamera();
        currentRound++;
        if (Random.Range(0, 5) < 1 && currentRound != 0)
        {
            hasEvent = true;
            endGameAnim.SetTrigger("StartEvent");
            eventManager.StartEvent();
        }
        else
        {
            endGameAnim.SetTrigger("StartRound");
        }
    }


    public void EndGame()
    {
        endGameAnim.ResetTrigger("StartRound");
        endGameAnim.ResetTrigger("StartEvent");

        endGameAnim.SetTrigger("Countdown");
        StartCoroutine(VictoryScreenSafty());
    }

    public void VictoryScreen()
    {
        //321 count down
        //disable player input
        eventManager.PlaybackSpeedOriginal();
        //List<int> winners = CheckWinner();
        //APlayerData data = playersData.allPlayerData[winners[0]];
        //
        VictoryManager.Instance.InstantiateVictoryScene(playerWins);
    }

    public List<int> CheckWinner()
    {
        int winner = 0;
        List<int> winners = new List<int>();
        winners.Clear();
        bool multipleWinner = false;
        for (int i = 1; i < playerWins.Length; i++)
        {
            if(playerWins[i] > playerWins[winner])
            {
                winner = i;
                multipleWinner = false;
            }else if (playerWins[winner] == playerWins[i])
            {
                multipleWinner = true;
            }
        }
        if (multipleWinner)
        {
            string victoryText = "Tied ! :";
            for (int i = 0; i < playerWins.Length; i++)
            {
                if (playerWins[i] == playerWins[winner])
                {
                    winners.Add(i);
                    victoryText += " player " + i;
                }
            }

        }
        else
        {
            winners.Add(winner);
        }
        return winners;
    }

    public void BlockAllPlayers()
    {
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].BlockPlayerInput();
        }
    }
    
    public void FreeAllPlayers()
    {
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].FreePlayerInput();
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator VictoryScreenSafty()
    {
        yield return new WaitForSeconds(7);
        VictoryManager.Instance.IsVictoryActive(playerWins);
    }
}
