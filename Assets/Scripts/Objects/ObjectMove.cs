using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour {

    public Lever lever;
    public float moveTime = 2;
    public AmountOfParticleTypes[] moveParticle;

    private bool moveDown = false;

	// Use this for initialization
	void Start () 
    {
        if (lever)
        {
            lever.OnLeverActivated += MoveDown;
        }
	}

    void Update()
    {
        if (moveDown)
        {
            if (transform.position.y > -2)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -5, transform.position.z), moveTime * Time.deltaTime);
            }
        }
    }

    void MoveDown()
    {
        SpawnEffects.EffectOnHit(moveParticle, transform.position);
        moveDown = true;
    }
}
