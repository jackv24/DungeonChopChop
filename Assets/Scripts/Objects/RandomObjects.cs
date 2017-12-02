using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjects : MonoBehaviour {

    [System.Serializable]
    public struct ObjectGroups
    {
        public GameObject[] objs;
    }

    public ObjectGroups[] props;
    public float chanceOfHiding = 50;

	// Use this for initialization
	void Start () 
    {
        foreach (ObjectGroups groups in props)
        {
            int random = Random.Range(0, 101);

            if (random < chanceOfHiding || chanceOfHiding == 100)
            {
                foreach (GameObject obj in groups.objs)
                {
                    if (obj)
                        obj.SetActive(false);
                }

            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
