using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MasterJoystickController : MonoBehaviour {

	//create pointer to Tracker object
	public VisageTracker visageTracker;

	// create pointer to master tracking controller
	public MasterTrackingController masterTrackingController;

	// create pointer to Face Orientation Guide object
	public GameObject faceOrientationGuide;

	// create pointer to Audio Effects Controller object
	public AudioEffectsController audioEffectsController;

	// create pointer to Calibration Controller object
	public CalibrationController calibrationController;

	//ARKit-specific variables
	private UnityARSessionNativeInterface m_session;
	ARKitFaceTrackingConfiguration config;
	Dictionary<string, float> currentBlendShapes;
	private Vector3 relativeRotation;
	private float ARKitFaceNotDetectedTimer;

	//Visage-specific variables
	public Vector3 faceTranslation;
	public Vector3 faceRotation;
	public int VisageTrackerStatus;
	public float[] actionUnitValues;
	public Vector3[] Vertices;
	public int[] lipVertexIndices;
	public Vector3[] lipVertices;

	//Calibration variables
	private string currentSceneName;
	public bool faceDetected;
	public bool PTSimpleIsConnected;

	//Message display variables
	public Text unityMessageText;
	private bool displayUnityMessageText;
	private float unityMessageTimer;


	//***********  INIT  ***********//

	void Start () {
		PTCToCBridge.CallPTConnect();

		faceTranslation = new Vector3 (0, 0, 0);
		faceRotation = new Vector3 (0, 0, 0);
		relativeRotation = new Vector3 (0, 0, 0);

		lipVertexIndices = new int[] {135, 23, 282, 139, 130, 0, 85, 87};
		lipVertices = new Vector3[lipVertexIndices.Length];

		ARKitFaceNotDetectedTimer = 0.5f;
		faceDetected = false;

		unityMessageText.gameObject.SetActive (false);
	}

	void Update() {

		// only perform Visage-specific actions if the current tracking mode is "1" for Visage
		if (masterTrackingController.CurrentTracker == 0) {

			GetVisageTrackerStatus ();

			if (VisageTrackerStatus == 1) {

				GetVisageOrientationAndExpressions ();
				GetVisagePuckerValues ();
				MoveFaceOrientationGuide ();
				SendVisageOrientation (faceRotation.x, faceRotation.y, faceRotation.z);
				SendVisageExpressions ();
				SendVisagePuckerValues ();

				faceDetected = true;

			}

			if (VisageTrackerStatus != 1) {
				faceDetected = false;
			}
		}

		if (masterTrackingController.CurrentTracker == 1) {
			if (faceDetected == true) {
				ARKitFaceNotDetectedTimer -= Time.deltaTime;
			}
			if (ARKitFaceNotDetectedTimer <= 0) {
				faceDetected = false;
			}
		}

		if (displayUnityMessageText) {
			if (unityMessageTimer <= 0f) {
				displayUnityMessageText = false;
				unityMessageText.gameObject.SetActive (false);
			}
			unityMessageTimer -= Time.deltaTime;
		}
	}

	//***********  TRACKER INIT / START / PAUSE  ***********//

	public void ARKitTrackerStart() {

		// Initialize the bridge between Unity and the ARKit Native functions/structures
		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

		// Create the configuration struct for this ARKit session
		config = new ARKitFaceTrackingConfiguration();
		config.alignment = UnityARAlignment.UnityARAlignmentGravity;
		config.enableLightEstimation = true;

		// If the config file looks good, start the ARKIT session and add FaceAdded, FaceUpdated, and FaceRemoved as callback functions on ARKit events
		if (config.IsSupported ) {

			m_session.RunWithConfig (config);

			UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
			UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
			UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;

		}

	}

	public void ARKitTrackerPause() {
		m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

		m_session.Pause ();
	}

	public void VisageTrackerStart() {
		visageTracker.VisageTrackerStart ();
	}

	public void VisageTrackerPause() {
	}


	//***********  GUI  ***********//



	void MoveFaceOrientationGuide(){
		// Transform the faceTranslation and faceRotation Vectors to be in world space
		Vector3 faceTranslationViewportCoordinates = Camera.main.WorldToViewportPoint (faceTranslation);
		Vector3 newFaceOrientationPosition = Camera.main.ViewportToWorldPoint (new Vector3(faceTranslationViewportCoordinates.x, faceTranslationViewportCoordinates.y, 7.0f));

		// Set the position of the face orientation guide
		faceOrientationGuide.transform.position = newFaceOrientationPosition;
		faceOrientationGuide.transform.rotation = Quaternion.Euler (faceRotation);
	}

	public void ScaleFaceOrientationGuide(int currentTracker) {
		if (currentTracker == 0) {
			// scale it up
			faceOrientationGuide.transform.localScale = new Vector3 (1F, 1F, 1F);
		} else if (currentTracker == 1) {
			faceOrientationGuide.transform.localScale = new Vector3 (0.05F, 0.05F, 0.05F);
		}
	}


	//***********  ARKIT COMMUNICATION FUNCTIONS  ***********//

	void FaceAdded (ARFaceAnchor anchorData) {

		// Set the position of the face axes to the center of your face data
		faceOrientationGuide.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		faceOrientationGuide.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);

		// get the current blend shape key/value pairs from your ARFaceAnchor
		currentBlendShapes = anchorData.blendShapes;

		faceDetected = true;
		ARKitFaceNotDetectedTimer = 0.5f;
	}

	void FaceUpdated (ARFaceAnchor anchorData) {

		// Set the position of the face axes to the center of your face data
		faceOrientationGuide.transform.position = UnityARMatrixOps.GetPosition (anchorData.transform);
		faceOrientationGuide.transform.rotation = UnityARMatrixOps.GetRotation (anchorData.transform);

		// get the current blend shape key/value pairs from your ARFaceAnchor
		currentBlendShapes = anchorData.blendShapes;

		// Get world rotation of face anchor
		Vector3 faceRotationWorld = UnityARMatrixOps.GetRotation (anchorData.transform).eulerAngles;

		// Get world rotation of camera
		Matrix4x4 cameraPose = UnityARSessionNativeInterface.GetARSessionNativeInterface ().GetCameraPose ();
		Vector3 cameraRotationWorld = UnityARMatrixOps.GetRotation (cameraPose).eulerAngles;

		// Subtract face anchor vector from world space vector
		relativeRotation = cameraRotationWorld - faceRotationWorld;


		// Send the face rotation data via USB
		SendARKitOrientation (relativeRotation.x, relativeRotation.y, relativeRotation.z);


		// get the current blend shape key/value pairs from your ARFaceAnchor
		currentBlendShapes = anchorData.blendShapes;


		// Send the required blendshape values via USB
		SendARKitExpressions ();

		faceDetected = true;
		ARKitFaceNotDetectedTimer = 0.5f;
	}

	void FaceRemoved (ARFaceAnchor anchorData) {
		faceDetected = false;
	}

	public void SendARKitOrientation(float x, float y, float z) {
		PTCToCBridge.CallSendARKitOrientation (x, y, z);
	}

	public void SendARKitExpressions() {
		float mouthPucker = currentBlendShapes ["mouthPucker"];
		float jawOpen = currentBlendShapes["jawOpen"];
		float browInnerUp = currentBlendShapes ["browInnerUp"];

		PTCToCBridge.CallSendARKitExpressions (mouthPucker, jawOpen, browInnerUp);
	}


	//***********  VISAGE COMMUNICATION FUNCTIONS  ***********//

	public void GetVisageTrackerStatus(){
		VisageTrackerStatus = visageTracker.TrackerStatus;
	}

	public void GetVisageOrientationAndExpressions(){
		faceTranslation = visageTracker.Translation;
		faceRotation = visageTracker.Rotation;

		//get visage expressions / actionUnits
		actionUnitValues = visageTracker.ActionUnitValues;
	}

	public void GetVisagePuckerValues(){
		Vertices = visageTracker.Vertices;

		// make this so it doesn't create a new piece of memory every loop
		for (int i = 0; i < lipVertexIndices.Length; i++) {
			lipVertices [i] = Vertices [lipVertexIndices [i]];
		}
			
	}

	public void SendVisageOrientation(float x, float y, float z) {
		PTCToCBridge.CallSendVisageOrientation (x, y, z);
	}

	public void SendVisageExpressions() {
		float noseWrinkler = actionUnitValues[0];
		float jawOpen = actionUnitValues[3];
		float eyebrowsUp = (actionUnitValues[9] + actionUnitValues[19])/2;

		PTCToCBridge.CallSendVisageExpressions (noseWrinkler, jawOpen, eyebrowsUp);
	}

	public void SendVisagePuckerValues() {
		PTCToCBridge.CallSendVisagePucker (lipVertices [0].x, lipVertices [0].y, lipVertices [0].z, lipVertices [1].x, lipVertices [1].y, lipVertices [1].z, lipVertices [2].x, lipVertices [2].y, lipVertices [2].z, lipVertices [3].x, lipVertices [3].y, lipVertices [3].z, lipVertices [4].x, lipVertices [4].y, lipVertices [4].z, lipVertices [5].x, lipVertices [5].y, lipVertices [5].z, lipVertices [6].x, lipVertices [6].y, lipVertices [6].z, lipVertices [7].x, lipVertices [7].y, lipVertices [7].z);
		
	}

	//***********  GENERAL COMMUNICATION FUNCTIONS  ***********//

	public void UnityMessageInterpreter(string message) {

		Debug.Log(message);

		if ((message == "down") || (message == "up")) {
			audioEffectsController.PlaySoundEffect (message);
		} 

		if (message == "connected") {
			PTSimpleIsConnected = true;
		}

		if (message == "startCalibrationButton") {
			calibrationController.DidReceiveStartCalibrationMessage ();
		}

		if (message == "hasEndedCalibration") {
			calibrationController.DidReceiveEndCalibrationMessage ();
		}

		if (message == "startTutorialButton") {
			
		}

		if (message == "PTConnected") {
			PTSimpleIsConnected = true;
		}

		if (message == "PTDisconnected") {
			PTSimpleIsConnected = false;
		}

		if (message == "eyebrowsup") {
			string displayMessage = "eyebrows up";
			DisplayUnityMessageText (displayMessage);
		}

		if (message == "headshake") {
			string displayMessage = "head shake";
			DisplayUnityMessageText (displayMessage);
		}

		if (message == "puckeron") {
			string displayMessage = "pucker on";
			DisplayUnityMessageText (displayMessage);
		}

		if (message == "eyeblink") {
			string displayMessage = "eye blink";
			DisplayUnityMessageText (displayMessage);
		}


		/*
			string firstLetter = message.Substring (0, 1);
			if (firstLetter == "w") {
				int w = 0;
				int.TryParse(message.Substring (1), out w);
				Debug.Log ("did receive width: " + w.ToString());
			} else if (firstLetter == "h") {
				int h = 0;
				int.TryParse (message.Substring (1), out h);
				Debug.Log ("did receive height: " + h.ToString());
			}
		*/

	}

	public void DisplayUnityMessageText(string message) {
		unityMessageText.gameObject.SetActive (true);
		displayUnityMessageText = true;
		unityMessageText.text = message;
		unityMessageTimer = 1.5f;
	}


	//***********  APPLICATION STATE HANDLERS  ***********//

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus == true) {
			// we are paused
		} else if (pauseStatus == false) {
			//let's connect
			PTCToCBridge.CallPTConnect();
		}
	}



	//***********  DEPRECATED METHODS  ***********//

	/*
	public void GetTouchPosition() {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			// Get touch position
			Vector2 touchPosition = Input.GetTouch(0).position;
			int touchPositionX = (int)touchPosition.x;
			int touchPositionY = (int)touchPosition.y;

			//Debug.Log ("touch pos X: " + touchPositionX + "  touch pos Y: " + touchPositionY);
			//PTCToCBridge.CallSendTouchPosition(touchPositionX, touchPositionY);
		}
	}

	void GetFacelineCylinderIntersection(){
		RaycastHit cylinderHit;
		Vector3 origin = new Vector3 (0, 0, 0);

		//Ray cylinderRay = new Ray(faceOrientationGuide.transform.position, faceOrientationGuide.transform.rotation.eulerAngles);
		Ray cylinderRay = new Ray(Camera.main.transform.position, faceOrientationGuide.transform.forward);


		if (Physics.Raycast (cylinderRay, out cylinderHit)) {
			string name = cylinderHit.transform.gameObject.name;
			Vector3 planePosition = cylinderHit.transform.gameObject.transform.position;
			Vector3 facelineCylinderIntersection = cylinderHit.point;
			float x = planePosition.x - facelineCylinderIntersection.x;
			float y = planePosition.y - facelineCylinderIntersection.y;
			float z = planePosition.z - facelineCylinderIntersection.z;
			Debug.Log (name + ":  x: " + x.ToString ("F2") + " y: " + y.ToString ("F2") + " z: " + z.ToString ("F2"));
		}
	}
	*/



}
