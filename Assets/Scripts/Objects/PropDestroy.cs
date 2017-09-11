using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDestroy : MonoBehaviour {

    public GameObject[] PropEffects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoEffect(Vector3 position)
    {
        int randomEffect = Random.Range(0, PropEffects.Length);
        GameObject smoke = ObjectPooler.GetPooledObject(PropEffects[randomEffect]);
        smoke.transform.position = position;

        //do drop
        GetComponent<Drops>().DoDrop();
    }
}
