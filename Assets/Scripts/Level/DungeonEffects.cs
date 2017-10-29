using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DungEffType
{
    None,
    HiddenHealth,
    ExtremePower,
    NoMap
}

[System.Serializable]
public class DE
{
    public DungEffType effectType;
    public int index;
    public bool canHappen;
    [Header("Dungeon Annoucement Text")]
    public string Header;
    public string Description;
}

public class DungeonEffects : MonoBehaviour {

    [Tooltip("The chance of an effect actually happening, 0 - 100")]
    public int chanceOfEffect = 25;
    public float announcementDelayTime;

    [Header("Dungeon Effects")]
    public DE[] effects;

    [Header("Hidden Health Values")]
    public GameObject[] playersUI;

    [Header("Extreme Power Values")]
    public float strengthMultiplier = 2f;
    private List<float> originalStrength;

    [Header("No Map Values")]
    public GameObject map;

    private bool effectOn = false;
    private DE currentEffect;

	// Update is called once per frame
	void Update () {
        LevelGenerator.Instance.OnGenerationFinished += DungeonEffect;
	}

    void DungeonEffect()
    {
        //check if the players are in a dungeon
        if (LevelGenerator.Instance.profile is DungeonGeneratorProfile)
        {
            if (!effectOn)
            {
                //do the random chance of doing the effect
                int random = Random.Range(0, 101);

                if (random <= chanceOfEffect)
                {
                    ChoseEffect();
                }
            }
        }
        //check if the players arent in a dungeon and if they were, reset there stats
        else if (LevelGenerator.Instance.profile is OverworldGeneratorProfile)
        {
            if (effectOn)
            {
                RevertEffect();
            }
        }
    }

    void ChoseEffect()
    {
        effectOn = true;

        int random = Random.Range(0, effects.Length);

        foreach (DE effect in effects)
        {
            if (effect.index == random)
            {
                currentEffect = effect;
                DoEffect(effect);
            }
        }
    }

    void DoEffect(DE effect)
    {
        if (effect.canHappen)
        {
            //check what effect it is and do that effect
            if (effect.effectType == DungEffType.HiddenHealth)
                DoHiddenHealthEffect();
            else if (effect.effectType == DungEffType.ExtremePower)
                DoExtremePowerEffect();
            else if (effect.effectType == DungEffType.NoMap)
                DoNoMapEffect();

            if (effectOn)
                AnnounceEffectOn();
        }
    }

    void RevertEffect()
    {
        effectOn = false;

        DoEffect(currentEffect);
    }

    void AnnounceEffectOn()
    {
        Announcement.DoAnnouncement(currentEffect.Header, currentEffect.Description, announcementDelayTime);
    }

    void AnnounceEffectOff()
    {
        //do the text saying the effect is gone
    }

    void DoHiddenHealthEffect()
    {
        if (effectOn)
        {
            foreach (GameObject ui in playersUI)
            {
                ui.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject ui in playersUI)
            {
                ui.SetActive(true);
            }
        }
    }

    void DoExtremePowerEffect()
    {
        if (effectOn)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.strength *= strengthMultiplier;
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.strength /= strengthMultiplier;
            }
        }
    }

    void DoNoMapEffect()
    {
        if (effectOn)
        {
            map.SetActive(false);
        }
        else
        {
            map.SetActive(true);
        }
    }
}
