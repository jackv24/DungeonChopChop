using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProOrCon
{
    None,
    MaxHealth,
    HeathAmount,
    Burn,
    Poison,
    Freeze,
    Infect,
    Sandy,
    DoubleCoins,
    DoubleKeys,
}

public class RandomProOrCon : MonoBehaviour {

    [Header("Health Amount Values")]
    public float healthAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChoseProOrCon(Collider col)
    {
        int random = Random.Range(0, System.Enum.GetNames(typeof(ProOrCon)).Length);

        PlayerInformation playerInfo = col.GetComponent<PlayerInformation>();

        switch (random)
        {
            case 0:
                break;
            case 1:
                MaxHealth(playerInfo);
                break;
            case 2:
                HealthAmount(playerInfo);
                break;
            case 3:
                Ailment(playerInfo, StatusType.burn);
                break;
            case 4:
                Ailment(playerInfo, StatusType.poison);
                break;
            case 5:
                Ailment(playerInfo, StatusType.Ice);
                break;
            case 6:
                Ailment(playerInfo, StatusType.slowlyDying);
                break;
            case 7:
                Ailment(playerInfo, StatusType.Sandy);
                break;
            case 8:
                DoubleCoins();
                break;
            case 9:
                DoubleKeys();
                break;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //player layer
        if (col.gameObject.layer == 14)
        {
            ChoseProOrCon(col);

            gameObject.SetActive(false);
        }
    }

    void MaxHealth(PlayerInformation playerInfo)
    {
        playerInfo.playerMove.playerHealth.health = playerInfo.playerMove.playerHealth.maxHealth;
    }

    void HealthAmount(PlayerInformation playerInfo)
    {
        playerInfo.playerMove.playerHealth.health += healthAmount;
    }

    void Ailment(PlayerInformation playerInfo, StatusType type)
    {
        if (type == StatusType.burn)
            playerInfo.playerMove.playerHealth.SetBurned();
        if (type == StatusType.Ice)
            playerInfo.playerMove.playerHealth.SetIce();
        if (type == StatusType.poison)
            playerInfo.playerMove.playerHealth.SetPoison();
        if (type == StatusType.slowlyDying)
            playerInfo.playerMove.playerHealth.SetSlowDeath();
        if (type == StatusType.Sandy)
            playerInfo.playerMove.playerHealth.SetSandy();
    }

    void DoubleCoins()
    {
        ItemsManager.Instance.Coins *= 2;
    }

    void DoubleKeys()
    {
        ItemsManager.Instance.Keys *= 2;
    }
}
