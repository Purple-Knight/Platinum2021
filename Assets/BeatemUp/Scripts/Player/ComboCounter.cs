using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboCounter : MonoBehaviour
{
    // ----- Player refs
    private PlayerManager playerManager;
    private Weapon currentWeapon;

    // ----- Inputs
    private bool GotInput = false;

    // ----- Combo Values
    [SerializeField] private int _combo = 0;
    // Modifiers
    [Range(1,10)] public int multiplier = 1;
    public bool exponential = false;
    [Range(1.0f, 2.0f)] public float expoValue = 2;

    // Reset
    public bool downgradeWeaponOnReset = true;
    public bool zeroReset = false;

    public int maxLimit = 1000;

    // ----- Feedbacks
    public Transform TMP;
    private TextMeshProUGUI comboText;
    Vector3 initialScale;

    #region Get / Set
    public int Combo {
        get { return _combo; }
        set { _combo = value; }
    }
    #endregion

    public void Init(PlayerManager _playerManager)
    {
        playerManager = _playerManager;
        Combo = 0;

        comboText = TMP.GetComponentInChildren<TextMeshProUGUI>();
        initialScale = comboText.rectTransform.localScale;
        Debug.Log(initialScale.x + " Scale");
    }

    public void CurrentWeaponRef(in Weapon weaponRef)
    {
        currentWeapon = weaponRef;
        UpdateText();
    }

    public void Keep()
    {
        if (GotInput) return;
        else GotInput = true;
    }

    // Modify Combo Values
    public void Up()
    {
        if (GotInput) return;

        Keep();

        Combo = ApplyModifier(Combo);

        if(currentWeapon != null && Combo >= currentWeapon.ComboToUpgrade)
        {
            currentWeapon.Upgarde();
        }

        UpdateText();
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

    // Resets
    public void ResetComboValues(bool forceReset = false, bool playAnim = true)
    {
        if ((Combo <= 0 && !GotInput) && !forceReset) return;

        if (!GotInput || forceReset) ResetCombo(playAnim);
        GotInput = false;
    }

    private void ResetCombo(bool playAnim)
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

        if(playAnim)
            playerManager.playerAnimator.SetTrigger("BeatMissed");
        UpdateText();
    }

    // Feedback
    private void UpdateText()
    {
        if
            (Combo <= 0) comboText.color = new Color(1, 1, 1, 0);
        else
        {
            if (comboText.color != currentWeapon.comboTextColor) comboText.color = currentWeapon.comboTextColor;
            comboText.text = "x" + Combo;
        }

        //----
        {
            Color debugColor = playerManager.PlayerID switch
            {
                0 => new Color(232 / 255f, 53 / 255f, 161 / 255f),
                1 => Color.cyan,
                2 => new Color(53 / 255f, 232 / 255f, 107 / 255f),
                3 => Color.yellow,
                _ => Color.white,
            };

            //Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>J{3}</color>: Combo x{4}",
                //(byte)(debugColor.r * 255f), (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), playerManager.PlayerID + 1, Combo));
            //----
        }
    }

    public void WeaponSwapFeedback(bool upgrade)
    {
        TMP.DOComplete();
        Sequence bounceSeq = DOTween.Sequence();
        if(upgrade)
        {
            bounceSeq.Append(comboText.transform.DOScale(initialScale * 2.2f,.07f));
            bounceSeq.Append(comboText.transform.DOScale(initialScale, .13f));
        }
        else
        {
            bounceSeq.Append(comboText.transform.DOScale(initialScale * 0.6f, .04f));
            bounceSeq.Append(comboText.transform.DOScale(initialScale, .16f));
        }
    }
}
