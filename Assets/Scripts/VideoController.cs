using UnityEngine;

public class VideoController : MonoBehaviour
{
	public MasterTrackingController masterTrackingController;
	public VisageTracker visageTracker;
	private float aspect;
	private Vector3 scale;
	private Vector3 offset;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if ((visageTracker != null) && (masterTrackingController.CurrentTracker == 0)) {
         
			float widthScale = visageTracker.TexWidth / (float)visageTracker.ImageWidth;
			float heightScale = visageTracker.TexHeight / (float)visageTracker.ImageHeight;
			
			float aspect = visageTracker.ImageWidth / (float)visageTracker.ImageHeight;

			float height = 100.0f;
			float width = height / aspect;
			
			float texAspect = visageTracker.TexWidth / (float)visageTracker.TexHeight;

			float widthDiff = (visageTracker.TexWidth - visageTracker.ImageWidth) / (float)visageTracker.TexWidth;

			//scale = new Vector3(-100.0f*heightScale, 100.0f*heightScale, 1.0f);
			//scale = new Vector3(-width*widthScale, width*widthScale, 1.0f);
			scale = new Vector3 (-height * heightScale * texAspect, height * heightScale, 1.0f);

			// this scales the video plane to fill the screen vertically and centers it horizontally
			offset = new Vector3 ((heightScale * height * widthDiff * texAspect) / 2.0f, (height - scale.y) / 2.0f, 100.0f);

			gameObject.transform.localScale = scale;
			gameObject.transform.position = offset;
		}
	}
}
