using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PlayerMovement playerMovement;
    public PlayerWeapon playerWeapon;
    public ComboCounter comboManager;
    public PlayerHealth playerHealth;
    private int characterID;
    [SerializeField] List<Sprite> sprites;
    Color playerColor;
    [SerializeField] ParticleSystem notesParticle;

    public int CharacterID { get => characterID; }

    public void InstantiatePlayer(int conrtollerID, int playerID, Color color, int spriteID)
    {
        characterID = playerID;
        playerMovement.playerID = conrtollerID;
        playerMovement.playerColor = color;
        playerColor = color;
        spriteRenderer.sprite = sprites[spriteID];
        spriteRenderer.color = color;

        playerHealth.PlayerDied.AddListener(PlayerDied);
        playerMovement.InstantiateMovement();
        comboManager.Init(this);
    }

    public void PlayerDied(int i)
    {
        notesParticle.Play();
        playerWeapon.enabled = false;
        playerHealth.enabled = false;
        spriteRenderer.color = new Color(playerColor.r, playerColor.g, playerColor.g, .3f);
        playerMovement.playerColor = new Color(playerColor.r, playerColor.g, playerColor.g, .3f);
        gameObject.tag ="Ghost";
        gameObject.layer = 8;
        //movement.ResetPositions();
    }

    public void ResetPlayer()
    {
        playerWeapon.enabled = true;
        playerHealth.enabled = true;
        playerMovement.enabled = true;
        playerMovement.playerColor = playerColor;
        spriteRenderer.color = playerColor;
        gameObject.tag = "Player";
        gameObject.layer = 7;
        playerMovement.ResetPositions();
        playerHealth.ResetPlayer();
    }
}
