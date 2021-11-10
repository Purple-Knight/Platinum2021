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

    #region Debug
    public bool debug = false;
    int maxSteps = 0;
    float freeTime = 0;
    string maxStepsStr = "0";
    string freeTimeStr = "0";
    Rect debugRect;
    #endregion
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
        debugRect = new Rect(10 + characterID * 100.0f, 10, 100, 150);
        debug = true;
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
        playerWeapon.SwapToBaseWeapon();
    }

    public void IgnoreTimelineForSec(float ignoreTime, int maxNumOfStepsPerSec)
    {
        playerMovement.StartFreeMovement(maxNumOfStepsPerSec);
        StartCoroutine(FreeMovementTime(ignoreTime));
    }
    
    IEnumerator FreeMovementTime(float freeTime)
    {
        yield return new WaitForSeconds(freeTime);
        playerMovement.EndFreeMovement();
    }

    private void OnGUI()
    {
        if (!debug) return;
        GUILayout.BeginArea(debugRect);
        GUILayout.TextArea("Player " + characterID);
        GUILayout.BeginHorizontal();
        GUILayout.TextArea("Max Steps :");
        maxStepsStr = GUILayout.TextField(maxStepsStr);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.TextArea("Event Time :");
        freeTimeStr = GUILayout.TextField(freeTimeStr);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Lance FreeMovement Event"))
        {
            if(float.TryParse(freeTimeStr, out freeTime)&& int.TryParse(maxStepsStr, out maxSteps))
                IgnoreTimelineForSec(freeTime, maxSteps);
        }
        GUILayout.EndArea();
    }
}