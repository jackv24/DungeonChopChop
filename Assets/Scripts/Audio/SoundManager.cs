using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
	private static SoundManager Instance;

	private List<AudioSource> sourcePool = new List<AudioSource>();

	[Tooltip("Should be an AudioSource on a child of this GameObject")]
	public AudioSource templateSource;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//Disable template audio source as it is just that, a template
		templateSource.gameObject.SetActive(false);
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
			AudioSource source = Instance.GetSource();

			if(source)
			{
				//Position audiosource at position, for 3D sound
				source.transform.position = position;

				source.clip = sound.RandomClip;
				source.volume = sound.volume;

				//Randomise pitch
				source.pitch = 1.0f + Random.Range(sound.minPitchVariance, sound.maxPitchVariance);

				//Return audio source to pool after the clip has played
				Instance.StartCoroutine(Instance.ReturnSourceToPool(source, source.clip.length));

				source.Play();
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
	public AudioClip RandomClip { get { return clips[Random.Range(0, clips.Length)]; } }

	[Range(0, 1f)]
	public float volume = 1.0f;

	public float minPitchVariance = -0.1f;
	public float maxPitchVariance = 0.1f;
}