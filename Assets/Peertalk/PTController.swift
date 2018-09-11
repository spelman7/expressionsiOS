// example

import Foundation
import Dispatch

class PTController : NSObject {

  static let sharedInstance: PTController = PTController()

  var ptManager: PTManager!

  func PTConnect() {
    //try and establish the delegate
    ptManager = PTManager.instance
    ptManager.delegate = self

    let portNumberString = String(PORT_NUMBER)
    print("connecting on port : \(portNumberString)")
    ptManager.connect(portNumber: PORT_NUMBER)

  }

  public func sendFloatsToServer(_ f1: Float, f2: Float, f3: Float) {
    // send face rotational data as an array of floats
    let rotationalFloatArray: [Float] = [f1, f2, f3]
    ptManager.sendObject(object: rotationalFloatArray, type: PTType.faceRotationArray.rawValue)
  }

  public func sendBlendshapesToServer(_ mouthPucker: Float, eyeBlink_L: Float, eyeBlink_R: Float, eyeSquint_L: Float, eyeSquint_R: Float, cheekPuff: Float, browInnerUp: Float) {
    // send blendshapes as an array of floats
    let blendshapeFloatArray: [Float] = [mouthPucker, eyeBlink_L, eyeBlink_R, eyeSquint_L, eyeSquint_R, cheekPuff, browInnerUp]
    ptManager.sendObject(object: blendshapeFloatArray, type: PTType.blendshapeArray.rawValue)
  }

  public func sendVisageOrientationToServer(_ x: Float, y: Float, z: Float) {
    let visageOrientationArray: [Float] = [x, y, z]
    ptManager.sendObject(object: visageOrientationArray, type: PTType.visageOrientationArray.rawValue)
  }

  public func sendVisageExpressionsToServer(_ a: Float, b: Float, c: Float) {
    let visageExpressionArray: [Float] = [a, b, c]
    ptManager.sendObject(object: visageExpressionArray, type: PTType.visageExpressionArray.rawValue)
  }

  public func sendVisagePuckerToServer(_ v1x: Float, v1y: Float, v1z: Float, v2x: Float, v2y: Float, v2z: Float, v3x: Float, v3y: Float, v3z: Float, v4x: Float, v4y: Float, v4z: Float, v5x: Float, v5y: Float, v5z: Float, v6x: Float, v6y: Float, v6z: Float, v7x: Float, v7y: Float, v7z: Float, v8x: Float, v8y: Float, v8z: Float) {
    let visagePuckerArray: [Float] = [v1x, v1y, v1z, v2x, v2y, v2z, v3x, v3y, v3z, v4x, v4y, v4z, v5x, v5y, v5z, v6x, v6y, v6z, v7x, v7y, v7z, v8x, v8y, v8z]
    ptManager.sendObject(object: visagePuckerArray, type: PTType.visagePuckerArray.rawValue)
  }

  public func sendARKitOrientationToServer(_ x: Float, y: Float, z: Float) {
    let arkitOrientationArray: [Float] = [x, y, z]
    ptManager.sendObject(object: arkitOrientationArray, type: PTType.arkitOrientationArray.rawValue)
  }

  public func sendARKitExpressionsToServer(_ a: Float, b: Float, c: Float) {
    let arkitExpressionArray: [Float] = [a, b, c]
    ptManager.sendObject(object: arkitExpressionArray, type: PTType.arkitExpressionArray.rawValue)
  }

  func sendUnityMessage(_ message: String) {
    let command = message
    UnitySendMessage("MasterJoystickController", "UnityMessageInterpreter", command)
  }

  public func sendMessageToPTSimple(_ message: String) {
    ptManager.sendObject(object: message, type: PTType.PTSimpleMessage.rawValue)
  }

}

extension PTController: PTManagerDelegate {

    func peertalk(shouldAcceptDataOfType type: UInt32) -> Bool {
        return true
    }

    func peertalk(didReceiveData data: Data, ofType type: UInt32) {
        if (type == PTType.unityMessage.rawValue) {
            // convert message to a string and fire send Unity message function
            let msg = data.convert() as! String
            sendUnityMessage(msg)
        } else if (type == PTType.unityDimensions.rawValue) {
            let dimensions = data.convert() as! [Int]
            let w = "w" + String(dimensions[0])
            let h = "h" + String(dimensions[1])
            sendUnityMessage(w)
            sendUnityMessage(h)
        }
    }

    func peertalk(didChangeConnection connected: Bool) {
      if (connected == true) {
        sendUnityMessage("PTConnected")
      } else if (connected == false) {
        sendUnityMessage("PTDisconnected")
      }

    }

}
