using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;

    private void Start()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        foreach (PlayerHealth player in players)
        {
            player.PlayerHit.AddListener(CameraShake);
        }
    }
    public void CameraShake()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOShakePosition(shakeDuration, shakeStrength));
        seq.Insert(0, transform.DOShakeRotation(shakeDuration, shakeStrength));
        seq.Play();
    }
}
