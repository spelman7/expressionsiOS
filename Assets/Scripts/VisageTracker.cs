using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

/** Enum used for tracking status.
 */
public enum TrackStatus
{
	Off = 0,
	Ok = 1,
	Recovering = 2,
	Init = 3
}


public class VisageTracker : MonoBehaviour
{
	[Header("Master Tracking Controller")]
	public MasterTrackingController masterTrackingController;

	[Header("Configuration Paths")]
	// Tracker config files on iOS
	public string ConfigFileIOS;
	public string ConfigFileEditor;
	public string licenseFileFolder;
	public string licenseFileName;
	// Probably unnecessary config files
	[HideInInspector]
	public string ConfigFileStandalone;
	public string ConfigFileAndroid;
	public string ConfigFileOSX;


	// Mesh & Texture Parameters
	[Header("Mesh & Texture Parameters")]
	public const int MaxVertices = 1024;
	public const int MaxTriangles = 2048;
	public int VertexNumber;
	public int TriangleNumber;
	public Vector2[] TexCoords;
	public Vector3[] Vertices;
	public int[] Triangles;
	private float[] vertices = new float[MaxVertices * 3];
	private int[] triangles = new int[MaxTriangles * 3];
	private float[] texCoords = new float[MaxVertices * 2];
	private MeshFilter meshFilter;
	public TextAsset TexCoordinatesFile;
	private Vector2[] modelTexCoords; 

	// Orientation Parameters
	[Header("Orientation & Expression Parameters")]
	public Vector3 Translation;
	public Vector3 Rotation;
	public float[] ActionUnitValues;
	private float[] values = new float[22];

	// Video Camera Display Parameters
	[Header("Video Camera Display Parameters")]
	public Material CameraViewMaterial;
	public float Focus;
	public int ImageWidth = 360;
	public int ImageHeight = 480;
	public int TexWidth = 512;
	public int TexHeight = 512;
	private Texture2D texture = null;
	private Color32[] texturePixels;
	private GCHandle texturePixelsHandle;
	public bool isTracking = false;
	public int TrackerStatus = 0;

	// Camera Settings
    [Header("Camera Settings")]
    public int Orientation = 0;
    private int currentOrientation = 0;
	public int isMirrored = 1;
	public int device = 0;
    private int currentDevice = 0;
    public int defaultCameraWidth = -1;
    public int defaultCameraHeight = -1;
   
	//private bool AppStarted = false;

    void Start ()
	{

		// get mesh filter component
		meshFilter = GetComponent<MeshFilter>();

		// create a new mesh
		meshFilter.mesh = new Mesh();

		// get model texture coordinates
		modelTexCoords = GetTextureCoordinates();

		Translation = new Vector3 (0, 0, 0);
		Rotation = new Vector3 (0, 0, 0);

		// choose config file
		//string configFilePath = Application.streamingAssetsPath + "/" + ConfigFileStandalone;
		//string licenseFilePath = Application.streamingAssetsPath + "/" + licenseFileFolder;

		//configFilePath = "Data/Raw/Visage Tracker/" + ConfigFileIOS;
		//licenseFilePath = "Data/Raw/Visage Tracker/" + licenseFileName;

		// initialize tracker
		//InitializeTracker (configFilePath, licenseFilePath);
		
		// check orientation and start cam
		//Orientation = GetDeviceOrientation ();

		//OpenCamera (Orientation, device, defaultCameraWidth, defaultCameraHeight, isMirrored);

		//isTracking = true;

		//if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
		//	Debug.Log ("Notice: if graphics API is set to OpenGLCore, the texture might not get properly updated.");
	}

	void Update ()
	{
		
		#if (UNITY_IPHONE || UNITY_ANDROID) && UNITY_EDITOR
		// no tracking on ios while in editor
		return;
		#endif

		if (masterTrackingController.CurrentTracker != 0) {
			return;
		}

		if (isTracking) {

			// find device orientation
			currentOrientation = GetDeviceOrientation ();

			// check if orientation or camera device changed
			if (currentOrientation != Orientation || currentDevice != device) 
			{
				OpenCamera (currentOrientation, currentDevice, defaultCameraWidth, defaultCameraHeight, isMirrored);
				Orientation = currentOrientation;
				device = currentDevice;
				texture = null;
			}
				
			// grab current frame and start face tracking
			VisageTrackerNative._grabFrame ();

			TrackerStatus = VisageTrackerNative._track ();

			// update tracker status and translation and rotation
			VisageTrackerNative._get3DData (out Translation.x, out Translation.y, out Translation.z, out Rotation.x, out Rotation.y, out Rotation.z);

			this.transform.position = Translation;
			this.transform.rotation = Quaternion.Euler (Rotation);

            VisageTrackerNative._getCameraInfo (out Focus, out ImageWidth, out ImageHeight);

			float aspect = ImageWidth / (float)ImageHeight;

			float yRange = (ImageWidth > ImageHeight) ? 1.0f : 1.0f / aspect;

			Camera.main.fieldOfView = Mathf.Rad2Deg * 2.0f * Mathf.Atan (yRange / Focus);

			VisageTrackerNative._getFaceModel (out VertexNumber, vertices, out TriangleNumber, triangles, texCoords);

			// vertices
			if (Vertices.Length != VertexNumber)
				Vertices = new Vector3[VertexNumber];

			for (int i = 0; i < VertexNumber; i++) {
				Vertices [i] = new Vector3 (vertices [i * 3 + 0], vertices [i * 3 + 1], vertices [i * 3 + 2]);
			}

			// triangles
			if (Triangles.Length != TriangleNumber)
				Triangles = new int[TriangleNumber * 3];

			for (int i = 0; i < TriangleNumber * 3; i++) {
				Triangles [i] = triangles [i];
			}

			// tex coords
			if (TexCoords.Length != VertexNumber)
				TexCoords = new Vector2[VertexNumber];

			for (int i = 0; i < VertexNumber; i++) {
				TexCoords[i] = new Vector2(modelTexCoords[i].x, modelTexCoords[i].y); //new Vector2 (texCoords [i * 2 + 0], texCoords [i * 2 + 1]);
			}

			// action unit values
			VisageTrackerNative._getActionUnitValues (values);
			ActionUnitValues = values;

		}

		RefreshImage();

		meshFilter.mesh.Clear();

	}

	public void VisageTrackerStart() {
		
		// choose config file
		string configFilePath = Application.streamingAssetsPath + "/" + ConfigFileStandalone;
		string licenseFilePath = Application.streamingAssetsPath + "/" + licenseFileFolder;

		configFilePath = "Data/Raw/Visage Tracker/" + ConfigFileIOS;
		licenseFilePath = "Data/Raw/Visage Tracker/" + licenseFileName;

		// initialize tracker
		InitializeTracker (configFilePath, licenseFilePath);

		// check orientation and start cam
		Orientation = GetDeviceOrientation ();

		OpenCamera (Orientation, device, defaultCameraWidth, defaultCameraHeight, isMirrored);

		isTracking = true;

		if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
			Debug.Log ("Notice: if graphics API is set to OpenGLCore, the texture might not get properly updated.");
	}


	public bool InitializeTracker (string config, string license) {
		Debug.Log ("Visage Tracker: Initializing tracker with config: '" + config + "'");

		#if (UNITY_IPHONE || UNITY_ANDROID) && UNITY_EDITOR
		return false;
		#endif

		Shader shader = Shader.Find("Custom/BGRATex");
		CameraViewMaterial.shader = shader;

        // initialize tracker
        VisageTrackerNative._initTracker(config, license);
		return true;
	}

	// if width and height are -1, values will be set internally
	void OpenCamera (int orientation, int currDevice, int width, int height, int mirrored) {
		VisageTrackerNative._openCamera (orientation, currDevice, width, height, mirrored);

	}

	public void OpenCameraFromMasterTrackerController() {
		OpenCamera (currentOrientation, currentDevice, defaultCameraWidth, defaultCameraHeight, isMirrored);
	}

	public void CloseCamera () {
		VisageTrackerNative._closeCamera ();
	}

	void OnDestroy () {
		CloseCamera ();
	}


	// returns current device orientation
	int GetDeviceOrientation () {
		int devOrientation;


		if (Input.deviceOrientation == DeviceOrientation.Portrait)
			devOrientation = 0;
		else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
			devOrientation = 2;
		else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
			devOrientation = 3;
		else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
			devOrientation = 1;
		else if (Input.deviceOrientation == DeviceOrientation.FaceUp)
			devOrientation = Orientation;
		else 
			devOrientation = 0;

		return devOrientation;
	}





	void RefreshImage () {
		// create texture
		if (texture == null && isTracking) {
			TexWidth = Convert.ToInt32 (Math.Pow (2.0, Math.Ceiling (Math.Log (ImageWidth) / Math.Log (2.0))));
			TexHeight = Convert.ToInt32 (Math.Pow (2.0, Math.Ceiling (Math.Log (ImageHeight) / Math.Log (2.0))));

			texture = new Texture2D (TexWidth, TexHeight, TextureFormat.RGBA32, false);
			
			var cols = texture.GetPixels32();
            for (var i = 0; i < cols.Length; i++)
				cols[i] = Color.black;
			
			texture.SetPixels32(cols);
			texture.Apply(false);
			
		}

		if (texture != null && isTracking && TrackerStatus != 0) {
			
			CameraViewMaterial.SetTexture ("_MainTex", texture);

			if (SystemInfo.graphicsDeviceVersion.StartsWith ("Metal"))
				VisageTrackerNative._bindTextureMetal (texture.GetNativeTexturePtr ());
			else
				VisageTrackerNative._bindTexture ((int)texture.GetNativeTexturePtr ());
		} 
	}


	void Unzip()
	{
		string[] pathsNeeded = {
			"candide3.fdp",
			"candide3.wfm",
			"jk_300.fdp",
			"jk_300.wfm",
			"Head Tracker.cfg",
            "visage_powered.png",
			"warning.png",
			"bdtsdata/FF/ff.dat",
			"bdtsdata/LBF/lv",
			"bdtsdata/LBF/pr.lbf",
			"bdtsdata/NN/fa.lbf",
			"bdtsdata/NN/fc.lbf",
			"bdtsdata/LBF/pe/landmarks.txt",
			"bdtsdata/LBF/pe/W",
			"bdtsdata/LBF/pe/lp11.bdf",
			"bdtsdata/LBF/ye/lp11.bdf",
			"bdtsdata/LBF/ye/W",
			"bdtsdata/LBF/ye/landmarks.txt"
			, "license-file-name.vlc"
		};
		string outputDir;
		string localDataFolder = "Visage Tracker";
		
		StreamWriter sw;

		outputDir = Application.persistentDataPath;
		
		if (!Directory.Exists(outputDir)) 
		{
			Directory.CreateDirectory(outputDir);
		}
		foreach(string filename in pathsNeeded)
		{
			//if(!File.Exists(outputDir + "/" + filename))
			//{
			
			WWW unpacker = new WWW("jar:file://" + Application.dataPath + "!/assets/" + localDataFolder + "/" + filename);
			
			while(!unpacker.isDone){ }
			
			if(!string.IsNullOrEmpty(unpacker.error))
			{
				Debug.Log(unpacker.error);
				continue;
			}
			
			Debug.Log(filename);
			
			if (filename.Contains("/")) 
			{
				string[] split = filename.Split('/');
				string name = "";
				string folder = "";
				string curDir = outputDir;
				
				for (int i = 0; i < split.Length; i++) 
				{
					if (i == split.Length - 1)
					{
						name = split[i];
					}
					else 
					{
						folder = split[i];
						curDir = curDir + "/" + folder;                    
					}
				}
				if (!Directory.Exists(curDir))
				{
					Directory.CreateDirectory(curDir);
				}
				
				File.WriteAllBytes("/" + curDir + "/" + name, unpacker.bytes);
			}
			else
			{
				File.WriteAllBytes("/" + outputDir + "/" + filename, unpacker.bytes);
			}
			
			Debug.Log("File written " + filename + "\n");
		}
	}


	// .obj file parsing (normals excluded)
	Vector2[] GetTextureCoordinates()
	{
		List<Vector3> v = new List<Vector3>();
		List<Vector2> vt = new List<Vector2>();
		Vector2[] texCoords = new Vector2[1024];
		int indexPoint;
		int texIndexPoint;

		string text = TexCoordinatesFile.text;
		string[] lines = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

		foreach (string line in lines)
		{
			string[] lineSplit = line.Split(' ');

			// ex. "v -22.571495 -26.737658 38.676575"             
			if (lineSplit[0] == "v")
			{
				v.Add(new Vector3((float)Double.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
			}

			// ex. "vt 0.5465 0.2624"
			else if (lineSplit[0] == "vt")
			{
				vt.Add(new Vector2(float.Parse(lineSplit[1]), float.Parse(lineSplit[2])));
			}

			// ex. "f 410/410/413 399/399/413 63/60/413"
			else if (lineSplit[0] == "f")
			{
				int[] indexPoints = { 0, 0, 0 };
				int[] texIndexPoints = { 0, 0, 0 };
				for (int i = 1; i < 4; i++)
				{
					indexPoints[i - 1] = int.Parse(lineSplit[i].Split('/')[0]);
					texIndexPoints[i - 1] = int.Parse(lineSplit[i].Split('/')[1]);
				}

				for (int i = 0; i < 3; i++)
				{
					indexPoint = indexPoints[i];
					texIndexPoint = texIndexPoints[i];

					texCoords[indexPoint - 1].x = vt[texIndexPoint - 1].x;
					texCoords[indexPoint - 1].y = vt[texIndexPoint - 1].y;
				}

			}
		}

		return texCoords;
	}

}
