using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioClip hitSound;
	public AudioClip menuButtonSound;
    public AudioClip pickUpSound;
    public AudioClip deathSound;
    public AudioClip buttonPressSound;
    public AudioClip rewardSound;
    public AudioClip pulseSound;

    public float pitchCount;

    // Use this for initialization
    public void PlayClip (AudioClip clip, float volume) {

        if(GameManager.instance.saveData.volumeOn)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.volume = volume;
            newSource.PlayOneShot(clip);
            Destroy(newSource, clip.length);
        }
	}

    public void PlayClipWithPitch(AudioClip clip, float volume,float pitch = 1)
    {
        if (GameManager.instance.saveData.volumeOn)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.volume = volume;
            newSource.pitch = pitch;
            newSource.PlayOneShot(clip);
            Destroy(newSource, clip.length);
        }
    }
}
