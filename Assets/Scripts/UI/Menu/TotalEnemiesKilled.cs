using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyIcon
{
    public EnemyType enemyType;
    public Sprite enemyIcon;
}

public class TotalEnemiesKilled : MonoBehaviour {

    public EnemyIcon[] enemyIcons;
    public float timeBetweenIconSpawn = .1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnEnemyIcons());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator SpawnEnemyIcons()
    {
        foreach (EnemyType enemy in Statistics.Instance.enemiesKilled)
        {
            if (enemy == EnemyType.AppleShooter)
            {
                GameObject sprite = new GameObject();
                sprite.AddComponent<Image>();

                if (GetIcon(enemy) != null)
                    sprite.GetComponent<Image>().sprite = GetIcon(enemy);

                sprite.transform.parent = transform;

                yield return new WaitForSeconds(timeBetweenIconSpawn);
            }
        }
    }

    Sprite GetIcon(EnemyType type)
    {
        //loop through the icons and get the correct one depending on type
        foreach (EnemyIcon enemyIcon in enemyIcons)
        {
            if (enemyIcon.enemyType == type)
                return enemyIcon.enemyIcon;
        }

        return null;
    }
}
