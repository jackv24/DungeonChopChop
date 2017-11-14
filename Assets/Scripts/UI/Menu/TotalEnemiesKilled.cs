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
    public Text totalEnemiesText;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (transform.childCount > 0)
        {
            if (transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>().position.y < 120)
            {
                transform.GetComponent<RectTransform>().position = Vector3.Lerp(transform.GetComponent<RectTransform>().position, new Vector3(
                        transform.GetComponent<RectTransform>().position.x, 
                        transform.GetComponent<RectTransform>().position.y + 50, 
                        transform.GetComponent<RectTransform>().position.z), 2 * Time.fixedDeltaTime);
            }
        }
	}

    public IEnumerator SpawnEnemyIcons()
    {
        int enemyAmount = 1;
        foreach (EnemyKind enemy in Statistics.Instance.enemiesKilled)
        {
            GameObject sprite = new GameObject();
            sprite.AddComponent<Image>();

            if (GetIcon(enemy) != null)
                sprite.GetComponent<Image>().sprite = GetIcon(enemy);

            sprite.transform.parent = transform;

            totalEnemiesText.text = "Total Enemies Killed: " + enemyAmount;

            enemyAmount++;

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
