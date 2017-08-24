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
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (fadeIn)
        {
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
            alpha -= amountOfAlpha;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            if (alpha <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        transform.position += (direction * distance) * (1 + cc.velocity.magnitude / 9.5f);
        transform.localScale += new Vector3(xScaleIncrease, yScaleIncrease, 0);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 11)
        {
            if (col.GetComponent<Health>())
            {
                Vector3 dir = transform.position - col.transform.position;
                float dist = Vector3.Distance(transform.position, col.transform.position);
                col.GetComponent<Health>().Knockback(playerInfo, -dir, dist);
                col.GetComponent<Health>().AffectHealth(-playerInfo.strength * playerInfo.GetCharmFloat("strengthMultiplier") * playerAttack.criticalHit());
                CameraShake.ShakeScreen(magnitude, shakeAmount, duration);
            }
        }
    }
}
