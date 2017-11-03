using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ChallengeType
{
    KillEnemiesInTime,
    TakeNoDamage,
}

public class TileQuest : MonoBehaviour {

    public ChallengeType challengeType;

    private LevelTile levelTile;

	// Use this for initialization
	void Start () 
    {
        levelTile = GetComponentInParent<LevelTile>();

        levelTile.OnTileEnter += StartChallenge;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartChallenge()
    {
        if (challengeType == ChallengeType.KillEnemiesInTime)
            StartCoroutine(KillEnemiesInTime());
        else if (challengeType == ChallengeType.TakeNoDamage)
            StartCoroutine(TakeNoDamage());
    }

    IEnumerator KillEnemiesInTime()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator TakeNoDamage()
    {
        yield return new WaitForSeconds(1);
    }
}
