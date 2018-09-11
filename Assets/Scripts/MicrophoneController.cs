using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneController : MonoBehaviour {

	AudioClip microphoneInput;
	bool microphoneInitialized;
	public float averageLevel;
	public float sensitivity;
	public bool detected;

	public MicrophoneUIHandler microphoneUIHandler;


	void Awake() {
		// microphone init
		if (Microphone.devices.Length > 0) {
			microphoneInput = Microphone.Start (Microphone.devices [0], true, 999, 44100);
			microphoneInitialized = true;
		}
	}

	void Update () {
		GetMicrophoneInput ();
		UpdateMicrophoneUI (averageLevel);
	}

	void GetMicrophoneInput(){
		int dec = 128;
		float[] waveData = new float[dec];
		int micPosition = Microphone.GetPosition (null) - (dec + 1);
		microphoneInput.GetData (waveData, micPosition);

		float levelMax = 0;
		for (int i = 0; i < dec; i++) {
			float wavePeak = waveData [i] * waveData [i];
			if (levelMax < wavePeak) {
				levelMax = wavePeak;
			}
		}

		averageLevel = Mathf.Sqrt (Mathf.Sqrt (levelMax));
	}

	void UpdateMicrophoneUI(float level) {
		microphoneUIHandler.UpdateLoudnessText (level.ToString ("F2"));
	}
}
