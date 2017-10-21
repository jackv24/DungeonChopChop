using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageInHeirarchy : MonoBehaviour
{
	public enum SendType { Parents, Children }
    public SendType sendType;

	public enum SendTime { OnDisable }
    public SendTime sendTime;

    public string message = "";
    public bool requireReceiver = false;
    
	public float delayBeforeSend = 1.0f;
    private float timeBeforeSend;

	void OnEnable()
	{
        timeBeforeSend = Time.time + delayBeforeSend;
    }

    void OnDisable()
	{
		if(sendTime == SendTime.OnDisable)
            Send();
    }

	void Send()
	{
        if (message.Length > 0 && Time.time > timeBeforeSend)
        {
            switch (sendType)
            {
                case SendType.Parents:
                    gameObject.SendMessageUpwards(message, requireReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                    break;
				case SendType.Children:
                    gameObject.BroadcastMessage(message, requireReceiver ? SendMessageOptions.RequireReceiver : SendMessageOptions.DontRequireReceiver);
                    break;
            }
        }
    }
}
