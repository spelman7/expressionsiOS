using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

public static partial class VisageTrackerNative
{
#if UNITY_STANDALONE_WIN
    /** This function initialises the tracker.
 	 * 
 	 * Implemented in VisageTrackerUnityPlugin library.
 	 */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _initTracker(string config, string license);

    /** Fills the imageData with the current frame image data.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _setFrameData(IntPtr imageData);

    /** This functions returns the current head translation, rotation and tracking status.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    #if (UNITY_64 || UNITY_EDITOR_64)
	    [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _get3DData(out float tx, out float ty, out float tz, out float rx, out float ry, out float rz);

    /** This functions returns camera info.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern bool _getCameraInfo(out float focus, out int width, out int height);

    /** This function starts face tracking on current frame and returns tracker status.
     * 
     * Implemented in VisageTrackerUnity library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern int _track();

    /**This function grabs current frame.
      * 
      * Implemented in VisageTrackerUnity library.
      */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _grabFrame();

    /** This function initializes new camera with the given orientation and camera information.
     * 
     * Implemented in VisageTrackerUnityPlugin library.
     */
    #if (UNITY_64 || UNITY_EDITOR_64)
        [DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
    public static extern void _openCamera(int orientation, int device, int width, int height, int mirrored);

   /** This function closes camera.
    * 
    * Implemented in VisageTrackerUnityPlugin library.
    */
	#if (UNITY_64 || UNITY_EDITOR_64)
		[DllImport("VisageTrackerUnityPlugin64")]
	#else
	    [DllImport("VisageTrackerUnityPlugin")]
	#endif
    public static extern void _closeCamera();

    /** This functions returns data needed to draw 3D face model.
     * 
     * Implemented in VisageTrackerUnity library.
     */
	#if (UNITY_64 || UNITY_EDITOR_64)
		[DllImport("VisageTrackerUnityPlugin64")]
    #else
	    [DllImport("VisageTrackerUnityPlugin")]
    #endif
	public static extern bool _getFaceModel (out int vertexNumber, float[] vertices, out int triangleNumber, int[] triangles, float[] texCoords);

#endif
}
