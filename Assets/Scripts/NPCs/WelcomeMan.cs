using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeMan : MonoBehaviour {

    public string messageAfterCollectingLoot;
    public bool dropCoins = false;
    public int cashAmount;

    [Header("Sounds and Effects")]
    public SoundEffect collectSound;
    public AmountOfParticleTypes[] collectParticle;

    private DialogueSpeaker dialogueSpeaker;
    private bool looted = false;
    private Drops drop;

	// Use this for initialization
	void Start () 
    {
        dialogueSpeaker = GetComponent<DialogueSpeaker>();
        drop = GetComponent<Drops>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!looted)
        {
            if (GameManager.Instance.enteredDungeon)
            {
                dialogueSpeaker.lines[0] = messageAfterCollectingLoot;
                looted = true;
            }
            else
            {
                if (dialogueSpeaker.CurrentPlayer)
                {
                    if (!looted)
                    {
                        if (dialogueSpeaker.CurrentPlayer.playerMove.input.Purchase)
                        {
                            if (dropCoins)
                                drop.DoDrop();
                            else
                            {
                                ItemsManager.Instance.Coins += cashAmount;
                                ItemsManager.Instance.CoinChange();

                                SoundManager.PlaySound(collectSound, transform.position);
                                SpawnEffects.EffectOnHit(collectParticle, transform.position);
                            }

                            looted = true;
                        }
                    }
                }
            }
        }
        else
        {
            dialogueSpeaker.lines[0] = messageAfterCollectingLoot;

            if (dialogueSpeaker.currentBox)
                dialogueSpeaker.currentBox.prompt.SetActive(false);
        }

        dialogueSpeaker.UpdateLines();
	}
}
