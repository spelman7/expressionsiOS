using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterTrackingController : MonoBehaviour {

	// global variable to establish the current tracking mode - 0 = VISAGE, 1 = ARKIT
	public int CurrentTracker = 0;

	// other tracking-related variables
	public bool visageTrackerHasInitialized = false;

	// Pointers to different tracker objects
	public VisageTracker visageTracker;
	public MasterJoystickController masterJoystickController;
	public GameObject trackingButtonCanvasObject;

	// Pointers to other game objects that need to set active or inactive depending on the tracking mode
	public GameObject Video;
	public GameObject VideoCamera;


	public Camera mainCamera;

	void Start() {
		// Set the target frame rate to be 60FPS
		Application.targetFrameRate = 60;

		// Set the app to never go to sleep
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		//visageTrackerHasInitialized = false;

		// Detect whether the device is an iPhone X or not
		bool deviceIsiPhoneX = GameControl.control.deviceIsiPhone10;

		if (deviceIsiPhoneX) {
			// Set CurrentTracker to be 1
			CurrentTracker = 1;

			// Change the Video and Video Camera objects to be inactive
			Video.SetActive (false);
			VideoCamera.SetActive (false);

			// Initialize ARKit Tracker
			masterJoystickController.ARKitTrackerStart();

		} else {
			// Set CurrentTracker to be 0
			CurrentTracker = 0;

			// Change the Video and Video Camera objects to be active
			Video.SetActive (true);
			VideoCamera.SetActive (true);

			// Change the Switch Tracking Mode button to be inactive
			trackingButtonCanvasObject.SetActive(false);

			// Initialize Visage Tracker
			masterJoystickController.VisageTrackerStart();

			// Switch visage tracker initialization bool to be true
			visageTrackerHasInitialized = true;
		}

		// Scale the face orientation guide
		masterJoystickController.ScaleFaceOrientationGuide (CurrentTracker);

	}


	public void SwitchCurrentTracker() {
		if (CurrentTracker == 0) {

			// Close the camera that's currently running as part of visageTracker
			visageTracker.CloseCamera ();

			// Change the Video and Video Camera objects to be inactive
			Video.SetActive (false);
			VideoCamera.SetActive (false);

			// Start the ARKit Tracker
			masterJoystickController.ARKitTrackerStart();

			// Set the CurrentTracker status to be 1 (ARKit)
			CurrentTracker = 1;

		} else if (CurrentTracker == 1) {

			// Check to see if the visage tracker has been initialized or not
			if (visageTrackerHasInitialized == false) {
				// If not, initialize it
				masterJoystickController.VisageTrackerStart ();
				visageTrackerHasInitialized = true;
			} else {
				// If the visage tracker has been initialized, open the camera that's not currently running as part of visageTracker
				visageTracker.OpenCameraFromMasterTrackerController ();
			}
				
			// Change the Video and Video Camera objects to be active
			Video.SetActive (true);
			VideoCamera.SetActive (true);

			// Pause the ARKit Tracker
			masterJoystickController.ARKitTrackerPause();

			// Set the CurrentTracker status to be 1 (ARKit)
			CurrentTracker = 0;
		}

		// Scale the face orientation guide
		masterJoystickController.ScaleFaceOrientationGuide (CurrentTracker);

		// Switch the camera rotation
		SwitchCameraRotation();

		Debug.Log ("SWITCHED CURRENT TRACKING MODE");

	}

	void SwitchCameraRotation () {
		if (CurrentTracker == 0) {
			mainCamera.ResetProjectionMatrix ();
			mainCamera.transform.rotation = Quaternion.identity;
			//mainCamera.fieldOfView = 36.87f;
		} else if (CurrentTracker == 1) {
			//mainCamera.fieldOfView = 60f;
		}
	}
		
}
