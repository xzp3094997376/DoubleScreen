using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制扩展屏展示
/// </summary>
public class ExtendDisplay : MonoBehaviour
{
    //private string displayCount = "display count:{0}";
    private GameObject ScreenPointToRayCamera;
    public GameObject CameraBackground;

    [Header("是否开启外置摄像头（默认开启）")]
    public bool IsDefaultOpenWebCam = true;

    [Header("是否跟随ZCORE相机（用于固定相机视角）")]
    public bool followDynamicCamera = true;
    private GameObject _virtualCameraStereo;

    private void Start()
    {
        SwitchenWebCam(IsDefaultOpenWebCam);
        MultPlateformEvent.OpenLy3d += ActivateDisplays;
        _virtualCameraStereo =transform.parent.GetComponentInChildren<VirtualCameraStereo>(true).gameObject;
    }

    void OnDestroy()
    {
        MultPlateformEvent.OpenLy3d -= ActivateDisplays;
    }

    /// <summary>
    /// 激活扩展屏
    /// </summary>
    public void ActivateDisplays()
    {
        _virtualCameraStereo.SetActive(true);

        if (this.gameObject.activeSelf)
            for (int i = 0; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
    }
    private bool CheckCameraBackground()
    {
        if (CameraBackground == null)
        {
            CameraBackground = gameObject.GetComponentInChildren<CameraBackground>().gameObject;
        }
        if (CameraBackground == null)
        {
            throw new System.Exception("CameraBackground is null!");
        }
        return CameraBackground != null;
    }
    /// <summary>
    /// 开启、关闭外置摄像头模式
    /// </summary>
    public void SwitchenWebCam(bool isOpen)
    {
        if (CheckCameraBackground())
        {
            CameraBackground.SetActive(isOpen);
        }
    }
    // Update is called once per frame
    void Update()
    {
        SetParentToZcoreCamera();

    }
    /// <summary>
    /// 设置扩展屏相机到ZCORE视野相机下
    /// </summary>
    private void SetParentToZcoreCamera()
    {
        if (!ScreenPointToRayCamera)
        {
            ScreenPointToRayCamera = GameObject.Find("ScreenPointToRayCamera");

        }
        else
        {
            if (transform.parent != ScreenPointToRayCamera.transform && followDynamicCamera)
            {
                transform.SetParent(ScreenPointToRayCamera.transform, false);
            }
        }
    }
    //private void OnGUI()
    //{
    //    //GUI.Label(new Rect(120, 10, 100, 100), string.Format(displayCount, Display.displays.Length));
    //    //if (GUI.Button(new Rect(10, 10, 100, 100), "Activate"))
    //    //{
    //    //    ActivateDisplays();
    //    //}
    //}
}
