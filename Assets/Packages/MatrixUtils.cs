using UnityEngine;

internal static class MatrixUtils
{
    public static void Matrix4x4(this Transform transfrom, Matrix4x4 matrix4X4)
    {
        transfrom.localScale = matrix4X4.GetScale();
        transfrom.rotation = matrix4X4.GetRotation();
        transfrom.position = matrix4X4.GetPostion();
    }

    public static Quaternion GetRotation(this Matrix4x4 matrix4X4)
    {
        float qw = Mathf.Sqrt(1f + matrix4X4.m00 + matrix4X4.m11 + matrix4X4.m22) / 2;
        float w = 4 * qw;
        float qx = (matrix4X4.m21 - matrix4X4.m12) / w;
        float qy = (matrix4X4.m02 - matrix4X4.m20) / w;
        float qz = (matrix4X4.m10 - matrix4X4.m01) / w;
        return new Quaternion(qx, qy, qz, qw);
    }

    public static Vector3 GetPostion(this Matrix4x4 matrix4X4)
    {
        var x = matrix4X4.m03;
        var y = matrix4X4.m13;
        var z = matrix4X4.m23;
        return new Vector3(x, y, z);
    }

    public static Vector3 GetScale(this Matrix4x4 m)
    {
        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
        return new Vector3(x, y, z);
    }
}