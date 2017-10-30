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
}

[System.Serializable]
public class DE
{
    public DungEffType effectType;
    public bool canHappen;
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

    [Space()]
    public DE[] effects;

    [Header("Hidden Health Values")]
    public GameObject[] playersUI;

    [Header("Extreme Power Values")]
    public float strengthMultiplier = 2f;
    private List<float> originalStrength;

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

    private bool effectOn = false;
    private bool dungeonDoofDoof = false;

    private List<bool> specialAtkBools = new List<bool>(0);
    private List<GameObject> partyLights = new List<GameObject>(0);

    private float originalDungeonLighting;
    private float originalFOV;
    private float originalAccel;

    private DE currentEffect;

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
            switch (effect.effectType)
            {
                case DungEffType.HiddenHealth:
                    DoHiddenHealthEffect();
                    break;
                case DungEffType.ExtremePower:
                    DoExtremePowerEffect();
                    break;
                case DungEffType.NoMap:
                    DoNoMapEffect();
                    break;
                case DungEffType.NoSpecialAttacks:
                    DoSpecialAttacks();
                    break;
                case DungEffType.DarkDungeon:
                    DoDarkDungeon();
                    break;
                case DungEffType.DungeonDoofDoof:
                    DoDungeonDoof();
                    break;
                case DungEffType.WorkOut:
                    DoWorkOut();
                    break;
                case DungEffType.FineDetails:
                    DoFineDetails();
                    break;
                case DungEffType.CleanFloors:
                    DoCleanFloors();
                    break;
            }

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
            Camera.main.fieldOfView = cameraFOV;
    }

    void DoCleanFloors()
    {
        if (effectOn)
        {
            originalAccel = GameManager.Instance.players[0].playerMove.acceleration;

            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.playerMove.acceleration = acceleration;
            }
        }
        else
        {
            foreach (PlayerInformation player in GameManager.Instance.players)
            {
                player.playerMove.acceleration = originalAccel;
            }
        }
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
