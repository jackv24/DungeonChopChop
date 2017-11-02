using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStatistics : MonoBehaviour {

    public Vector2 size;
    public Vector2 pos;
    public GUIStyle style;

    private RectTransform rect;
    private int fontSize;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        fontSize = style.fontSize;
    }
	
	// Update is called once per frame
	void OnGUI () {

        if (enabled)
        {
            string text = "";

            Vector2 p = new Vector2(rect.position.x - pos.x, rect.position.y - pos.y);

            style.fontSize = Screen.currentResolution.width / fontSize;

            string time = string.Format("{0}:{1:00}", (int)Statistics.Instance.TotalPlayTime / 60, (int)Statistics.Instance.TotalPlayTime % 60);

            text += "Enemies Killed\n";
            text += "Slimes: " + Statistics.Instance.slimes + "\n";
            text += "Apple Shooters: " + Statistics.Instance.appleShooters + "\n";
            text += "Bisps: " + Statistics.Instance.bisps + "\n";
            text += "Crystal Turrets: " + Statistics.Instance.crystalTurrets + "\n";
            text += "Paddy Bums: " + Statistics.Instance.paddyBums + "\n";
            text += "Visps: " + Statistics.Instance.visps + "\n";
            text += "Snapeyes: " + Statistics.Instance.snapEyes + "\n";
            text += "MushBooms: " + Statistics.Instance.mushBooms + "\n";
            text += "\n";
            text += "Total: " + Statistics.Instance.totalEnemiesKilled + "\n";
            text += "\n";
            text += "Total Damage Taken: " + Statistics.Instance.totalDamageTaken + "\n";
            text += "Total Damage Given: " + Statistics.Instance.totalDamageGiven + "\n";
            text += "\n";
            text += "Total Distance Traveled: " + (System.Math.Round(Statistics.Instance.distanceTraveled, 2) / 10) + "m\n";
            text += "Total Cash Earned: " + Statistics.Instance.moneyEarned + "\n";
            text += "\n";
            text += "Current play time: " + time + "\n";


            GUI.Label(new Rect(p, size), text, style);
        }
	}
}
