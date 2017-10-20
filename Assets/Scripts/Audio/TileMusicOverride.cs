

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMusicOverride : MonoBehaviour
{
    public AudioClip musicClip;

    public void SwitchTo()
	{
        SoundManager.FadeMusic(musicClip);
    }
}
