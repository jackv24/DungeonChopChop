using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public CanvasGroup group;

    [Space()]
    public float fadeInTime = 0.25f;
    public float fadeOutTime = 1.0f;

    [Space()]
    public Image loadingImage;
    public Sprite[] loadingAnimFrames;
    private int frame = 0;
    public float frameRate = 6.0f;
    private float nextFrameUpdate;

    [Space()]
    public Text generatingText;
    public string generatingString = "Generating";
    public float ellipsisDelay = 0.5f;
    private float nextTextUpdate;
    public int ellipsisAmount = 3;
    private int ellipsisCount;

    [Space()]
    public int tileUpdatePause = 10;
	public static int TileUpdatePause { get { return Instance ? Instance.tileUpdatePause : 0; }}

    private Coroutine showRoutine;
	private Coroutine hideRoutine;

    private bool visible = false;

    void Awake()
	{
		if(Instance)
		{
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

	void Update()
	{
        if (visible)
        {
            if (loadingImage && loadingAnimFrames.Length > 1)
            {
                if (Time.time >= nextFrameUpdate)
                {
                    nextFrameUpdate = Time.time + (1 / frameRate);

                    frame++;
                    if (frame >= loadingAnimFrames.Length)
                        frame = 0;

                    loadingImage.sprite = loadingAnimFrames[frame];
                }
            }

            if (generatingText && Time.time >= nextTextUpdate)
            {
                nextTextUpdate = Time.time + ellipsisDelay;

                ellipsisCount++;
                if (ellipsisCount > ellipsisAmount)
                    ellipsisCount = 0;

                string ellipsis = "";
                string invisible = "";

                for (int i = 1; i <= ellipsisCount; i++)
                {
                    ellipsis += ".";
                }

                for (int i = ellipsisCount; i <= ellipsisAmount; i++)
                {
                    invisible += ".";
                }

                generatingText.text = string.Format("{0}{1}<color=#FFFFFF00>{2}</color>", generatingString, ellipsis, invisible);
            }
        }
    }

    public static float Show(string text = "", bool fadeIn = true)
	{
		if(Instance)
		{
        	Instance.generatingString = text;

            if (!Instance.visible)
            {
                Instance.visible = true;
                Instance.showRoutine = Instance.StartCoroutine(Instance.ShowScreen(fadeIn));
            }

            return Instance.fadeInTime;
        }

        return 0;
    }

	public static void Hide()
	{
		if(Instance)
		{
			Instance.visible = false;
            Instance.hideRoutine = Instance.StartCoroutine(Instance.HideScreen());
        }
	}

	IEnumerator ShowScreen(bool fadeIn)
	{
        if (group)
        {
            if (fadeIn)
            {
                float elapsed = 0;

                while (elapsed <= fadeInTime)
                {
                    group.alpha = elapsed / fadeInTime;

                    yield return new WaitForEndOfFrame();
                    elapsed += Time.deltaTime;
                }
            }

            group.alpha = 1;
        }
    }

	IEnumerator HideScreen()
	{
		if (group)
        {
            float elapsed = 0;

            while (elapsed <= fadeInTime)
            {
                group.alpha = 1 - (elapsed / fadeInTime);

                yield return new WaitForEndOfFrame();
                elapsed += Time.deltaTime;
            }

            group.alpha = 0;
        }
	}
}
