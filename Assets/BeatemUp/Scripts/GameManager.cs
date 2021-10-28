using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;

    [SerializeField] PlayerHealth[] players;
    int numOfPlayerAlive;
    [SerializeField] Transform deathRoom;

    public UnityEvent<int> PlayerWon;

    void Start()
    {
        _instance = this;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].PlayerDied.AddListener(CheckPlayerAlive);
        }
        numOfPlayerAlive = players.Length;
    }

    
    void Update()
    {
        
    }

    public void CheckPlayerAlive(int playerID )
    {
        numOfPlayerAlive--;
        int playerAlive =0;
        players[playerID].transform.position = deathRoom.position;

        if (numOfPlayerAlive == 1)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].isAlive)
                    playerAlive = i;
            }

            Debug.Log("player " + players[playerAlive].gameObject.name + " won");
            PlayerWon.Invoke(playerAlive);
        }

    }
}
