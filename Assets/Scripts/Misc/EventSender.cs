using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour
{
    public delegate void NormalEvent();
    public event NormalEvent OnEnableEvent;
	public event NormalEvent OnDisableEvent;

    public bool disableOnLevelClear = false;

	void OnEnable()
	{
		if(disableOnLevelClear && LevelGenerator.Instance)
		{
			if(!LevelGenerator.Instance.IsFinished)
                return;
        }

		if(OnEnableEvent != null)
            OnEnableEvent();
    }

	void OnDisable()
    {
		if (disableOnLevelClear && LevelGenerator.Instance)
        {
            if (!LevelGenerator.Instance.IsFinished)
                return;
        }

        if (OnDisableEvent != null)
            OnDisableEvent();
    }

	public void SendDisabledEvent()
	{
		if (OnDisableEvent != null)
            OnDisableEvent();
	}
}
