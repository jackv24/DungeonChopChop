using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class ShowCutscene : MonoBehaviour {

    public VideoClip[] lockBreakCutscenes;
    public float fadeTime = .01f;
    public float waitToPlayTime = 2;

    private RawImage image;
    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    private AudioSource audioSource;

    private Coroutine movieCoroutine;
    private Coroutine fadeCoroutine;

	// Use this for initialization
	void Start () 
    {
        image = GetComponent<RawImage>();

        if (LevelGenerator.Instance)
            LevelGenerator.Instance.OnGenerationFinished += DoCutscene;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void DoCutscene()
    {
        int count = 0;
        string key = "";
        //loop through all the dungeon items
        foreach (KeyValuePair<string, bool> item in ItemsManager.Instance.dungeonItems)
        {
            //checks if the cutscene has been played for said dungeon item
            if (item.Value == false)
            {
                PlayCutscene(item.Key);
                key = item.Key;
                break;
            }
        }

        if (ItemsManager.Instance.dungeonItems.ContainsKey(key))
            ItemsManager.Instance.dungeonItems[key] = true;
    }

    void PlayCutscene(string key)
    {
        if (movieCoroutine != null)
        {
            movieCoroutine = null;
        }

        int number = 0;

        if (key == "Goggles")
            number = 0;
        else if (key == "Boots")
            number = 1;
        else if (key == "Armor")
            number = 2;
        else if (key == "Gauntlet")
            number = 3;
            

        movieCoroutine = StartCoroutine(playVideo(lockBreakCutscenes[number]));
    }

    IEnumerator Fading(bool fadeIn)
    {
        if (fadeIn)
        {
            while (image.color.a < 1)
            {
                image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 1), fadeTime);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (image.color.a > 0)
            {
                image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, 0), fadeTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator playVideo(VideoClip videoToPlay)
    {
        fadeCoroutine = null;

        yield return new WaitForSeconds(waitToPlayTime);

        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

        //Add VideoPlayer to the GameObject
        if (!videoPlayer)
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();

        //We want to play from video clip not from url
        videoPlayer.source = VideoSource.VideoClip;

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        //fadeCoroutine = StartCoroutine(Fading(false));

        movieCoroutine = null;
    }
}
