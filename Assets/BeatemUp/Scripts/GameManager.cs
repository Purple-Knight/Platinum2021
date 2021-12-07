using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] GameObject timeline;

    [SerializeField] Animator endGameAnim;

    void Start()
    {
        _instance = this;

        eventManager = GetComponent<EventManager>();
        RhythmManager.Instance.StartGame();
        playersData = SaveData.Load();
        spawnPoints =  levelGen.SpawnNextMap();
        SpawnPlayer();
        timeline.transform.position = new Vector2(timeline.transform.position.x, levelGen.transform.position.y +1.5f);
        camera.SetStartPos(levelGen.transform.position);
        RhythmManager.Instance.EndOfMusic.AddListener(EndGame);
        scoreManager.InstantiateScore();
        endGameAnim.SetTrigger("StartGame");
    }

    public void SpawnPlayer()
    {
        playersAlive = new List<bool>();
        players = new List<PlayerManager>();
        numOfPlayerAlive = playersData.numberOfPlayer; 
        playerWins = new int[numOfPlayerAlive];
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            APlayerData data = playersData.allPlayerData[i];
            Debug.Log(spawnPoints[i]);
            PlayerManager playerManager = Instantiate(playerPrefab, spawnPoints[i], Quaternion.identity).GetComponent<PlayerManager>();
            playerManager.InstantiatePlayer(data.playerControllerID, i , data.myColorID, data.myCharID);
            playersAlive.Add(true);
            players.Add(playerManager.GetComponent<PlayerManager>());
            players[i].playerHealth.PlayerDied.AddListener(CheckPlayerAlive);
        }
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
            StartCoroutine(NextRound());
        }

    }

    public void ResetPlayers()
    {
        playersAlive.Clear();
        playersAlive = new List<bool>();
        numOfPlayerAlive = playersData.numberOfPlayer;
        Debug.Log(numOfPlayerAlive);
        for (int i = 0; i < numOfPlayerAlive; i++)
        {
            players[i].transform.position = spawnPoints[i];
            players[i].ResetPlayer();
            playersAlive.Add(true);
            
        }
    }

    IEnumerator NextRound()
    {
        yield return new WaitForSecondsRealtime(3);
        spawnPoints = levelGen.SpawnNextMap();
        timeline.transform.position = new Vector2(timeline.transform.position.x, -levelGen.transform.position.y);// a modifier !!!
        ResetPlayers();
        camera.SetStartPos(levelGen.transform.position);
        camera.ResetCamera();
        if (Random.Range(0, 5) >= 4)
        {
            hasEvent = true;
            endGameAnim.SetTrigger("StartEvent");
            eventManager.StartEvent();
        }
    }


    public void EndGame()
    {
        endGameAnim.SetTrigger("Countdown");
    }

    public void VictoryScreen()
    {
        //321 count down
        //disable player input
        eventManager.PlaybackSpeedOriginal();
        List<int> winners = CheckWinner();
        APlayerData data = playersData.allPlayerData[winners[0]];
        VictoryManager.Instance.InstantiateVictoryScene(playerWins);
    }

    public List<int> CheckWinner()
    {
        int winner = 0;
        List<int> winners = new List<int>();
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

            Debug.Log(victoryText);
        }
        else
        {
            winners.Add(winner);
        }
        return winners;
    }


    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
