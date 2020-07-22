using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class FollowOnceObjectToB
{
    public float positionX;
    public float positionY;
    public float positionZ;

    public float eX;
    public float eY;
    public float eZ;

    public float m00;
    public float m01;
    public float m02;
    public float m03;
    public float m10;
    public float m11;
    public float m12;
    public float m13;
    public float m20;
    public float m21;
    public float m22;
    public float m23;
    public float m30;
    public float m31;
    public float m32;
    public float m33;

    public float farClip;

    public static void SerializeNow(FollowOnceObjectToB c)
    {
        using (FileStream fs = new FileStream(FilePath("FollowOnceObjct.bytes"), FileMode.Create))
        {
            new BinaryFormatter().Serialize(fs, c);
            fs.Close();
            Debug.LogError("存储zspace下相机视角数据：" + FilePath("FollowOnceObjct.bytes"));
        }
    }
    public static FollowOnceObjectToB DeSerializeNow()
    {
        if (!File.Exists(FilePath("FollowOnceObjct.bytes"))) return null;
        using (FileStream fs = new FileStream(FilePath("FollowOnceObjct.bytes"), FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            FollowOnceObjectToB fo = new BinaryFormatter().Deserialize(fs) as FollowOnceObjectToB;
            fs.Close();
            return fo;
        }
    }
    public static string FilePath(string name)
    {
        return Application.dataPath + "/StreamingAssets/" + name;
    }
}
