using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour {

    public Lever lever;
    public float moveTime = 2;
    public bool canComeBackUp = false;
    public AmountOfParticleTypes[] moveParticle;

    private Vector3 originalPosition;
    private bool moveDown = false;
    private bool moveUp = false;

	// Use this for initialization
	void Start () 
    {
        originalPosition = transform.position;

        if (lever)
        {
            lever.OnLeverActivated += MoveDown;
            if (canComeBackUp)
                lever.OnLeverDisabled += MoveUp;
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
        else if (moveUp)
        {
            if (transform.position != originalPosition)
                transform.position = Vector3.Lerp(transform.position, originalPosition, moveTime * Time.deltaTime);
        }
    }

    void MoveDown()
    {
        SpawnEffects.EffectOnHit(moveParticle, transform.position);
        moveDown = true;
        moveUp = false;
    }

    void MoveUp()
    {
        SpawnEffects.EffectOnHit(moveParticle, transform.position);
        moveUp = true;
        moveDown = false;
    }
}
