using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PlayerMovement movement;
    public PlayerWeapon playerWeapon;
    public int characterID;
    [SerializeField] List<Sprite> sprites;

    public void InstantiatePlayer(int conrtollerID, int playerID, Color color, int spriteID)
    {
        characterID = playerID;
        movement.playerID = conrtollerID;
        movement.playerColor = color;
        spriteRenderer.sprite = sprites[spriteID];
        spriteRenderer.color = color;

        movement.InstantiateMovement();
    }

    public void ResetPlayer()
    {
        movement.ResetPositions();
        playerWeapon.enabled = true;
    }
}
