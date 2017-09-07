using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkState : MonoBehaviour
{
	private static List<LinkState> states = new List<LinkState>();

	public bool disableOnStart = false;
	public bool isTarget = false;

	public string identifier;

	private void Awake()
	{
		states.Add(this);
	}

	private void Start()
	{
		if(disableOnStart)
			gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		if (!isTarget)
			UpdateLinkedStates(true);
	}

	private void OnDisable()
	{
		if (!isTarget)
			UpdateLinkedStates(false);
	}

	private void OnDestroy()
	{
		states.Remove(this);
	}

	void UpdateLinkedStates(bool enabled)
	{
		List<LinkState> matchedStates = new List<LinkState>();

		foreach(var s in states)
		{
			if (s.identifier == identifier && s != this)
				matchedStates.Add(s);
		}

		foreach(LinkState state in matchedStates)
		{
			state.gameObject.SetActive(enabled);
		}
	}
}
