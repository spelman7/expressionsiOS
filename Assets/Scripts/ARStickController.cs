using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class ARStickController : MonoBehaviour {

	//ARKit-specific variables
	private UnityARSessionNativeInterface m_session;
	Dictionary<string, float> currentBlendShapes;

	// Tracking status variables
	public bool isTracking = false;


	// Links to other game objects
	public GameObject anchorPrefab;

	// GUI/Debug variables
	public bool toggleGUI = false;
	public Font yikes;
	public GUIStyle blendshapeGUIstyle;


	//***********  INIT  ***********//

	void Start () {
		// Set the target frame rate to be 60FPS
		Application.targetFrameRate = 60;

		// Initialize the bridge between Unity and the ARKit Native functions/structures
		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

		// Create the configuration struct for this ARKit session
		ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;

		// If the config file looks good, start the ARKIT session and add FaceAdded, FaceUpdated, and FaceRemoved as callback functions on ARKit events
		if (config.IsSupported ) {
			
			m_session.RunWithConfig (config);

			UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
			UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
			UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;

		}

		// Fire the init/start functions necessary for Peertalk
		PTCToCBridge.CallPTConnect();

		// Instantiate the blendshape GUI style
		blendshapeGUIstyle = new GUIStyle();
		blendshapeGUIstyle.font = yikes;
		blendshapeGUIstyle.fontSize = 30;

	}

	//***********  GUI  ***********//
	void OnGUI()
	{
		if (toggleGUI == true) {
			PrintBlendshapeValuesToGUI ();
		}
	}


	//***********  FACE ADDED, UPDATED, REMOVED  ***********//

	void FaceAdded (ARFaceAnchor anchorData) {
		//Set isTracking to true
		isTracking = true;
		
		// Set the position of the face axes to the center of your face data
		anchorPrefab.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		anchorPrefab.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);
		anchorPrefab.SetActive (true);

		// get the current blend shape key/value pairs from your ARFaceAnchor
		currentBlendShapes = anchorData.blendShapes;
	}

	void FaceUpdated (ARFaceAnchor anchorData) {
		//Set isTracking to true
		isTracking = true;
		
		// Set the position of the face axes to the center of your face data
		anchorPrefab.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		anchorPrefab.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);

		// Get world rotation of face anchor
		Vector3 faceRotationWorld = UnityARMatrixOps.GetRotation (anchorData.transform).eulerAngles;

		// Get world rotation of camera
		Matrix4x4 cameraPose = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetCameraPose ();
		Vector3 cameraRotationWorld = UnityARMatrixOps.GetRotation (cameraPose).eulerAngles;

		// Subtract face anchor vector from world space vector
		Vector3 relativeRotation = cameraRotationWorld - faceRotationWorld;

		// Send the face rotation data via USB
		SendFaceRotation (relativeRotation.x, relativeRotation.y, relativeRotation.z);

		// get the current blend shape key/value pairs from your ARFaceAnchor
		currentBlendShapes = anchorData.blendShapes;

		// Send the required blendshape values via USB
		SendBlendshapeValues();
	}

	void FaceRemoved (ARFaceAnchor anchorData) {
		isTracking = false;
		anchorPrefab.SetActive (false);
	}


	//***********  HANDLE BLENDSHAPES  ***********//


	void PrintBlendshapeValuesToGUI(){
		
		string blendshapes = "";
		foreach(KeyValuePair<string,float> kvp in currentBlendShapes) {
			blendshapes += " [";
			blendshapes += kvp.Key.ToString ();
			blendshapes += ":";
			blendshapes += kvp.Value.ToString ("F2");
			blendshapes += "]\n";
		}

		GUILayout.BeginHorizontal (GUILayout.ExpandHeight(true));
		GUILayout.Box (blendshapes, blendshapeGUIstyle);
		GUILayout.EndHorizontal ();

	}

	void SendBlendshapeValues(){
		
		// Click test #1: Mouth Pucker
		float mouthPucker = currentBlendShapes ["mouthPucker"];

		// Click test #2: Blink/Squint
		float eyeBlinkLeft = currentBlendShapes ["eyeBlink_L"];
		float eyeBlinkRight = currentBlendShapes ["eyeBlink_R"];
		float eyeSquintLeft = currentBlendShapes ["eyeSquint_L"];
		float eyeSquintRight = currentBlendShapes ["eyeSquint_R"];

		// Click test #3: Cheek Puff
		float cheekPuff = currentBlendShapes["cheekPuff"];

		// Click test #4: Eyebrows Up
		float browInnerUp = currentBlendShapes ["browInnerUp"];

		//PTCToCBridge.CallSendBlendshapes (mouthPucker, eyeBlinkLeft, eyeBlinkRight, eyeSquintLeft, eyeSquintRight, cheekPuff, browInnerUp);

	}

		

	//***********  SEND MESSAGES TO PTSIMPLE  ***********//

	// Function to send face rotational float data to PTSimple
	public void SendFaceRotation(float f1, float f2, float f3) {
		//PTCToCBridge.CallSendFloats (f1, f2, f3);
	}



	// Function to fire a CGEvent/mouseClick in PTSimple
	public void SendButtonClick() {
		//PTCToCBridge.CallButtonPush (1);
	}



	//***********  APPLICATION STATE HANDLERS  ***********//

	//
	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus == true) {
			// we are paused
		} else if (pauseStatus == false) {
			//let's connect
			PTCToCBridge.CallPTConnect();
		}
	}


	/*
	void OnDestroy()
	{
		
	}

	// test function to send screen touch position to peertalk
	public void GetTouchPosition() {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			// Get touch position
			Vector2 touchPosition = Input.GetTouch(0).position;
			int touchPositionX = (int)touchPosition.x;
			int touchPositionY = (int)touchPosition.y;

			//Debug.Log ("touch pos X: " + touchPositionX + "  touch pos Y: " + touchPositionY);
			PTCToCBridge.CallSendTouchPosition(touchPositionX, touchPositionY);
		}
	}
	*/
}
