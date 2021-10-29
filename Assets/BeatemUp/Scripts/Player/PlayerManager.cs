using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PlayerMovement movement;
    public PlayerWeapon playerWeapon;
    public PlayerHealth playerHealth;
    public int characterID;
    [SerializeField] List<Sprite> sprites;
    Color playerColor;
    [SerializeField] ParticleSystem notesParticle;

    public void InstantiatePlayer(int conrtollerID, int playerID, Color color, int spriteID)
    {
        characterID = playerID;
        movement.playerID = conrtollerID;
        movement.playerColor = color;
        playerColor = color;
        spriteRenderer.sprite = sprites[spriteID];
        spriteRenderer.color = color;

        playerHealth.PlayerDied.AddListener(PlayerDied);
        movement.InstantiateMovement();
    }

    public void PlayerDied(int i)
    {
        notesParticle.Play();
        playerWeapon.enabled = false;
        playerHealth.enabled = false;
        spriteRenderer.color = new Color(playerColor.r, playerColor.g, playerColor.g, .3f);
        movement.playerColor = new Color(playerColor.r, playerColor.g, playerColor.g, .3f);
        gameObject.tag ="Ghost";
        gameObject.layer = 8;
        //movement.ResetPositions();
    }

    public void ResetPlayer()
    {
        playerWeapon.enabled = true;
        playerHealth.enabled = true;
        movement.enabled = true;
        movement.playerColor = playerColor;
        spriteRenderer.color = playerColor;
        gameObject.tag = "Player";
        gameObject.layer = 7;
        movement.ResetPositions();
        playerHealth.ResetPlayer();
    }
}
