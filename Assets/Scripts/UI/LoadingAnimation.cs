using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadingAnimation : MonoBehaviour
{
    public int loadScene = 1;
    public int preloadScene = 2;

    private float minWaitTime = 0;
    private float minDoneTime;

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

        minDoneTime = Time.time + minWaitTime;

        StartCoroutine(Load(loadScene));
        //StartCoroutine(Load(preloadScene, false));
    }

	IEnumerator Load(int scene, bool activate = true)
	{
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        async.allowSceneActivation = false;
        
		while(async.progress < 0.9f)
		{
			yield return new WaitForEndOfFrame();
            Debug.Log("Loading scene " + scene + " progress: " + async.progress);
        }

		while(Time.time < minDoneTime)
		{
            yield return new WaitForEndOfFrame();
        }
		
        if(activate)
            async.allowSceneActivation = true;
    }
}
