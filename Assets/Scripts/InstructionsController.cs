using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsController : MonoBehaviour {

	private bool phoneStabilized = false;
	private bool faceDetected = false;
	private bool connected = false;

	public MasterJoystickController masterJoystickController;
	public Image instructionsImage;
	public Image messageImageBG;
	public Sprite instructionsFaceNotDetected;
	public Sprite instructionsNotConnected; // not connected

	void Awake () {
		instructionsImage.gameObject.SetActive (false);
		messageImageBG.gameObject.SetActive (false);

	}
	

	void Update () {
		CheckIfInstructionsRequired ();
	}

	void CheckIfInstructionsRequired() {
		CheckIfConnected ();
		CheckIfFaceDetected ();

		if ((connected) && (faceDetected) && (instructionsImage.gameObject.activeSelf)) {
			DeactivateInstructionsImage ();
		} else if ((!connected) && (faceDetected)) {
			instructionsImage.sprite = instructionsNotConnected;
		}
	}

	void CheckIfConnected() {
		if ((masterJoystickController.PTSimpleIsConnected == false) && (connected == true)) {
			connected = false;
			ActivateInstructionsImage ();
			instructionsImage.sprite = instructionsNotConnected;
		} else if ((masterJoystickController.PTSimpleIsConnected == true) && (connected == false)) {
			connected = true;
		} else if (masterJoystickController.PTSimpleIsConnected == false) {
			connected = false;
			if (instructionsImage.gameObject.activeSelf == false) {
				ActivateInstructionsImage ();
				instructionsImage.sprite = instructionsNotConnected;
			}
		}
		else if (masterJoystickController.PTSimpleIsConnected) {
			connected = true;
		}
	}

	void CheckIfFaceDetected() {
		if ((masterJoystickController.faceDetected == false) && (faceDetected == true)) {
			faceDetected = false;
			ActivateInstructionsImage ();
			instructionsImage.sprite = instructionsFaceNotDetected;
		} 
		else if ((masterJoystickController.faceDetected == true) && (faceDetected == false)) {
			faceDetected = true;
		} else if (masterJoystickController.faceDetected == false) {
			faceDetected = false;
			if (instructionsImage.gameObject.activeSelf == false) {
				ActivateInstructionsImage ();
				instructionsImage.sprite = instructionsFaceNotDetected;
			}
		}
		else if (masterJoystickController.faceDetected) {
			faceDetected = true;
		}
	}

	void ActivateInstructionsImage(){
		instructionsImage.gameObject.SetActive (true);
		messageImageBG.gameObject.SetActive (false);
		Color c = instructionsImage.color;
		c.a = 0.75f;
		instructionsImage.color = c;
	}

	void DeactivateInstructionsImage(){
		instructionsImage.gameObject.SetActive (false);
		messageImageBG.gameObject.SetActive (true);
	}
}
