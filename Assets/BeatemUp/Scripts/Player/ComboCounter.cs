using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    private int _combo = 0;
    // Modifiers
    [Range(1,10)] public int multiplier = 1;
    public bool exponential = false;
    [Range(1.0f, 2.0f)] public float expoValue = 2;

    // Reset
    public bool zeroReset = false;

    public int Combo {
        get { return _combo; }
        set { _combo = value; }
    }


    public void Up()
    {
        Combo = ApplyModifier(Combo);
        Debug.Log("Combo x" + Combo);
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

    public void ResetCombo()
    {
        if (zeroReset)
        {
            Combo = 0;
            return;
        }
    }
}
