using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    Quaternion originRotate;
    Vector3 originPosition;
    PlayerHealth[] players;

    private void Start()
    {
        originPosition = transform.position;
        originRotate = transform.rotation;

        GameManager.Instance.PlayerWon.AddListener(CameraZoom);

        players = FindObjectsOfType<PlayerHealth>();
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
        seq.Append(transform.DORotateQuaternion(originRotate, .1f ));
        seq.Append(transform.DOMove(originPosition, .1f ));
        seq.Play();
    }

    public void CameraZoom(int playerID)
    {
        Vector3 zoomCenter = players[playerID].transform.position;
        zoomCenter.z = transform.position.z;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(zoomCenter, 1));
        seq.Insert(0, Camera.main.DOOrthoSize(1, 1));

    }

}
