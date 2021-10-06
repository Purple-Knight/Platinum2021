using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarTL : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    void Start()
    {
        Destroy(gameObject, 5);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
