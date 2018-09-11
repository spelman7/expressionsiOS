using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffectsController : MonoBehaviour {

	public AudioClip downClick;
	public AudioClip upClick;
	private AudioSource source;
	private float volLowRange = 0.5f;
	private float volHighRange = 1.0f;

	void Awake () {
		source = GetComponent<AudioSource> ();
	}

	public void PlaySoundEffect(string effect) {
		if (effect == "down") {
			source.PlayOneShot (downClick, 1);
		} else if (effect == "up") {
			source.PlayOneShot (upClick, 1);
		}
	}


}
