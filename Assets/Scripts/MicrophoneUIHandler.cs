using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneUIHandler : MonoBehaviour {

	Text loudnessText;

	void Start () {
		loudnessText = GameObject.Find ("loudnessHelper").GetComponent<Text> ();
	}

	public void UpdateLoudnessText(string loudness) {

		loudnessText.text = "Loudness: " + loudness;

	}
}
