using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
	public delegate void PlayerGet(PlayerInformation playerInfo, bool value);
	public event PlayerGet OnGetPlayer;

    public delegate void PlayerEvent(PlayerInformation playerInfo);
    public event PlayerEvent OnOpen;

    [Multiline]
	public string[] lines = { "Default text" };
	private int lastIndex = -1;

	[Space()]
	public LayerMask playerLayer;
	public float textBoxHeight = 3.0f;
	public float textBoxDepth = 0.0f;

	[Space()]
	public GameObject dialogueBoxPrefab;

    public PlayerInformation CurrentPlayer { get { return currentBox ? playerInfo : null; } }

    [HideInInspector]
    public DialogueBox currentBox;
	private PlayerInformation playerInfo;

	private List<GameObject> objs = new List<GameObject>();
	private bool allowEntering = false;

    public bool IsOpen { get { return currentBox != null; } }

	void Start()
	{
		//Delay detection of players entering trigger until the next frame
		StartCoroutine(DelayEntering());
	}

	//Does not need to happen every frame
	void FixedUpdate()
    {
        //if colliders were found, player is in range
        if (objs.Count > 0)
        {
            //if box isn't already showing, show box
            if (!currentBox)
            {
                float closestDistance = float.MaxValue;
                GameObject closestObject = null;

                foreach (GameObject obj in objs)
                {
                    float distance = Vector3.Distance(obj.transform.position, transform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = obj;
                    }
                }

                Open(closestObject);
            }
        }
        else if (currentBox)
            Close();
    }

	IEnumerator DelayEntering()
	{
		yield return new WaitForEndOfFrame();

		allowEntering = true;
	}

    void OnDestroy()
    {
        if (currentBox)
        {
            currentBox.gameObject.SetActive(false);
        }
    }

	void OnTriggerEnter(Collider collider)
	{
		if (allowEntering)
		{
			//if collider's layer is in the player layermask
			if (playerLayer == (playerLayer | (1 << collider.gameObject.layer)))
			{
                if (!collider.GetComponent<Health>().isDead)
                {
                    if (!objs.Contains(collider.gameObject))
                        objs.Add(collider.gameObject);
                }
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		//if collider's layer is in the player layermask
		if (playerLayer == (playerLayer | (1 << collider.gameObject.layer)))
		{
			if (objs.Contains(collider.gameObject))
				objs.Remove(collider.gameObject);
		}
	}

	void Open(GameObject player)
    {
        playerInfo = player.GetComponent<PlayerInformation>();

        if (OnOpen != null)
            OnOpen(playerInfo);

        //Position pooled dialogue box
        GameObject obj = ObjectPooler.GetPooledObject(dialogueBoxPrefab);
        obj.transform.position = transform.position + new Vector3(0, textBoxHeight, textBoxDepth);

        currentBox = obj.GetComponent<DialogueBox>();

        //Set random line
        if (currentBox)
        {
            UpdateLines();

            if (OnGetPlayer != null)
                OnGetPlayer(playerInfo, true);
        }
    }

    public void UpdateLines()
    {
        if (currentBox)
        {
            if (lines.Length > 0)
            {
                int index = lastIndex;

                if (lines.Length > 1)
                {
                    while (index == lastIndex)
                        index = Random.Range(0, lines.Length);
                }
                else
                    index = 0;

				string text = lines[index];

				//Remove whitespace from end of line
				text = text.Trim();

                currentBox.SetDialogue(text);

                lastIndex = index;
            }
            else
                currentBox.SetDialogue("!NO LINES!");
        }
    }

	public void Close(bool disable = false)
	{
		//Close dialouge and set null
		currentBox.CloseDialogue();

		currentBox = null;

		if (OnGetPlayer != null)
			OnGetPlayer(playerInfo, false);

		if (disable)
			enabled = false;
	}
}
