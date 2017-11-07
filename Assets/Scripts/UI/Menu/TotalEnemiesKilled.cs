using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyIcon
{
    public EnemyKind enemyKind;
    public Sprite enemyIcon;
}

public class TotalEnemiesKilled : MonoBehaviour {

    public EnemyIcon[] enemyIcons;
    public float timeBetweenIconSpawn = .1f;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator SpawnEnemyIcons()
    {
        foreach (EnemyKind enemy in Statistics.Instance.enemiesKilled)
        {
            GameObject sprite = new GameObject();
            sprite.AddComponent<Image>();

            if (GetIcon(enemy) != null)
                sprite.GetComponent<Image>().sprite = GetIcon(enemy);

            sprite.transform.parent = transform;

            yield return new WaitForSeconds(timeBetweenIconSpawn);
        }
    }

    Sprite GetIcon(EnemyKind type)
    {
        //loop through the icons and get the correct one depending on type
        foreach (EnemyIcon enemyIcon in enemyIcons)
        {
            if (enemyIcon.enemyKind.enemyType == type.enemyType && enemyIcon.enemyKind.enemyBiome == type.enemyBiome)
                return enemyIcon.enemyIcon;
        }

        return null;
    }
}
