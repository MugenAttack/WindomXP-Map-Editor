using UnityEngine;
using System.Collections;

public static class Utils
{


    public static Quaternion GetRotation(Matrix4x4 matrix)
    {
        return matrix.rotation;
    }

    public static Vector3 GetPosition(Matrix4x4 matrix)
    {
        return matrix.GetColumn(3);
    }

    public static Vector3 GetScale(Matrix4x4 m)
    {
        return m.lossyScale;
    }
}
