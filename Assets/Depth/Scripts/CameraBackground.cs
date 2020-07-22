using UnityEngine;
using System.Collections;

public class CameraBackground : MonoBehaviour
{
    [Tooltip("默认摄像头设备名称")]
    [Header("设备名称")]
    public string WebcamName;

    private float SnipeFov = 45;
    private Camera mCamera;
    private WebCamTexture cameraTexture;
    private Quaternion baseRotation;
    private ScreenOrientation mScreenOrientation = ScreenOrientation.AutoRotation;
    private float mHeight, mWidth;
    private float finalWidth = 0, finalHeight = 0;
    private bool ChangeHeight = false;
    private int cameraNum = 0;
    private bool arRunning = false;
    private string[] webcamNames;


    // Use this for initialization
    void Awake()
    {
        mCamera = GetComponentInParent<Camera>();
        var dis = (transform.position - mCamera.transform.position).magnitude;
        float halfFOV = (mCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        mHeight = 2 * dis * Mathf.Tan(halfFOV);
        StartCoroutine(OpenCamera(cameraNum));
    }
    private IEnumerator OpenCamera(int whichcamera)
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            webcamNames = new string[WebCamTexture.devices.Length];
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                webcamNames[i] = WebCamTexture.devices[i].name;
            }
            if (!string.IsNullOrEmpty(WebcamName))
            {
                cameraTexture = new WebCamTexture(WebcamName, Screen.width, Screen.height);
            }
            else
            {
                cameraTexture = new WebCamTexture();
            }
            baseRotation = transform.rotation;
            cameraTexture.filterMode = FilterMode.Point;
            cameraTexture.requestedFPS = 25;
            if (WebCamTexture.devices.Length > 0)
            {
                cameraTexture.Play();
                GetComponent<Renderer>().material.mainTexture = cameraTexture;
#if UNITY_EDITOR
                transform.localScale = AdjustPlaneScale(mHeight);
#else
        CheckScreenOrientation();
#endif
            }
        }
    }
    void CheckScreenOrientation()
    {
        if (Screen.orientation != mScreenOrientation)
        {
            mScreenOrientation = Screen.orientation;
            switch (mScreenOrientation)
            {
                case ScreenOrientation.Portrait:
                    transform.localScale = AdjustPlaneScale(mHeight);
                    //transform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
                case ScreenOrientation.Landscape:
                    transform.localScale = AdjustPlaneScale(mHeight);
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case ScreenOrientation.LandscapeRight:
                    transform.localScale = AdjustPlaneScale(mHeight);
                    transform.localRotation = Quaternion.Euler(0, 0, 180);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    transform.localScale = AdjustPlaneScale(mHeight);
                    transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;

            }
        }
    }
    Vector3 AdjustPlaneScale(float height)
    {
        finalHeight = height;
        var camTexAspect = cameraTexture.width * 1f / cameraTexture.height;
        mWidth = finalWidth = mHeight * camTexAspect;
        if (mCamera.aspect > camTexAspect)
        {
            ChangeHeight = true;
            finalWidth = (mWidth / camTexAspect) * mCamera.aspect;
            finalHeight = (finalWidth / mWidth) * height;
        }
        var localScale = new Vector3(finalWidth, finalHeight, 1);
#if UNITY_IPHONE && !UNITY_EDITOR
        switch (mScreenOrientation)
            {
            case ScreenOrientation.LandscapeLeft:
                localScale.y *= -1;
                break;
            case ScreenOrientation.LandscapeRight:
                localScale.x *= -1;
                break;
        }
#endif
        return localScale;
    }
    void Update()
    {
        if (!arRunning) return;
#if !UNITY_EDITOR
        CheckScreenOrientation();
#endif
    }
    void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(0, 200, 200, 200));

        GUILayout.Label(string.Join(",", webcamNames));
        //if (GUILayout.Button("翻转Y"))
        //{
        //    Snipe(true);
        //}
        //if (GUILayout.Button("翻转X"))
        //{
        //    Snipe(false);
        //}
        //GUILayout.EndArea();
    }
    void OnDestroy()
    {
        StopCamera();
    }
    public byte[] GetCameraTexture()
    {
        int w = 0, h = 0, offset = 0;
        var camTexAspect = cameraTexture.width * 1f / cameraTexture.height;
        if (ChangeHeight)
        {
            w = cameraTexture.width;
            h = Mathf.CeilToInt(cameraTexture.height * (1 / camTexAspect));
            offset = Mathf.CeilToInt(cameraTexture.height - h);
        }
        else
        {
            w = Mathf.CeilToInt(cameraTexture.width * (1 / camTexAspect));
            h = cameraTexture.height;
            offset = Mathf.CeilToInt(cameraTexture.width - w);
        }

        //Debug.Log(w+"  "+h+"  "+offset+" "+ChangeHeight+"  "+ cameraTexture.width+"  "+ cameraTexture.height);
        Texture2D mTexture = new Texture2D(w, h, TextureFormat.ARGB32, false);
        mTexture.SetPixels(cameraTexture.GetPixels(ChangeHeight ? 0 : offset / 2, ChangeHeight ? offset / 2 : 0, w, h));
        mTexture.Apply();
        byte[] bt = mTexture.EncodeToJPG();
        //SaveTexture(bt);
        return bt;
    }
    private void SaveTexture(byte[] bt)
    {
        string mPhotoName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
        var mPhotoPath = Application.dataPath + "/" + mPhotoName;
        System.IO.File.WriteAllBytes(mPhotoPath, bt);
    }

    public void ChangeCamera()
    {
        StopCamera();
        cameraNum = cameraNum == 0 ? 1 : 0;
        StartCoroutine(OpenCamera(cameraNum));
    }
    public void StopCamera()
    {
        if (cameraTexture == null) return;
        cameraTexture.Stop();
        StopAllCoroutines();
    }
}
