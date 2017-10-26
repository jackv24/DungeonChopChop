using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowShieldStats : MonoBehaviour {

    private DialogueSpeaker dialogueSpeaker;
    private ShieldStats thisShieldStats;

    private float resistanceMultiplier;
    private float speedDamping;
    private WeaponEffect weaponEffect;
    private ShieldPickup shieldPickup;

    private Color resistanceColor;
    private Color speedDampingColor;

    private string resistanceText;
    private string speedDampingText;

    private PlayerMove playerMove;

	// Use this for initialization
	void Start () 
    {
        shieldPickup = GetComponent<ShieldPickup>();
        thisShieldStats = GetComponent<ShieldStats>();
        dialogueSpeaker = GetComponent<DialogueSpeaker>();

        dialogueSpeaker.OnGetPlayer += SetDialgeText;
	}

    void Update()
    {
        if (dialogueSpeaker.CurrentPlayer != null)
        {
            if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase.WasPressed)
            {
                shieldPickup.Pickup(dialogueSpeaker.CurrentPlayer);
                dialogueSpeaker.Close();
                enabled = false;
                dialogueSpeaker.enabled = false;
                Destroy(GetComponent<Collider>());
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
        if (playerInfo.playerAttack.shield)
        {
            resistanceMultiplier = thisShieldStats.blockingResistance - playerInfo.GetComponent<PlayerAttack>().shield.blockingResistance;
            SetTextColor(resistanceMultiplier, ref resistanceColor);
            speedDamping = thisShieldStats.speedDamping - playerInfo.GetComponent<PlayerAttack>().shield.speedDamping;
            SetTextColor(speedDamping, ref speedDampingColor);

            if (resistanceMultiplier > 0)
                resistanceText = "+" + resistanceMultiplier;
            else if (resistanceMultiplier == 0)
                resistanceText = "-";
            else
                resistanceText = "" + resistanceMultiplier;
        
            if (speedDamping > 0)
                speedDampingText = "+" + speedDamping;
            else if (speedDamping == 0)
                speedDampingText = "-";
            else
                speedDampingText = "" + speedDamping;
        }
        else
        {
            resistanceText = "+" + thisShieldStats.blockingResistance;
            resistanceColor = Color.green;

            speedDampingText = "+" + thisShieldStats.speedDamping;
            speedDampingColor = Color.green;
        }
    }

    void SetDialgeText(PlayerInformation playerInfo, bool b)
    {
        GetStatDifferences(playerInfo);

        string text = "";

        //sets the panel to have the sword name, sword stats and weapon effect
        text += thisShieldStats.shieldName + "\n"; 
        text += string.Format("Blocking Resistance: <color=#{1}>{0}</color>\n", resistanceText, ColorUtility.ToHtmlStringRGB(resistanceColor));
        text += string.Format("Speed Damping: <color=#{1}>{0}</color>", speedDampingText, ColorUtility.ToHtmlStringRGB(speedDampingColor));

        dialogueSpeaker.lines[0] = text;

        dialogueSpeaker.UpdateLines();
    }
}
