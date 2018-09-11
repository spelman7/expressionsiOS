using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

	public static GameControl control;

	public bool hasCompletedTutorial;
	public bool deviceIsiPhone10;
	public string deviceId;
	public float highScore;

	private float timeLeft = 2.0f;
	private bool timerHasExpired = false;

	void Awake () {
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}

		Scene activeScene = SceneManager.GetActiveScene ();
		if (activeScene.name == "Start") {
			ChangeSceneSafely ("Active");
		}

		DetectIphoneVersion ();
	}

	void DetectIphoneVersion() {
		deviceIsiPhone10 = UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX;
	}

	public void Save() {
		// create a file
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/userSettings.dat");

		UserData data = new UserData ();
		data.hasCompletedTutorial = hasCompletedTutorial;
		data.deviceIsiPhone10 = deviceIsiPhone10;
		data.deviceId = deviceId;
		data.highScore = highScore;

		// take our serializable data and write it to our file
		bf.Serialize(file, data);
		file.Close ();

	}

	public void Load() {
		if (File.Exists(Application.persistentDataPath + "/userSettings.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/userSettings.dat", FileMode.Open);
			UserData data = (UserData)bf.Deserialize(file);
			file.Close();

			hasCompletedTutorial = data.hasCompletedTutorial;
			deviceIsiPhone10 = data.deviceIsiPhone10;
			deviceId = data.deviceId;
			highScore = data.highScore;
		}
	}

	[Serializable]
	class UserData {
		public bool hasCompletedTutorial;
		public bool deviceIsiPhone10;
		public string deviceId;
		public float highScore;
	}

	public void ChangeSceneSafely(string sceneName) {
		StartCoroutine (LoadAsyncScene (sceneName));
	}

	IEnumerator LoadAsyncScene(string sceneName) {
		
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
