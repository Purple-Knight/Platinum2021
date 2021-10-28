using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int healthPoints = 1;
    private int playerID;
    public UnityEvent PlayerHit;
    public UnityEvent<int> PlayerDied;
    public bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        playerID = GetComponent<PlayerManager>().characterID;
    }

    // Update is called once per frame
    void Update()
    {
        
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

        GetComponent<PlayerWeapon>().enabled = false;
        GetComponent<PlayerMovement>().ResetPositions();
    }
}
