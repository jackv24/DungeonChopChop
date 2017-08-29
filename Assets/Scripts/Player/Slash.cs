using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour {

    [Header("Attack Values")]
    public float timeToComplete;
    public float amountOfAlpha;
    public float distance;
    public float xScaleIncrease;
    public float yScaleIncrease;
    public CharacterController cc;
    public GameObject particle;

    [Header("Camera Shake Values")]
    public float magnitude = 1;
    public float shakeAmount = 1;
    public float duration = 1;

    private int fadeInCounter = 0;

    private SpriteRenderer sprite;
    private float alpha = 0;

    private Vector3 ogScale;
    private PlayerAttack playerAttack;
    private PlayerInformation playerInfo;
    private Health playerHealth;
    public Vector3 direction;

    public GameObject player;

    bool fadeIn = false;

	// Use this for initialization
	void OnEnable () {
        fadeIn = true;
	}

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerInfo = player.GetComponent<PlayerInformation>();
        playerAttack = player.GetComponent<PlayerAttack>();
        playerHealth = player.GetComponent<Health>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (fadeIn)
        {
            //fades in the slash
            fadeInCounter++;
            alpha += amountOfAlpha;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            if (fadeInCounter > timeToComplete * 60)
            {
                fadeIn = false;
                fadeInCounter = 0;
            }
        }
        else
        {
            //fades out the slash
            alpha -= amountOfAlpha;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            if (alpha <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        //moves the slash forward
        transform.position += (direction * distance) * (1 + cc.velocity.magnitude / 9.5f);
        //increases the size of the slash
        transform.localScale += new Vector3(xScaleIncrease, yScaleIncrease, 0);
    }

    void OnCollisionEnter(Collision col)
    {
        //check if the collider is on the enemy layer
        if (col.gameObject.layer == 11)
        {
            if (col.gameObject.GetComponent<Health>())
            {
                //calculates knockback depending on direction
                col.gameObject.GetComponent<Health>().Knockback(playerInfo, direction);
                //checks if the player has a status condition
                GameObject p = Instantiate(particle, col.contacts[0].point, Quaternion.Euler(0, 0, 0)) as GameObject;
                Destroy(p, .2f);
                if (playerHealth.HasStatusCondition())
                {
                    //if the player is burned or poisoned, a charm may affect the damage output
                    if (playerHealth.isBurned || playerHealth.isPoisoned)
                    {
                        col.gameObject.GetComponent<Health>().AffectHealth((-playerInfo.strength * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit()) * playerInfo.GetCharmFloat("dmgMultiWhenBurned") * playerInfo.GetCharmFloat("dmgMultiWhenPoisoned"));
                    }
                    else
                    {
                        col.gameObject.GetComponent<Health>().AffectHealth(-playerInfo.strength * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                    }
                }
                else
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-playerInfo.strength * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                }
                CameraShake.ShakeScreen(magnitude, shakeAmount, duration);
            }
        }
    }
}
