using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zSpace.Core;
public class MultPlateformZcore : MonoBehaviour
{
    public GameObject ZCamerPrefab;
    
    private GameObject PcCamera;
    private GameObject ZspaceCamera;
    public static MultPlateformZcore Instance;
    private Vector3 _cameraOriginalPosition;
    private Quaternion _cameraOriginalRotation;
    private Matrix4x4 _cameraOriginalProjectionMatrix;
    private void Awake()
    {
        Instance = this;
        
        PcCamera = Camera.main.gameObject;
        MultPlateformEvent.SwitchPlateform += SwitchPlateformChangeZcore;
        
    }
    private void SwitchPlateformChangeZcore(PlatformType arg1, PlatformType arg2)
    {
        //Debug.LogError(arg1 + "        " + arg2);
        if (arg2 == PlatformType.zSpace)
        {
            SetzSpaceEnvironment();
        }
        if (arg2 == PlatformType.PC)
        {
            SetPcEnvironment();
        }
    }
    private void OnCameraPreRender(Camera sender)
    {
        // Cache the camera's original position, rotation, and projection matrix.
        _cameraOriginalPosition = sender.transform.position;
        _cameraOriginalRotation = sender.transform.rotation;
        _cameraOriginalProjectionMatrix = sender.projectionMatrix;
        
        //sender.transform.position = PlateformData.zspaceData.poisition;
        //sender.transform.eulerAngles = PlateformData.zspaceData.eulerAngles;
        //sender.projectionMatrix = PlateformData.zspaceData.projectionMatrix;
    }

    private void OnCameraPostRender(Camera sender)
    {
        // Restore the camera's original position, rotation, and projection matrix.
        sender.transform.position = _cameraOriginalPosition;
        sender.transform.rotation = _cameraOriginalRotation;
        sender.projectionMatrix = _cameraOriginalProjectionMatrix;
    }
    private void SetPcEnvironment()
    {
        Camera pCamera = PcCamera.GetComponent<Camera>();
        PlateformData.SetLineActive(false);
        //pCamera.enabled = true;
        PcCameraRenderCallbacks _cameraRenderCallbacks = pCamera.gameObject.AddComponent<PcCameraRenderCallbacks>();
        //pCamera.transform.position = PlateformData.zspaceData.poisition;
        //pCamera.transform.eulerAngles = PlateformData.zspaceData.eulerAngles;
        //pCamera.projectionMatrix = PlateformData.zspaceData.projectionMatrix;
        _cameraRenderCallbacks.PreRender += OnCameraPreRender;
        _cameraRenderCallbacks.PostRender += OnCameraPostRender;
        //if(this.GetComponentInParent<PlateformData>()!=null)
        //    this.GetComponentInParent<PlateformData>().ZspaceVector3 = PlateformData.zspaceData.eulerAngles;

        ZspaceCamera = pCamera.gameObject;
        if (ZspaceCamera.GetComponent<ZCore.CameraRenderCallbacks>() != null)
        {
            ZCore.CameraRenderCallbacks ZspaceCameraRenderCallbacks = ZspaceCamera.GetComponent<ZCore.CameraRenderCallbacks>();
                ZspaceCameraRenderCallbacks.enabled = false;
        }
        if (PlateformData.zCore.enabled)
        {
            PlateformData.zCore.enabled = false;
        }
    }
    private void SetzSpaceEnvironment()
    {
        PlateformData.SetLineActive(true);
        PlateformData.zCore = this.gameObject.GetComponentInChildren<ZCore>(true);
        //this.GetComponentInParent<PlateformData>().ZspaceVector3 = PlateformData.zCore.GetViewportWorldRotation().eulerAngles;
        PcCameraRenderCallbacks _cameraRenderCallbacks = PcCamera.GetComponent<PcCameraRenderCallbacks>()?? PcCamera.AddComponent<PcCameraRenderCallbacks>();
        _cameraRenderCallbacks.PreRender -= OnCameraPreRender;
        _cameraRenderCallbacks.PostRender -= OnCameraPostRender;
        DestroyImmediate(PcCamera.GetComponent<PcCameraRenderCallbacks>());
        //PcCamera.GetComponent<Camera>().enabled = false;
        //ZspaceCamera = Instantiate(ZCamerPrefab, PcCamera.transform);
        ZspaceCamera = PcCamera.gameObject;
        PlateformData.zCore.enabled = true;
        ZCore.CameraRenderCallbacks ZspaceCameraRenderCallbacks = ZspaceCamera.GetComponentInChildren<ZCore.CameraRenderCallbacks>(true);
        if(ZspaceCameraRenderCallbacks!=null)
            ZspaceCameraRenderCallbacks.enabled = true;
        PlateformData.zCore.CurrentCameraObject = ZspaceCamera;
        StartCoroutine(WaitUpdate());
    }
    private IEnumerator WaitUpdate()
    {
        yield return new WaitForEndOfFrame();
        PlateformData.zCore.SetViewportWorldTransform(
                            Vector3.zero,
                            Quaternion.Euler(Vector3.zero),
                            38.95f);
    }
    private void OnDestroy()
    {
        MultPlateformEvent.SwitchPlateform -= SwitchPlateformChangeZcore;
    }
}
