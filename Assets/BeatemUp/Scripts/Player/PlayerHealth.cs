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
    Animator playerAnimator;

    
    void Start()
    {
        playerID = GetComponent<PlayerManager>().CharacterID;
        currentHealth = healthPoints;
        playerAnimator = GetComponent<Animator>();
    }


    public void OnHit()
    {
        --currentHealth;

        PlayerHit.Invoke();

        playerAnimator.SetTrigger("Hit");
        
        if (currentHealth <= 0 && isAlive)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {

        isAlive = false;
        PlayerDied.Invoke(playerID);

    }

    public void ResetPlayer()
    {
        currentHealth = healthPoints;
        isAlive = true;
    }
}
