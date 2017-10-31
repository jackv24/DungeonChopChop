using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DungEffType
{
    None,
    HiddenHealth,
    ExtremePower,
    NoMap,
    NoSpecialAttacks,
    DarkDungeon,
    DungeonDoofDoof,
    WorkOut,
    FineDetails,
    CleanFloors,
    InstaKill,
    BigSwords,
    MoreMoney,
}

[System.Serializable]
public class DE
{
    public DungEffType effectType;
    public bool canHappen = true;
    [Header("Dungeon Annoucement Text")]
    public string Header;
    public string Description;
}

public class DungeonEffects : MonoBehaviour {

    [Header("Dungeon Effect Values")]
    [Tooltip("The chance of an effect actually happening, 0 - 100")]
    public int chanceOfEffect = 25;
    public float announcementDelayTime;
    public bool effectsReoccur = false;

    [Header("For testing purposes, overrides and only choses this effect")]
    public DungEffType overrider;

    [Space()]
    public DE[] effects;

    [Header("Hidden Health Values")]
    public GameObject[] playersUI;

    [Header("Extreme Power Values")]
    public float strengthMultiplier = 2f;

    [Header("No Map Values")]
    public GameObject map;

    [Header("No Special Attacks Values")]

    [Header("Darker Dungeon Values")]
    public float ambientIntensity = .20f;

    [Header("Dungeon Doof Doof Values")]
    public GameObject partyLight;
    public int minLightingAmount;
    public int maxLightingAmount;
    public float lightsOnAndOffTime = 1;

    [Header("Work Out Values")]
    public float workOutSpeedMultiplier;

    [Header("Fine Details Values")]
    public float cameraFOV = 35;

    [Header("Clean Floors Values")]
    public float acceleration = 1;

    [Header("Insta Kill Values")]
    public float instaKillStrength = 1000;

    [Header("Big Swords Values")]
    public float swordScaleMultiplier = 3;

    [Header("More Money Values")]
    public int percentageMore = 25;

    private bool effectOn = false;
    private bool dungeonDoofDoof = false;

    private List<bool> specialAtkBools = new List<bool>(0);
    private List<GameObject> partyLights = new List<GameObject>(0);
    private Dictionary<string, Vector3> weaponScales = new Dictionary<string, Vector3>(0);

    private float originalDungeonLighting;
    private float originalFOV;
    private float originalAccel;

    private DE currentEffect;
    private bool doneEffect = false;

    void Start()
    {
        originalFOV = Camera.main.fieldOfView;
    }

	// Update is called once per frame
	void Update () {
        LevelGenerator.Instance.OnGenerationFinished += DungeonEffect;
        //LevelGenerator.Instance.OnTileEnter += SpawnPartyLights;
	}

    void DungeonEffect()
    {
        //check if the players are in a dungeon
        if (LevelGenerator.Instance.profile is DungeonGeneratorProfile)
        {         
            if (!doneEffect)
            {
                if (overrider == DungEffType.None)
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
                else
                {
                    effectOn = true;
                    DE newEffect = new DE();
                    newEffect.effectType = overrider;
                    currentEffect = newEffect;

                    DoEffect(newEffect);
                }

                doneEffect = true;
            }
        }
        //check if the players arent in a dungeon and if they were, reset there stats
        else if (LevelGenerator.Instance.profile is OverworldGeneratorProfile)
        {
            if (effectOn)
            {
                RevertEffect();

                doneEffect = false;
            }
        }
    }

    void ChoseEffect()
    {
        effectOn = true;

        int random = Random.Range(0, effects.Length);

        for (int i = 0; i < effects.Length; i++)
        {
            if (i == random)
            {
                currentEffect = effects[i];
                DoEffect(effects[i]);
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
            else if (effect.effectType == DungEffType.NoSpecialAttacks)
                    DoSpecialAttacks();
            else if (effect.effectType == DungEffType.DarkDungeon)
                    DoDarkDungeon();
            else if (effect.effectType == DungEffType.DungeonDoofDoof)
                    DoDungeonDoof();
            else if (effect.effectType == DungEffType.WorkOut)
                    DoWorkOut();
            else if (effect.effectType == DungEffType.FineDetails)
                    DoFineDetails();
            else if (effect.effectType == DungEffType.CleanFloors)
                    DoCleanFloors();
            else if (effect.effectType == DungEffType.InstaKill)
                    DoInstaKill();
            else if (effect.effectType == DungEffType.BigSwords)
                    DoBigSwords();
            else if (effect.effectType == DungEffType.MoreMoney)
                    DoMoreMoney();

            if (effectOn)
                AnnounceEffectOn();
            else 
            {
                if (!effectsReoccur)
                    effect.canHappen = false;
            }
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

    void DoSpecialAttacks()
    {
        if (effectOn)
        {
            //get all the current bools of the player, then disable them
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                //both players have the same bools, so we can just add the first players to the list
                if (player.playerIndex == 0)
                {
                    specialAtkBools.Add(player.playerAttack.canDash);
                    specialAtkBools.Add(player.playerAttack.canDashAttack);
                    specialAtkBools.Add(player.playerAttack.canSpinAttack);
                    specialAtkBools.Add(player.playerAttack.canTripleAttack);
                }

                //set the players attacks to false
                player.playerAttack.canDash = false;
                player.playerAttack.canDashAttack = false;
                player.playerAttack.canSpinAttack = false;
                player.playerAttack.canTripleAttack = false;
            }
        }
        else
        {
            //loop through and reset their attacks
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.playerAttack.canDash = specialAtkBools[0];
                player.playerAttack.canDashAttack = specialAtkBools[1];
                player.playerAttack.canSpinAttack = specialAtkBools[2];
                player.playerAttack.canTripleAttack = specialAtkBools[3];
            }
        }
    }

    void DoDarkDungeon()
    {
        BiomeLighting lighting = GameObject.FindObjectOfType<BiomeLighting>();

        if (lighting)
        {
            if (effectOn)
            {
                originalDungeonLighting = lighting.dungeonProfile.ambientIntensity;

                lighting.dungeonProfile.ambientIntensity = ambientIntensity;

                lighting.UpdateLighting();
            }
            else
            {
                lighting.dungeonProfile.ambientIntensity = originalDungeonLighting;

                lighting.UpdateLighting();
            }
        }
    }

    void DoDungeonDoof()
    {
        if (effectOn)
            dungeonDoofDoof = true;
        else
            dungeonDoofDoof = false;
    }

    void DoWorkOut()
    {
        if (effectOn)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.maxMoveSpeed *= 2;
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.maxMoveSpeed /= 2;
            }
        }
    }

    void DoFineDetails()
    {
        if (effectOn)
            Camera.main.fieldOfView = cameraFOV;
        else
            Camera.main.fieldOfView = originalFOV;
    }

    void DoCleanFloors()
    {
        if (effectOn)
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.playerMove.acceleration = acceleration;
                player.playerMove.slipOverride = true;
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.playerMove.acceleration = originalAccel;
                player.playerMove.slipOverride = false;
            }
        }
    }

    void DoInstaKill()
    {
        if (effectOn)
        {             
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.strength *= instaKillStrength;
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.strength /= instaKillStrength;
            }
        }
    }

    void DoBigSwords()
    {
        if (effectOn)
        {        
            weaponScales.Clear();

            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                if (!weaponScales.ContainsKey(player.playerAttack.sword.name + player.playerIndex))
                {
                    weaponScales.Add(player.playerAttack.sword.name + player.playerIndex, player.playerAttack.sword.transform.lossyScale);

                    player.playerAttack.sword.transform.localScale *= swordScaleMultiplier;
                }
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                if (weaponScales.ContainsKey(player.playerAttack.sword.name + player.playerIndex))
                {
                    player.playerAttack.sword.transform.localScale /= swordScaleMultiplier;
                }
            }
        }
    }

    void DoMoreMoney()
    {
        if (effectOn)
            ItemsManager.Instance.itemDropMultiplier += percentageMore;
        else
            ItemsManager.Instance.itemDropMultiplier -= percentageMore;
    }

    void SpawnPartyLights()
    {
        if (dungeonDoofDoof)
        {
            if (partyLights.Count > 0)
            {
                foreach (GameObject l in partyLights)
                {
                    l.gameObject.SetActive(false);
                }
            }

            partyLights.Clear();

            //get the amount of lights to spawn
            int lightsAmount = Random.Range(minLightingAmount, maxLightingAmount);

            for (int i = 0; i < lightsAmount; i++)
            {
                GameObject light = ObjectPooler.GetPooledObject(partyLight);

                Vector3 pos = new Vector3(LevelGenerator.Instance.currentTile.transform.position.x + Random.Range(0, 30), 0, LevelGenerator.Instance.currentTile.transform.position.z + Random.Range(0, 30));

                light.transform.position = pos;

                light.GetComponent<PartyLight>().lightOnAndOffTime = lightsOnAndOffTime;

                partyLights.Add(light);
            }
        }
    }
}
