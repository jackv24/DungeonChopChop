﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow Instance;

	[Header("Positioning")]
    public List<Transform> players = new List<Transform>();
	private List<Health> playersHealth = new List<Health>();

	[Space()]
	public float height = 15.0f;
	public float zOffset = -6.5f;

    [Space()]
    public float followSpeed = 10.0f;

	[Header("Player Limiting")]
	public float maxDistanceX = 13.5f;
	public float maxDistanceBack = 10.0f;
	public float maxDistanceFront = 9.0f;

	private Vector3 max = Vector3.zero;
	private Vector3 min = Vector3.zero;

	[Header("Tile Limits")]
	public float paddingTop = 2.0f;
	public float paddingBottom = 2.0f;
	public float paddingLeft = 2.0f;
	public float paddingRight = 2.0f;

	public Vector3 targetPos;

	private bool resetCamera = true;

	private AudioListener listener;
	private GameObject listenerObj;

	void Awake()
	{
		Instance = this;

		listener = GetComponent<AudioListener>();
	}

	void Start()
	{
		//Move audio listener to child object
		if(listener)
		{
			Destroy(listener);

			listenerObj = new GameObject("Audio Listener");
			listenerObj.transform.SetParent(transform);
			listenerObj.transform.rotation = Quaternion.identity;
			listenerObj.AddComponent<AudioListener>();
		}

		//Find all players
		PlayerInformation[] playerInfos = FindObjectsOfType<PlayerInformation>();

		foreach (PlayerInformation p in playerInfos)
		{
			players.Add(p.transform);
			playersHealth.Add(p.GetComponent<Health>());
		}

		if(LevelGenerator.Instance)
		{
			LevelGenerator.Instance.OnGenerationFinished += delegate
			{
				resetCamera = true;
			};
		}
	}

	void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	void LateUpdate()
    {
		targetPos = Vector3.zero;

		//Calculate average position between players
		if (players.Count > 0)
		{
			int numPlayers = 0;

			for(int i = 0; i < Mathf.Min(players.Count, playersHealth.Count); i++)
			{
				if (playersHealth[i].health > 0)
				{
					targetPos += players[i].position;
					numPlayers++;
				}
			}

			targetPos /= numPlayers;
		}

		if (listenerObj)
			listenerObj.transform.position = targetPos;

		///Clamp to tile bounds
		//Only clamp if max is more than min
		if (max != Vector3.zero && min != Vector3.zero)
		{
			if (max.x - paddingRight > min.x + paddingLeft)
				targetPos.x = Mathf.Clamp(targetPos.x, min.x + paddingLeft, max.x - paddingRight);
			else //Else centre
				targetPos.x = (min.x + max.x) / 2;

			if (max.z - paddingTop > min.z + paddingBottom)
				targetPos.z = Mathf.Clamp(targetPos.z, min.z + paddingBottom, max.z - paddingTop);
			else
				targetPos.z = (min.z + max.z) / 2;
		}

		Vector3 camPos = targetPos;

		//Move camera to correct offset
		camPos += Vector3.up * height;
        camPos += Vector3.forward * zOffset;

		if (resetCamera)
		{
			resetCamera = false;

			transform.position = camPos;
		}
		else
			transform.position = Vector3.Lerp(transform.position, camPos, followSpeed * Time.deltaTime);
    }

	public void UpdateCameraBounds(Bounds tileBounds)
	{
		min = tileBounds.min;
		max = tileBounds.max;

		resetCamera = true;
	}

	/// <summary>
	/// Clamps movement to within camera move area.
	/// </summary>
	/// <param name="pos">The position of the player.</param>
	/// <param name="move">The movement vector of the player.</param>
	/// <returns>The clamped movement vector.</returns>
	public Vector3 ValidateMovePos(Vector3 pos, Vector3 move)
	{
		Vector3 min = targetPos - new Vector3(maxDistanceX, 0, maxDistanceFront);
		Vector3 max = targetPos + new Vector3(maxDistanceX, 0, maxDistanceBack);

		Vector3 delta = move * Time.deltaTime;

		if (move.x > 0 && pos.x + delta.x > max.x)
			move.x = 0;
		else if (move.x < 0 && pos.x - delta.x < min.x)
			move.x = 0;

		if (move.z > 0 && pos.z + delta.z > max.z)
			move.z = 0;
		else if (move.z < 0 && pos.z - delta.z < min.z)
			move.z = 0;

		return move;
	}

	void OnDrawGizmosSelected()
	{
		Vector3 backLeft = targetPos + new Vector3(-maxDistanceX, 0, maxDistanceBack);
		Vector3 backRight = targetPos + new Vector3(maxDistanceX, 0, maxDistanceBack);
		Vector3 frontLeft = targetPos + new Vector3(-maxDistanceX, 0, -maxDistanceFront);
		Vector3 frontRight = targetPos + new Vector3(maxDistanceX, 0, -maxDistanceFront);

		Gizmos.DrawLine(backLeft, backRight);
		Gizmos.DrawLine(frontLeft, frontRight);

		Gizmos.DrawLine(backLeft, frontLeft);
		Gizmos.DrawLine(backRight, frontRight);
	}
}
