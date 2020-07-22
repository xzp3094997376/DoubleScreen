using System.IO;
using UnityEngine;

public class GetCameraDepth : MonoBehaviour
{
    public Shader replaceShader;

    public float depthScale = 1;
    public float depthCameraNearMove = 0.0f;

    protected Camera depthCamera;

    void Awake()
    {
        depthCamera = GetComponent<Camera>();

        float cameraNear = depthCamera.nearClipPlane;
        cameraNear += depthCameraNearMove;
        float cameraFar = depthCamera.farClipPlane;
        cameraFar = cameraFar * depthScale;

        StreamReader sr = new StreamReader(FollowOnceObjectToB.FilePath("3DScale.txt"));
        string line;
        int index = 0;
        while ((line = sr.ReadLine()) != null)
        {
            //Debug.Log(line);
            if (index == 0)
            {
                float scale = float.Parse(line);
                ReplaceDepthScale(scale);
                //m_farInput.text = scale.ToString();
            }
            else
            {
                float move = float.Parse(line);
                ReplaceDepthCameraNearMove(move);
                //m_nearInput.text = move.ToString();
            }
            index++;
        }
        sr.Close();

        if (sr == null)
        {
            Shader.SetGlobalFloat("_DepthShaderCameraNear", cameraNear);
            Shader.SetGlobalFloat("_DepthShaderCameraFar", cameraFar);
            
        }

        //StartCoroutine(ChangeShader());
        depthCamera.SetReplacementShader(replaceShader, "");
    }

    //private void Update()
    //{
    //    Debug.Log(Shader.GetGlobalFloat("_debug"));
    //}

    public void ReplaceDepthScale(float depthScale)
    {
        float cameraFar = depthCamera.farClipPlane;
        cameraFar = cameraFar * depthScale;
        //Debug.Log("设置cameraFar为" + cameraFar);
        Shader.SetGlobalFloat("_DepthShaderCameraFar", cameraFar);
    }

    public void ReplaceDepthCameraNearMove(float nearMove)
    {
        float num = this.depthCamera.nearClipPlane;
        num += nearMove;// this.depthCameraNearMove;
        //Debug.Log("设置num为" + num);
        Shader.SetGlobalFloat("_DepthShaderCameraNear", num);
    }

    //IEnumerator ChangeShader()
    //{
    //    string path = Application.dataPath;

    //    WWW www = new WWW("file://" + path + "/StreamingAssets/EncryptAlone/DepthShader");
    //    yield return www;
    //    if (www.error != null)
    //        throw new System.Exception("WWW download had an error:" + www.error);
    //    AssetBundle bundle = www.assetBundle;
    //    TextAsset textAsset = www.assetBundle.LoadAllAssets<TextAsset>()[0];

    //    byte[] useBytes = textAsset.bytes;
    //    DecipherAsset(useBytes);
    //    bundle.Unload(true);
    //    www.Dispose();
    //    //System.IO.File.WriteAllText(Application.dataPath + "/StreamingAssets/AA.txt", System.Text.Encoding.Unicode.GetString(useBytes));
    //    //System.IO.File.WriteAllBytes(Application.dataPath + "/StreamingAssets/AA.txt", useBytes);
    //    AssetBundle ab = AssetBundle.LoadFromMemory(useBytes);
    //    //TextAsset[] ta = ab.LoadAllAssets<TextAsset>();
    //    Shader[] shaders = ab.LoadAllAssets<Shader>();
    //    Debug.Log(shaders.Length);
    //    replaceShader = shaders[0];
    //    depthCamera.SetReplacementShader(replaceShader, "");
    //    //depthCamera = GetComponent<Camera>();
    //    //depthCamera.targetTexture = depthTexture;
    //}
    //private static void DecipherAsset(byte[] data)
    //{
    //    int index = data.Length / 2;
    //    byte b = data[index];
    //    data[index] = (byte)~b;
    //}

    //截图功能。
    //int i = 0;
    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    if (i >= 5)
    //    {
    //        Debug.Log("Done");
    //        return;
    //    }
    //    Texture2D texture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
    //    RenderTexture.active = source;
    //    texture.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
    //    texture.Apply();
    //    RenderTexture.active = null;
    //    byte[] pngBytes = texture.EncodeToPNG();
    //    System.IO.FileStream file = System.IO.File.Open(Application.streamingAssetsPath + "/Texture_" + i.ToString() + ".png", System.IO.FileMode.Create);
    //    System.IO.BinaryWriter writer = new System.IO.BinaryWriter(file);
    //    writer.Write(pngBytes);
    //    file.Close();
    //    i++;
    //}

    //void OnGUI()
    //{
    //    GUILayout.Label("_DepthShaderCameraFar==" + Shader.GetGlobalFloat("_DepthShaderCameraFar") + "       _DepthShaderCameraNear==" + Shader.GetGlobalFloat("_DepthShaderCameraNear"));
    //   
    //}
}
