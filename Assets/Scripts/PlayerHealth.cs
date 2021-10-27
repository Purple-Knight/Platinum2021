using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int healthPoints = 1;

    public UnityEvent PlayerHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit()
    {
        --healthPoints;

        PlayerHit.Invoke();

        if (healthPoints <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        Debug.Log("Dieded !");
    }
}
