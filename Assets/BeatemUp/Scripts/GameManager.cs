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
    [SerializeField] GameObject timeline;

    void Start()
    {
        _instance = this;
        
        playersData = SaveData.Load();
        spawnPoints =  levelGen.SpawnNextMap();
        SpawnPlayer();
        timeline.transform.position = new Vector2(timeline.transform.position.x, -levelGen.transform.position.y);
        camera.SetStartPos(levelGen.transform.position);
        RhythmManager.Instance.EndOfMusic.AddListener(EndGame);
        scoreManager.InstantiateScore();
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
            Debug.Log("player " + players[playerAlive].CharacterID + " won");
            PlayerWon.Invoke(playerAlive);
            playerWins[playerAlive]++;
            for (int i = 0; i < playerWins.Length; i++)
            {
                Debug.Log("Player " + (i +1) + " : " + playerWins[i]);
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
    }


    public void EndGame()
    {
        //321 count down
        //disable player input
        RhythmManager.Instance.eventMusic[1].Post(RhythmManager.Instance.gameObject);
        List<int> winners = CheckWinner();
        APlayerData data = playersData.allPlayerData[winners[0]];
        VictoryManager.Instance.InstantiateVictoryScene("Character idk", winners[0] +1, data.myCharID, data.myColorID);
        //new game and menu buttons

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
            Debug.Log("winner : " + winner);
            return winners;
    }


    public void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
