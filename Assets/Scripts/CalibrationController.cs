using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationController : MonoBehaviour {

	private bool phoneStabilized = false;
	private bool faceDetected = false;
	private bool ptSimpleConnected = false;
	private bool hasCalibratedCenter = false;

	private float calibrationTimer;
	private bool isCalibrating;
	private int calibrationState = 0;

	public MasterJoystickController masterJoystickController;
	public Image backgroundImage;
	public Sprite calibration1;
	public Sprite calibration3;
	public Sprite calibration5;

	void Awake() {
		backgroundImage.gameObject.SetActive (false);
	}

	void Update() {
		if (isCalibrating) {
			calibrationTimer += Time.deltaTime;

			UpdateCalibrationState ();
		}
	}

	void UpdateCalibrationState() {
		CheckIfFaceIsDetected ();
		CheckIfPTSimpleIsConnected ();

		if ((calibrationState == 1) && (faceDetected == true)) {
			calibrationState = 3;
			backgroundImage.sprite = calibration3;
			Color c = backgroundImage.color;
			c.a = 0.5f;
			backgroundImage.color = c;
		}

		if ((calibrationState == 3) && (ptSimpleConnected == true)) {
			PTCToCBridge.CallSendPTSimpleMessage ("centerCalibration");
			calibrationState = 5;
			backgroundImage.sprite = calibration5;
			StartCalibrationTimer ();
		}

		if ((calibrationState == 5) && (calibrationTimer > 5.0f) && (hasCalibratedCenter == true)) {
			GameObject imageObject = backgroundImage.gameObject;
			imageObject.SetActive (false);
			isCalibrating = false;
			calibrationState = 0;
		}
	}

	public void DidReceiveStartCalibrationMessage(){
		CheckIfFaceIsDetected ();
		CheckIfPTSimpleIsConnected ();
		if ((faceDetected) && (ptSimpleConnected)) {
			calibrationState = 1;
			backgroundImage.gameObject.SetActive (true);
			isCalibrating = true;
		} else {
			SendCalibrationErrorMessage ();
		}

	}

	public void DidReceiveEndCalibrationMessage(){
		hasCalibratedCenter = true;
	}

	void StartCalibrationTimer() {
		calibrationTimer = 0;
	}

	public void CheckIfFaceIsDetected() {
		if (masterJoystickController.faceDetected) {
			faceDetected = true;
		} else {
			faceDetected = false;
		}
	}

	public void CheckIfPTSimpleIsConnected() {
		if (masterJoystickController.PTSimpleIsConnected) {
			ptSimpleConnected = true;
		} else {
			ptSimpleConnected = false;
		}
	}

	public void CheckIfHasCalibratedCenter() {
		// check if face is not moving too much

		//hasCalibratedCenter = true;
	}

	public void SendCalibrationErrorMessage() {
		PTCToCBridge.CallSendPTSimpleMessage ("calibrationError");
	}
}
