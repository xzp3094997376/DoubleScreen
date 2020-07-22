using HTC.UnityPlugin.Pointer3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using zSpace.Core;

public enum PlatformType
{
    PC = 0,
    zSpace = 1,
    SteamVR = 2,
    GoogleVR = 3,
    OculusVR = 4,
    WaveVR = 5,
    Android = 6,
    Ios = 7,
    Auto=8,
}
public class PlateformData : MonoBehaviour
{
    public static FollowOnceObject zspaceData;
    public static ZCore zCore;
    public static GameObject Line;
    public PlatformType DefalutPlatform;
    private PlatformType editorPlateform = PlatformType.Auto;
    private PlatformType EditorPlateform
    {
        get
        {
            return editorPlateform;
        }
        set
        {
            SetCoexistValue(value == PlatformType.zSpace);
            if (editorPlateform != value)
            {
                //Debug.LogError(editorPlateform + "  " + value.ToString());
                PlayerPrefs.SetString("CurrentPlateform", value.ToString());
                if (MultPlateformEvent.SwitchPlateform != null)
                    MultPlateformEvent.SwitchPlateform(editorPlateform, value);
            }
            editorPlateform = value;
        }
    }

    private IEnumerator Start()
    {
        zCore = this.GetComponentInChildren<ZCore>(true);
        zCore.enabled = false;
        if(Camera.main.GetComponent<ZCore.CameraRenderCallbacks>()!=null)
            Camera.main.GetComponent<ZCore.CameraRenderCallbacks>().enabled = false;

        Line = this.GetComponentsInChildren<GuideLineDrawer>(true)[0].gameObject;

        yield return new WaitForEndOfFrame();
        if (DefalutPlatform != PlatformType.Auto)
        {
            EditorPlateform = DefalutPlatform;
        }
        else
        {
            GetRuntimePlatform();
        }
    }
    void GetRuntimePlatform()
    {
        string platPath = Application.streamingAssetsPath + "/PlatformSelect.txt";

        string platValue = "Other";
        try
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(platPath))
            {
                platValue = sr.ReadLine();
            }
        }
        catch
        {

        }

        Debug.LogError("平台值：" + platValue);

        if (Enum.IsDefined(typeof(ZCore.DisplayType), platValue))
        {
            ZCore.DisplayType CurrectPlatform = (ZCore.DisplayType)Enum.Parse(typeof(ZCore.DisplayType), platValue);
            EditorPlateform = CurrectPlatform == ZCore.DisplayType.zSpace ? PlatformType.zSpace : PlatformType.PC;
        }
        else
        {
            EditorPlateform = PlatformType.PC;
        }

        Debug.LogError("自动获取当前平台:" + EditorPlateform.ToString());

    }
    private void OnDisable()
    {
        EditorPlateform = DefalutPlatform;
        //Debug.LogError(EditorPlateform);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EditorPlateform = PlatformType.zSpace;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            EditorPlateform = PlatformType.PC;
        }
    }
#endif
    public static PlatformType GetCurrentPlatform()
    {
        return (PlatformType)Enum.Parse(typeof(PlatformType), PlayerPrefs.GetString("CurrentPlateform", "PC"));
    }
    public static void SetLineActive(bool isShow)
    {
        Line.SetActive(isShow);
    }
    public static void SetCoexistValue(bool isCoexist)
    {
        if(Pointer3DInputModule.Instance!=null)
            Pointer3DInputModule.Instance.SetCoexistValue(isCoexist);
    }
}
