using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

	float maxMoveSpeed;

	[Header("Main movement vals")]
	public float gravity = -9.8f;
	public float acceleration;
	public float rotateSpeed = 4f;

	[Header("Other vals")]
	public float inMudSpeed = 2.5f;

	private bool allowMove = true;

	private PlayerInputs input;
	private CharacterController characterController;
	private PlayerInformation playerInformation;
	private Animator animator;
	private PlayerAttack playerAttack;
    private Health playerHealth;

	private float speed;

	private Vector2 inputVector;
	private Vector3 targetMoveVector;
	private Vector3 fromMoveVector = Vector3.zero;

    private float slowdownMultiplier = 1;

	// Use this for initialization
	void Start()
	{
        playerHealth = GetComponent<Health>();
		playerAttack = GetComponent<PlayerAttack>();
		animator = GetComponentInChildren<Animator>();
		playerInformation = GetComponent<PlayerInformation>();
		if (InputManager.Instance)
		{
			input = InputManager.GetPlayerInput(playerInformation.playerIndex);
		}
		else
		{
			input = new PlayerInputs();
			input.SetupBindings();
		}
		characterController = GetComponent<CharacterController>();

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
	
	// Update is called once per frame
	void Update()
	{
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

            if (CameraFollow.Instance)
                targetMoveVector = CameraFollow.Instance.ValidateMovePos(transform.position, targetMoveVector);

            //rotate player
            if (inputVector.magnitude > 0.01f)
            {
                if (!playerAttack.blocking)
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
        }
	}

	void OnTriggerEnter(Collider col)
	{
        if (!ItemsManager.Instance.hasBoots)
        {
            if (col.tag == "Mud")
            {
                slowdownMultiplier = inMudSpeed;
            }
            if (col.tag == "Ice")
            {
                acceleration = 1;
            }
        }
	}

	void OnTriggerStay(Collider col)
	{
        if (!ItemsManager.Instance.hasBoots)
        {
            //checks if the floor is ice
            if (col.tag == "Ice")
            {
                acceleration = 1;
            }
        }
	}

	void OnTriggerExit(Collider col)
    {
        if (!ItemsManager.Instance.hasBoots)
        {
            if (col.tag == "Ice")
            {
                fromMoveVector = Vector3.zero;
                acceleration = 10f;
            }
            else if (col.tag == "Mud")
            {
                slowdownMultiplier = 1;
            }
        }
    }

}
