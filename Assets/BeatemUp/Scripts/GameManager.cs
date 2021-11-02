using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    void Start()
    {
        _instance = this;
        
        playersData = SaveData.Load();
        spawnPoints =  levelGen.GenerateLevel(); 
        SpawnPlayer();
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
                players[i].movement.enabled = false;
            }
            Debug.Log("player " + players[playerAlive].characterID + " won");
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
        ResetPlayers();
        camera.ResetCamera();
    }
}
