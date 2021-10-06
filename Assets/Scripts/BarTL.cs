using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarTL : MonoBehaviour
{
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed;
    [HideInInspector] public float deleteTime;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
