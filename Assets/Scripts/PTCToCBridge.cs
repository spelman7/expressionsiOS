using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class PTCToCBridge : MonoBehaviour {

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_callPTConnect();
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_callSendFloats(float f1, float f2, float f3);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_callSendBlendshapes(float mp, float ebl, float ebr, float esl, float esr, float cp, float biu);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendVisageOrientation(float x, float y, float z);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendVisageExpressions(float a, float b, float c);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendVisagePucker(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, float v3x, float v3y, float v3z, float v4x, float v4y, float v4z, float v5x, float v5y, float v5z, float v6x, float v6y, float v6z, float v7x, float v7y, float v7z, float v8x, float v8y, float v8z);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendARKitOrientation(float x, float y, float z);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendARKitExpressions(float a, float b, float c);
	#endif

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _ex_SendPTSimpleMessage(string message);
	#endif


	public static void CallPTConnect() {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_callPTConnect();
		#endif
	}

	public static void CallSendFloats(float f1, float f2, float f3) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_callSendFloats(f1, f2, f3);
		#endif
	}

	public static void CallSendBlendshapes(float bs1, float bs2, float bs3, float bs4, float bs5, float bs6, float bs7) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_callSendBlendshapes(bs1, bs2, bs3, bs4, bs5, bs6, bs7);
		#endif
	}

	public static void CallSendVisageOrientation(float x, float y, float z) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendVisageOrientation(x, y, z);
		#endif
	}

	public static void CallSendVisageExpressions(float a, float b, float c) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendVisageExpressions(a, b, c);
		#endif
	}

	public static void CallSendVisagePucker(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, float v3x, float v3y, float v3z, float v4x, float v4y, float v4z, float v5x, float v5y, float v5z, float v6x, float v6y, float v6z, float v7x, float v7y, float v7z, float v8x, float v8y, float v8z) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendVisagePucker(v1x, v1y, v1z, v2x, v2y, v2z, v3x, v3y, v3z, v4x, v4y, v4z, v5x, v5y, v5z, v6x, v6y, v6z, v7x, v7y, v7z, v8x, v8y, v8z);
		#endif
	}

	public static void CallSendARKitOrientation(float x, float y, float z) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendARKitOrientation(x, y, z);
		#endif
	}

	public static void CallSendARKitExpressions(float a, float b, float c) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendARKitExpressions(a, b, c);
		#endif
	}

	public static void CallSendPTSimpleMessage(string message) {
		#if UNITY_IOS && !UNITY_EDITOR
		_ex_SendPTSimpleMessage(message);
		#endif
	}
}
