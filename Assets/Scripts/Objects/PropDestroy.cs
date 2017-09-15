using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDestroy : MonoBehaviour {

    public int hitAmount;
    public int maxParticleAmountPerHit;
    public GameObject[] dustEffects;
    public GameObject[] smokeEffect;
    public GameObject[] shrapnelEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hitAmount <= 0)
        {
            ChoseEffectAndAmount();
            ChoseEffectAndAmount();

            int number = Random.Range(0, shrapnelEffect.Length);
            GameObject effect = ObjectPooler.GetPooledObject(shrapnelEffect[number]);
            effect.transform.position = transform.position + (new Vector3(Random.value, Random.value, Random.value) / 2);

            //do drop
            GetComponent<Drops>().DoDrop();

            gameObject.SetActive(false);
        }
	}

    void ChoseEffectAndAmount()
    {
        int random = Random.Range(1, maxParticleAmountPerHit);
        int number = 0;
        //loop through the amount of particles number
        for (int i = 0; i < random; i++)
        {
            //spawns 1 of each particle
            number = Random.Range(0, dustEffects.Length);
            GameObject effect = ObjectPooler.GetPooledObject(dustEffects[number]);
            effect.transform.position = transform.position + (new Vector3(Random.value, Random.value, Random.value) / 2);

            number = Random.Range(0, smokeEffect.Length);
            effect = ObjectPooler.GetPooledObject(smokeEffect[number]);
            effect.transform.position = transform.position + (new Vector3(Random.value, Random.value, Random.value) / 2);
        }
    }

    public void DoEffect(Vector3 position)
    {
        ChoseEffectAndAmount();
    }
}
