//Example.mm
#import <Foundation/Foundation.h>
#import <dispatch/dispatch.h>
#import <netinet/in.h>
#import <sys/socket.h>
#import "unityswift-Swift.h"    // Required
                                // This header file is generated automatically when Xcode build runs.


extern "C" {
    void _ex_callPTConnect() {
        //[PTController PTConnect];
        [[PTController sharedInstance] PTConnect];
    }
}

extern "C" {
    void _ex_callSendFloats(float f1Send, float f2Send, float f3Send) {
        //[PTController sendFloatsToServer:f1Send f2:f2Send f3:f3Send];
        [[PTController sharedInstance] sendFloatsToServer:f1Send f2:f2Send f3:f3Send];
    }
}

extern "C" {
    void _ex_callSendBlendshapes(float mp, float ebl, float ebr, float esl, float esr, float cp, float biu) {
        //[PTController sendBlendshapesToServer:mp eyeBlink_L:ebl eyeBlink_R:ebr eyeSquint_L:esl eyeSquint_R:esr cheekPuff:cp browInnerUp:biu];
        [[PTController sharedInstance] sendBlendshapesToServer:mp eyeBlink_L:ebl eyeBlink_R:ebr eyeSquint_L:esl eyeSquint_R:esr cheekPuff:cp browInnerUp:biu];
    }
}

extern "C" {
    void _ex_SendVisageOrientation(float x, float y, float z) {
        [[PTController sharedInstance] sendVisageOrientationToServer:x y:y z:z];
    }
}

extern "C" {
    void _ex_SendVisageExpressions(float a, float b, float c) {
        [[PTController sharedInstance] sendVisageExpressionsToServer:a b:b c:c];
    }
}

extern "C" {
    void _ex_SendVisagePucker(float v1x, float v1y, float v1z, float v2x, float v2y, float v2z, float v3x, float v3y, float v3z, float v4x, float v4y, float v4z, float v5x, float v5y, float v5z, float v6x, float v6y, float v6z, float v7x, float v7y, float v7z, float v8x, float v8y, float v8z) {
        [[PTController sharedInstance] sendVisagePuckerToServer:v1x v1y:v1y v1z:v1z v2x:v2x v2y:v2y v2z:v2z v3x:v3x v3y:v3y v3z:v3z v4x:v4x v4y:v4y v4z:v4z v5x:v5x v5y:v5y v5z:v5z v6x:v6x v6y:v6y v6z:v6z v7x:v7x v7y:v7y v7z:v7z v8x:v8x v8y:v8y v8z:v8z];
    }
}

extern "C" {
    void _ex_SendARKitOrientation(float x, float y, float z) {
        [[PTController sharedInstance] sendARKitOrientationToServer:x y:y z:z];
    }
}

extern "C" {
    void _ex_SendARKitExpressions(float a, float b, float c) {
        [[PTController sharedInstance] sendARKitExpressionsToServer:a b:b c:c];
    }
}

extern "C" {
    void _ex_SendPTSimpleMessage(const char *msg) {
        [[PTController sharedInstance] sendMessageToPTSimple:[NSString stringWithUTF8String:msg]];
    }
}
