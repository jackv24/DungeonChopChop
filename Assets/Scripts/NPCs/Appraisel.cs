using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appraisel : MonoBehaviour {

    public float timeBetweenCharmAppraisel;

    private DialogueSpeaker dialogueSpeaker;
    private bool appraising = false;
    private string originalSpeach;

	// Use this for initialization
	void Start () 
    {
        dialogueSpeaker = GetComponent<DialogueSpeaker>();
        originalSpeach = dialogueSpeaker.lines[0];
	}
	
	// Update is called once per frame
	void Update () {
        
        if (!appraising)
        {
            if (dialogueSpeaker.CurrentPlayer)
            {
                if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase)
                {
                    StartCoroutine(AppraiseCharms());
                }
            }
        }
    }

    void WipeLines()
    {
        dialogueSpeaker.lines[0] = "";
    }

    IEnumerator AppraiseCharms()
    {
        string text = "";

        appraising = true;

        WipeLines();

        if (dialogueSpeaker.CurrentPlayer.currentCharms.Count > 0)
        {
            foreach (Charm charm in dialogueSpeaker.CurrentPlayer.currentCharms)
            {
                dialogueSpeaker.lines[0] = "Your current Charm '" + charm.displayName + "'s' ability is\n" + charm.itemInfo;
                dialogueSpeaker.UpdateLines();

                yield return new WaitForSeconds(timeBetweenCharmAppraisel);

                WipeLines();
            }
        }
        else
        {
            dialogueSpeaker.lines[0] = "Come back to me when you have a charm lad";
            dialogueSpeaker.UpdateLines();
            yield return new WaitForSeconds(2);
        }

        appraising = false;

        WipeLines();

        dialogueSpeaker.lines[0] = originalSpeach;
        dialogueSpeaker.UpdateLines();

    }

       
}
