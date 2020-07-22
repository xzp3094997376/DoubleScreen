using UnityEngine;
using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class GlobeData 
{
    #region 射线 
    private static VivePoseTracker[] _posTracker;
    /// <summary>
    /// 左右手势追踪
    /// </summary>
    public static VivePoseTracker[] _PosTracker
    {
        get {
            if (_posTracker==null)
            {
                _posTracker = GameObject.FindObjectsOfType<VivePoseTracker>();
            }

            return _posTracker;
        }       
    }
    /// <summary>
    /// 获取左右手检测
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public static  Pointer3DRaycaster GetRaycsaterByHand(HandRole role)
    {
        Pointer3DRaycaster caster=null;
        for(int i=0;i< _PosTracker.Length;i++)
        {
            if (_posTracker[i].viveRole.roleValue==(int)role)
            {
                caster = _posTracker[i].GetComponentInChildren<Pointer3DRaycaster>(true);   
                if(caster!=null)
                {
                    return caster;
                }
            }         
        }
        return caster;
    }
    private static GrabItem _grabItem;
    /// <summary>
    /// 抓取对象
    /// </summary>
    public static GrabItem  _GrabItem
    {
        get { return _grabItem; }
        set { _grabItem = value; }
    }
    private static Pointer3DRaycaster _rightRaycaster;
    /// <summary>
    /// 右手检测射线
    /// </summary>
    public static Pointer3DRaycaster _RightRaycaster
    {
        get {
            if (_rightRaycaster==null)
            {
                
                _rightRaycaster = GetRaycsaterByHand(HandRole.RightHand);
            }
            return _rightRaycaster;
        }
    }
    private static Pointer3DRaycaster _leftRaycaster;
    /// <summary>
    /// 左手检测射线
    /// </summary>
    public static Pointer3DRaycaster _LeftRaycaster
    {
        get
        {
            if (_leftRaycaster == null)
            {

                _leftRaycaster = GetRaycsaterByHand(HandRole.LeftHand);
            }
            return _leftRaycaster;
        }
    }
    #endregion
    private static Transform _dragObj;
    /// <summary>
    /// 射线当前拖拽的物体
    /// </summary>
    public static Transform _DragObj
    {
        get {
            return _dragObj;
        }
        set {
            _dragObj = value;
        }
    }
    /// <summary>
    /// 获取不同屏幕的事件相机，zspace屏默认一定为1屏
    /// </summary>
    /// <returns></returns>
    public static Camera GetCurrentEventCamera()
    {
        Camera current;
        bool isFirstDisplay= MultiDisplayUtil.GetCurrentDisplay() == DisplayIndex.Display1;
        bool isSecondDisplay = MultiDisplayUtil.GetCurrentDisplay() == DisplayIndex.Display2;
        bool isZspaceDisplay = PlateformData.GetCurrentPlatform() == PlatformType.zSpace;
        if (isFirstDisplay)
        {
            current = isZspaceDisplay ? PlateformData.zCore._screenPointToRayCamera : Camera.main;
        }
        //else if (isSecondDisplay)
        //{
        //    current = GameObject.Find("SecondCamera").GetComponent<Camera>(); 
        //}
        else
        {
            current = Camera.main;
        }
        return current;
    }
}
