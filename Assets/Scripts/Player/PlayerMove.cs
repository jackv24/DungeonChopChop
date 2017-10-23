﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

	float maxMoveSpeed;

	[Header("Main movement vals")]
	public float gravity = -9.8f;
	public float acceleration;
	public float rotateSpeed = 4f;
    public bool LockY;

	[Header("Environment vals")]
    [Header("Desert")]
    public float inMudSpeed = .7f;
    [Header("Fire")]
    [Tooltip("Little lava spots, the speed damper, set value not multiplier")]
    public float inLavaSpeed = .7f;
    [Tooltip("The amount of damage you take just being in the biome without crest")]
    public float damageInFireBiome = .1f;
    [Tooltip("The multiplier to the above value, when in lava puddle")]
    public float damageInFireMultiplier = 5;
    [Tooltip("The time between tick damage in biome")]
    public float timeBetweenBiomeBurn = 2;
    [Tooltip("The divider of the above value, when in lava puddle")]
    public float timeBetweenTickDivider = 1.5f;
    [Header("Ice")]
    public float windSpeed = 2;
    public float inSnowSpeed = .7f;
    public float iceAcceleration = 1;

	private bool allowMove = true;

    [Space()]
    public LayerMask layerMask;

    [HideInInspector]
	public PlayerInputs input;
	private CharacterController characterController;
	private PlayerInformation playerInformation;
	private Animator animator;
	private PlayerAttack playerAttack;
    [HideInInspector]
    public Health playerHealth;

	private Vector2 inputVector;
	private Vector3 targetMoveVector;
	private Vector3 fromMoveVector = Vector3.zero;

    private float speed;
    private float slowdownMultiplier = 1;
    private float fireBiomeTickCounter = 0;

    private float ogTickDamageInFire;
    private float ogTimeBetweenTickInFire;

	// Use this for initialization
	void Start()
	{
        ogTickDamageInFire = damageInFireBiome;
        ogTimeBetweenTickInFire = timeBetweenBiomeBurn;
        
        playerHealth = GetComponent<Health>();
		playerAttack = GetComponent<PlayerAttack>();
		animator = GetComponentInChildren<Animator>();
		playerInformation = GetComponent<PlayerInformation>();
		
		characterController = GetComponent<CharacterController>();

        input = new PlayerInputs();

        if (playerInformation.playerIndex == 0)
        {
            input.AddKeyboardBindings();
            if (GameManager.Instance.players.Count > 1)
            {
                if (InControl.InputManager.Devices.Count > 1)
                {
                    input.AssignDevice(InControl.InputManager.Devices[0]);
                    input.SetupBindings();
                }
            } 
            else 
            {
                if (InControl.InputManager.Devices.Count == 1)
                {
                    input.AssignDevice(InControl.InputManager.Devices[0]);
                    input.SetupBindings();
                }
                else
                {
                    input.SetupBindings();
                }
            }
        } 
        else if (playerInformation.playerIndex == 1)
        {
            if (InControl.InputManager.Devices.Count <= 1)
            {
                input.AssignDevice(InControl.InputManager.Devices[0]);
                input.AddControllerBindings();
            } 
            else 
            {
                input.AssignDevice(InControl.InputManager.Devices[1]);
                input.SetupBindings();
            }
        }

        //Wait until level generation is done to allow movement
        if (LevelGenerator.Instance)
		{
			allowMove = false;

			LevelGenerator.Instance.OnGenerationFinished += delegate
			{
				allowMove = true;
			};
		}
	}

	void doAnimations()
	{
        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Blocking"))
        {
            //float speed = targetMoveVector.magnitude / maxMoveSpeed * shield damping;
            animator.SetFloat("move", characterController.velocity.magnitude / maxMoveSpeed * playerAttack.shield.speedDamping);
			
        }
        else if (animator.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
        {
            animator.SetFloat("move", characterController.velocity.magnitude / maxMoveSpeed * playerAttack.multiSpeedMultiplier);
        }
        else
		{
            animator.SetFloat("move", characterController.velocity.magnitude / maxMoveSpeed);
		}
	}

    Vector3 WindPush()
    {
        if (!ItemsManager.Instance.hasBoots)
        {
            if (LevelGenerator.Instance.currentTile && LevelGenerator.Instance.currentTile.GetComponent<TileParticles>())
            {
                if (LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Desert || LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Ice)
                {
                    if (LevelGenerator.Instance.currentTile.GetComponent<TileParticles>().HasParticles)
                        return LevelGenerator.Instance.currentTile.transform.forward * windSpeed;
                    else
                        return Vector3.zero;
                }
            }
        }

        return Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!ItemsManager.Instance.hasArmourPiece)
        {
            if (LevelGenerator.Instance.currentTile)
            {
                if (LevelGenerator.Instance.currentTile.Biome == LevelTile.Biomes.Fire)
                {
                    fireBiomeTickCounter++;
                    if (fireBiomeTickCounter > (timeBetweenBiomeBurn * 60))
                    {
                        playerHealth.AffectHealth(-damageInFireBiome);
                        fireBiomeTickCounter = 0;
                    }
                }
            }
        }
    }

    void RotatesUsingControls()
    {
        if (!playerInformation.HasCharmBool("inverted"))
        {
            //transform.rotation = Quaternion.LookRotation(new Vector3(inputVector.x, 0, inputVector.y));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(inputVector.x, 0, inputVector.y)), rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(-inputVector.x, 0, -inputVector.y)), rotateSpeed * Time.deltaTime);
        }
    }
	
	// Update is called once per frame
	void Update()
	{
        if (LockY)
        {
            if (transform.position.y > .5f)
            {
                transform.position = new Vector3(transform.position.x, .14f, transform.position.z);
            }
        }

		doAnimations();

		if (!allowMove)
			return;

        if (!playerHealth.isDead)
        {
            //sets movespeed to playerinformation movespeed;
            maxMoveSpeed = playerInformation.maxMoveSpeed;

            //sets the input vector
            inputVector = input.Move;

            if (inputVector.magnitude > 1)
                inputVector.Normalize();

            if (!playerInformation.HasCharmBool("inverted"))
            {
                targetMoveVector.x = inputVector.x * maxMoveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier") * playerInformation.GetItemFloat("speedMultiplier") * slowdownMultiplier;
                targetMoveVector.z = inputVector.y * maxMoveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier") * playerInformation.GetItemFloat("speedMultiplier") * slowdownMultiplier;
            }
            else
            {
                targetMoveVector.x = -inputVector.x * maxMoveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier") * playerInformation.GetItemFloat("speedMultiplier") * slowdownMultiplier;
                targetMoveVector.z = -inputVector.y * maxMoveSpeed * playerInformation.GetCharmFloat("moveSpeedMultiplier") * playerInformation.GetItemFloat("speedMultiplier") * slowdownMultiplier;
            }

            targetMoveVector += WindPush();

            if (CameraFollow.Instance)
                targetMoveVector = CameraFollow.Instance.ValidateMovePos(transform.position, targetMoveVector);

            //rotate player
            if (inputVector.magnitude > 0.01f)
            {
                if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("Blocking"))
                {
                    RotatesUsingControls();
                }  
                else 
                {
                    if (!playerAttack.autoBlock)
                    {
                        RotatesUsingControls();
                    }
                }
            }
            //checks to see if the user is grounded, if not apply gravity
            if (!characterController.isGrounded)
            {
                targetMoveVector.y += gravity * Time.deltaTime;
            }
			
            //moves the player using the input axis and move speed
            //checks if the player is on ice or not
            if (!playerHealth.isBurned)
            {
                fromMoveVector = Vector3.Lerp(fromMoveVector, targetMoveVector, acceleration * playerInformation.GetCharmFloat("slipMultiplier") * Time.deltaTime);
                characterController.Move(fromMoveVector * Time.deltaTime);
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 50, layerMask))
            {
                if (!ItemsManager.Instance.hasBoots)
                {
                    if (hit.collider.tag == "Mud" || hit.collider.tag == "Lava" || hit.collider.tag == "Snow")
                    {
                        //if in mud, slown down
                        if (hit.collider.tag == "Mud")
                        {
                            slowdownMultiplier = inMudSpeed;
                        }
                        if (hit.collider.tag == "Snow")
                        {
                            slowdownMultiplier = inSnowSpeed;
                        }
                        if (hit.collider.tag == "Lava")
                        {
                            slowdownMultiplier = inLavaSpeed;
                        }
                    }
                    else
                    {
                        slowdownMultiplier = 1;
                    }

                    //if on ice, slip
                    if (hit.collider.tag == "Ice")
                    {
                        acceleration = iceAcceleration;
                    }
                    else
                    {
                        acceleration = 10f;
                    }
                }
                else
                {
                    acceleration = 10f;
                    slowdownMultiplier = 1;
                }


                if (!ItemsManager.Instance.hasArmourPiece)
                {
                    //sets the in lava puddle vars
                    if (hit.collider.tag == "Lava")
                    {
                        damageInFireBiome *= damageInFireMultiplier;
                        timeBetweenBiomeBurn /= timeBetweenTickDivider;
                    }
                    else
                    {
                        damageInFireBiome = ogTickDamageInFire;
                        timeBetweenBiomeBurn = ogTimeBetweenTickInFire;
                    }
                }
            }
        }
	}
}
