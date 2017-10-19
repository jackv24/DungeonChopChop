using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadingAnimation : MonoBehaviour
{
    public int loadScene = 1;

    private AsyncOperation async;
    private float minWaitTime = 0;

    private VideoPlayer player;

	void Awake()
	{
        player = GetComponent<VideoPlayer>();
    }

	void Start()
	{
		if(player && player.clip)
		{
            player.Play();
            minWaitTime = (float)player.clip.length;
        }

        StartCoroutine(Load());
    }

	IEnumerator Load()
	{
        float minDoneTime = Time.time + minWaitTime;

        async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;
        
		while(async.progress < 0.9f)
		{
			yield return new WaitForEndOfFrame();
		}

		while(Time.time < minDoneTime)
		{
            yield return new WaitForEndOfFrame();
        }
		
        async.allowSceneActivation = true;
    }
}
