using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int healthPoints = 1;
    private int currentHealth;
    private int playerID;
    public UnityEvent PlayerHit;
    public UnityEvent<int> PlayerDied;
    public bool isAlive = true;

    
    void Start()
    {
        playerID = GetComponent<PlayerManager>().characterID;
        currentHealth = healthPoints;
    }


    public void OnHit()
    {
        --healthPoints;

        PlayerHit.Invoke();

        if (healthPoints <= 0 && isAlive)
            OnDeath();

    }

    public void OnDeath()
    {
        isAlive = false;
        Debug.Log("Dieded !"); 
        PlayerDied.Invoke(playerID);
    }

    public void ResetPlayer()
    {
        currentHealth = healthPoints;
        isAlive = true;
    }
}
