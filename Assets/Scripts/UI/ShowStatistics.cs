using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStatistics : MonoBehaviour {

    public Vector2 size;
    public Vector2 pos;
	
	// Update is called once per frame
	void OnGUI () {

        if (enabled)
        {
            string text = "";

            text += "Enemies Killed\n";
            text += "Slimes: " + Statistics.Instance.slimes + "\n";
            text += "Apple Shooters: " + Statistics.Instance.appleShooters + "\n";
            text += "Bisps: " + Statistics.Instance.bisps + "\n";
            text += "Crystal Turrets: " + Statistics.Instance.crystalTurrets + "\n";
            text += "Paddy Bums: " + Statistics.Instance.paddyBums + "\n";
            text += "Visps: " + Statistics.Instance.visps + "\n";
            text += "Snapeyes: " + Statistics.Instance.snapEyes + "\n";
            text += "\n";
            text += "Total: " + Statistics.Instance.totalEnemiesKilled + "\n";
            text += "\n";
            text += "Total Damage Taken: " + Statistics.Instance.totalDamageTaken + "\n";
            text += "Total Damage Given: " + Statistics.Instance.totalDamageGiven + "\n";
            text += "\n";
            text += "Total Distance Traveled: " + System.Math.Round(Statistics.Instance.distanceTraveled, 2) + "km\n";
            text += "Total Cash Earned: " + Statistics.Instance.moneyEarned + "\n";
            text += "\n";
            text += "Current play time: " + System.Math.Round(Statistics.Instance.TotalPlayTime, 2) + "\n";


            GUI.Label(new Rect(pos, size), text);
        }
	}
}
