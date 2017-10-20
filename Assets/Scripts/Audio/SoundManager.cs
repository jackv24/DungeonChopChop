using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	private static SoundManager Instance;

	private List<AudioSource> sourcePool = new List<AudioSource>();

	[Header("Sound Effects")]
	[Tooltip("Should be an AudioSource on a child of this GameObject")]
	public AudioSource templateSource;

	[Header("Music")]
	public AudioSource musicSource;
	private AudioSource primaryMusicSource;
	private AudioSource secondaryMusicSource;
	public float musicFadeTime = 0.5f;
	private Coroutine fadeRoutine;

	[Space()]
	public AudioClip grassBiomeMusic;
	public AudioClip forestBiomeMusic;
	public AudioClip desertBiomeMusic;
	public AudioClip fireBiomeMusic;
	public AudioClip iceBiomeMusic;
	public AudioClip dungeonBiomeMusic;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//Disable template audio source as it is just that, a template
		templateSource.gameObject.SetActive(false);

		//Setup music sources
		if(musicSource)
		{
			primaryMusicSource = musicSource;

			//Setup secondary source by copying data from primary
			secondaryMusicSource = musicSource.gameObject.AddComponent<AudioSource>();
			secondaryMusicSource.outputAudioMixerGroup = primaryMusicSource.outputAudioMixerGroup;
			secondaryMusicSource.spatialBlend = primaryMusicSource.spatialBlend;
			secondaryMusicSource.loop = primaryMusicSource.loop;
		}
	}

	public static void FadeMusicByBiome(LevelTile.Biomes biome)
	{
		if (Instance)
		{
			AudioClip clip = null;

			switch (biome)
			{
				case LevelTile.Biomes.Grass:
					clip = Instance.grassBiomeMusic;
					break;
				case LevelTile.Biomes.Forest:
					clip = Instance.forestBiomeMusic;
					break;
				case LevelTile.Biomes.Desert:
					clip = Instance.desertBiomeMusic;
					break;
				case LevelTile.Biomes.Ice:
					clip = Instance.iceBiomeMusic;
					break;
				case LevelTile.Biomes.Fire:
					clip = Instance.fireBiomeMusic;
					break;
				case LevelTile.Biomes.Dungeon:
					clip = Instance.dungeonBiomeMusic;
					break;
			}

            FadeMusic(clip);
		}
	}

	public static void FadeMusic(AudioClip musicClip)
	{
		if (Instance && Instance.musicSource)
		{
            //Don't fade if it's the same clip
            if (musicClip == Instance.primaryMusicSource.clip)
                return;

			//Prevent multiple fades from happening at once
			if(Instance.fadeRoutine != null)
				Instance.StopCoroutine(Instance.fadeRoutine);

			Instance.fadeRoutine = Instance.StartCoroutine(Instance.FadeMusicOverTime(musicClip, Instance.musicFadeTime));
		}
	}

	IEnumerator FadeMusicOverTime(AudioClip clip, float duration)
	{
		//Start secondary source at 0 volume
		secondaryMusicSource.clip = clip;
		secondaryMusicSource.volume = 0;
		secondaryMusicSource.Play();

		float elapsed = 0;

		while(elapsed < duration)
		{
			float amount = elapsed / duration;

			//Crossfade sources
			primaryMusicSource.volume = 1 - amount;
			secondaryMusicSource.volume = amount;

			yield return new WaitForEndOfFrame();
			elapsed += Time.deltaTime;
		}

		//Stop primary source
		primaryMusicSource.volume = 0;
		primaryMusicSource.clip = null;
		primaryMusicSource.Stop();

		secondaryMusicSource.volume = 1;

		//Swap primary and secondary sources
		AudioSource temp = primaryMusicSource;
		primaryMusicSource = secondaryMusicSource;
		secondaryMusicSource = temp;
	}

	/// <summary>
	/// Plays a SoundEffect at a given position, using pooled AudioSources.
	/// </summary>
	/// <param name="sound">Sound Efect to play.</param>
	/// <param name="position">The position to play this sound (for 3D positional sound)</param>
	public static void PlaySound(SoundEffect sound, Vector3 position)
	{
		//Make sure there is a sound manager with a template sound source assigned
		if(Instance && Instance.templateSource)
		{
            AudioClip clip = sound.RandomClip;

			if(clip)
			{
                AudioSource source = Instance.GetSource();

                if (source)
                {
                    //Position audiosource at position, for 3D sound
                    source.transform.position = position;

                    source.clip = clip;
                    source.volume = sound.volume;

                    //Randomise pitch
                    source.pitch = 1.0f + Random.Range(sound.minPitchVariance, sound.maxPitchVariance);

                    //Return audio source to pool after the clip has played
                    Instance.StartCoroutine(Instance.ReturnSourceToPool(source, clip.length));

                    source.Play();
                }
			}
		}
	}

	IEnumerator ReturnSourceToPool(AudioSource source, float time)
	{
		yield return new WaitForSeconds(time);

		//Reset to template
		source.transform.localPosition = Vector3.zero;

		source.clip = null;
		source.volume = templateSource.volume;
		source.pitch = templateSource.pitch;

		//Disable, returning to pool
		source.gameObject.SetActive(false);
	}

	public AudioSource GetSource()
	{
		AudioSource source = null;

		//Check if there are any available AudioSources in the pool
		for (int i = 0; i < sourcePool.Count; i++)
		{
			if (!sourcePool[i].gameObject.activeSelf)
			{
				source = sourcePool[i];
				source.gameObject.SetActive(true);
				break;
			}
		}

		//If no source was found, instantiate one from the template
		if (source == null && templateSource)
		{
			GameObject obj = Instantiate(templateSource.gameObject, transform);

			source = obj.GetComponent<AudioSource>();
			sourcePool.Add(source);

			obj.SetActive(true);
		}

		return source;
	}
}

[System.Serializable]
public class SoundEffect
{
	public AudioClip[] clips;

	//Public property that returns a random clip from the clips array
    public AudioClip RandomClip { get { return clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null; } }

	[Range(0, 1f)]
	public float volume = 1.0f;

	public float minPitchVariance = -0.1f;
	public float maxPitchVariance = 0.1f;
}