using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSwordStats : MonoBehaviour {

    private DialogueSpeaker dialogueSpeaker;
    private SwordStats thisSwordStats;

    private float damageMultiplier;
    private int range;
    private WeaponEffect weaponEffect;
    private SwordPickup swordPickup;

    private Color damageColor;
    private Color rangeColor;
    private Color effectColor;

    private string damageMultiplierText;
    private string rangeText;
    private string weaponEffectText;

    private PlayerMove playerMove;

	// Use this for initialization
	void Start () 
    {
        swordPickup = GetComponent<SwordPickup>();
        thisSwordStats = GetComponent<SwordStats>();
        dialogueSpeaker = GetComponent<DialogueSpeaker>();

        dialogueSpeaker.OnGetPlayer += SetDialgeText;

        weaponEffect = thisSwordStats.weaponEffect;
	}

    void Update()
    {
        if (dialogueSpeaker.CurrentPlayer != null)
        {
            if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase.WasPressed)
            {
                swordPickup.Pickup(dialogueSpeaker.CurrentPlayer);
                dialogueSpeaker.Close();
                enabled = false;
                dialogueSpeaker.enabled = false;
            }
        }
    }

    void SetTextColor(float value, ref Color color)
    {
        if (value > 0)
            color = Color.green;
        else if (value < 0)
            color = Color.red;
        else if (value == 0)
            color = new Color(1, .7f, 0);
    }

    void GetStatDifferences(PlayerInformation playerInfo)
    {
        damageMultiplier = thisSwordStats.damageMultiplier - playerInfo.GetComponent<PlayerAttack>().sword.damageMultiplier;
        SetTextColor(damageMultiplier, ref damageColor);
        range = thisSwordStats.range - playerInfo.GetComponent<PlayerAttack>().sword.range;
        SetTextColor(range, ref rangeColor);

        if (damageMultiplier > 0)
            damageMultiplierText = "+" + damageMultiplier;
        else if (damageMultiplier == 0)
            damageMultiplierText = "-";
        else
            damageMultiplierText = "" + damageMultiplier;
        
        if (range > 0)
            rangeText = "+" + range;
        else if (range == 0)
            rangeText = "-";
        else
            rangeText = "" + range;

        if (weaponEffect != WeaponEffect.Nothing)
        {
            if (weaponEffect == WeaponEffect.Burn)
                effectColor = Color.red;
            else if (weaponEffect == WeaponEffect.Poison)
                effectColor = Color.magenta;
            else if (weaponEffect == WeaponEffect.SlowDeath)
                effectColor = Color.green;
        }
    }

    void SetDialgeText(PlayerInformation playerInfo, bool b)
    {
        GetStatDifferences(playerInfo);

        string text = "";

        //sets the panel to have the sword name, sword stats and weapon effect
        text += thisSwordStats.swordName + "\n"; 
        text += string.Format("Damage: <color=#{1}>{0}</color>\n", damageMultiplierText, ColorUtility.ToHtmlStringRGB(damageColor));
        text += string.Format("Range: <color=#{1}>{0}</color>", rangeText, ColorUtility.ToHtmlStringRGB(rangeColor));
        if (weaponEffect != WeaponEffect.Nothing)
        {
            if (weaponEffect == WeaponEffect.Burn)
                text += string.Format("\n<color=#{1}>{0}</color>", "Burns", ColorUtility.ToHtmlStringRGB(effectColor));
            else if (weaponEffect == WeaponEffect.Poison)
                text += string.Format("\n<color=#{1}>{0}</color>", "Poisons", ColorUtility.ToHtmlStringRGB(effectColor));
            else if (weaponEffect == WeaponEffect.SlowDeath)
                text += string.Format("\n<color=#{1}>{0}</color>", "Eats Away", ColorUtility.ToHtmlStringRGB(effectColor));
        }

        dialogueSpeaker.lines[0] = text;

        dialogueSpeaker.UpdateLines();
    }
}
