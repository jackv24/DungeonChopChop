using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkState : MonoBehaviour
{
	public bool isTarget = false;

	public string identifier;

	private void OnEnable()
	{
		if (isTarget)
			return;

		UpdateLinkedStates(true);
	}

	private void OnDisable()
	{
		if (isTarget)
			return;

		UpdateLinkedStates(false);
	}

	void UpdateLinkedStates(bool enabled)
	{
		LinkState[] foundStates = FindObjectsOfType<LinkState>();

		List<LinkState> matchedStates = new List<LinkState>();

		foreach(LinkState state in foundStates)
		{
			if (state.identifier == identifier)
				matchedStates.Add(state);
		}

		foreach(LinkState state in matchedStates)
		{
			state.gameObject.SetActive(enabled);
		}
	}
}
