using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace KaizerWaldCode.RTTCamera
{
    internal static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -180f) lfAngle += 360f;
            if (lfAngle > 180) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Quaternion RotateFWorld(in Quaternion rotation, float x, float y, float z)
        {
            quaternion eulerRot = quaternion.EulerZXY(x, y, z);
            return math.mul(eulerRot, rotation);
            //Quaternion eulerRot = Quaternion.Euler(x, y, z);
            //rotation *= (Quaternion.Inverse(rotation) * eulerRot * rotation);
            //return rotation;
        }
            
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Quaternion RotateFSelf(in Quaternion localRotation, float x, float y, float z)
        {
            quaternion eulerRot = quaternion.EulerZXY(x, y, z);
            return math.mul(localRotation, eulerRot);
            //Quaternion eulerRot = Quaternion.Euler(x, y, z);
            //localRotation *= eulerRot;
            //return localRotation;
        }
    }
}
