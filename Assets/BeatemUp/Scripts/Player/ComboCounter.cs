using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComboCounter : MonoBehaviour
{
    private PlayerManager playerManager;
    private Weapon currentWeapon;

    [HideInInspector] public UnityEvent onComboGoalReached;
    private bool receivedInput = false;

    [SerializeField] private int _combo = 0;
    // Modifiers
    [Range(1,10)] public int multiplier = 1;
    public bool exponential = false;
    [Range(1.0f, 2.0f)] public float expoValue = 2;

    // Reset
    public bool downgradeWeaponOnReset = true;
    public bool zeroReset = false;

    public int maxLimit = 1000;


    #region Get / Set
    public int Combo {
        get { return _combo; }
        set { _combo = value; }
    }
    #endregion

    public void Init(PlayerManager _playerManager)
    {
        playerManager = _playerManager;
    }

    public void CurrentWeaponRef(in Weapon weaponRef)
    {
        currentWeapon = weaponRef;
    }

    public void Keep()
    {
        if (receivedInput) return;
        else receivedInput = true;
    }

    public void Up()
    {
        if (receivedInput) return;

        Keep();

        Combo = ApplyModifier(Combo);

        Color debugColor = playerManager.CharacterID switch
        {
            0 => new Color(232 / 255f, 53 / 255f, 161 / 255f),
            1 => Color.cyan,
            2 => new Color(53 / 255f, 232 / 255f, 107 / 255f),
            3 => Color.yellow,
            _ => Color.white,
        };
        Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>J{3}</color>: Combo x{4}", (byte)(debugColor.r * 255f), (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), playerManager.CharacterID + 1, Combo));

        if(currentWeapon != null && Combo >= currentWeapon.ComboToUpgrade)
        {
            currentWeapon.Upgarde();
        }
    }

    public void Down(int value)
    { 
        Combo -= value;
    }

    public int ApplyModifier(int comboValue, int addedValue = 1)
    {
        if(exponential && Combo > 0)
        {
            comboValue = Mathf.RoundToInt(comboValue * expoValue);
        }
        else
        {
            addedValue *= multiplier;
            comboValue += addedValue;
        }

        return Mathf.Clamp(comboValue, 0, maxLimit);
    }

    public void ResetComboValues()
    {
        if (Combo <= 0) return;

        if (!receivedInput) ResetCombo();
        receivedInput = false;
    }

    public void ResetCombo()
    {
        if (zeroReset) // to 0
        {
            Combo = 0;
            playerManager.playerWeapon.SwapToBaseWeapon(); // Reset to Base Weapon
        }
        else if(downgradeWeaponOnReset) // downgrade Pallier
        {
            currentWeapon.Downgrade();
            Combo = currentWeapon.ComboToDowngrade;
        }
    }
}
