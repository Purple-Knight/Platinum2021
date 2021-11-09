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
    public bool zeroReset = true;

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
        Debug.Log("Combo x" + Combo);

        if(currentWeapon != null && Combo >= currentWeapon.ComboToUpgarde)
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
        if(exponential)
        {
            comboValue *= Mathf.RoundToInt(expoValue);
        }
        else
        {
            addedValue *= multiplier;
            comboValue += addedValue;
        }

        return comboValue;
    }

    public void ResetComboValues()
    {
        if (!receivedInput) ResetCombo();

        receivedInput = false;
    }

    public void ResetCombo()
    {
        Debug.Log("RESET!!!");
        if (zeroReset)
        {
            Combo = 0;
            return;
        }
    }
}
