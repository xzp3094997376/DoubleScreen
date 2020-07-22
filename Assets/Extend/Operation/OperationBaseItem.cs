using System;
using System.Collections.Generic;
using System.Linq;
using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.EventSystems;

public class RestData
{
    public Enum en;
    public Vector3 pos;
    public Vector3 angle;
    public Vector3 zoom=Vector3.one;
}
public partial class OperationBaseItem : MonoBehaviour
{
    /// <summary>
    /// 该对象 默认分身缩放
    /// </summary>
    [SerializeField, HideInInspector]
    private Transform _tran;
    /// <summary>
    /// 设置分身或者 协同缩放对象
    /// </summary>
    public Transform _Tran
    {
        get { return _tran; }
        set
        {
            _tran = value;
            _zoomGroup = _tran.GetComponent<OperationItemGroup>();
            otherScale = Vector2.zero;
        }
    }
    [SerializeField, HideInInspector]
    private OperationItemGroup _zoomGroup;
    /// <summary>
    /// -1 :表示 协同缩放，0：表示单体缩放 1：表示 分身缩放
    /// </summary>
    private int action;
    /// <summary>
    /// 是否是有效点击
    /// </summary>
    private bool validClick = false;
  
    /// <summary>
    /// 是否拖拽该节点的子节点
    /// </summary>
    private bool isChild;
    List<RestData> _listRestData=new List<RestData>();

    enum dat
    {
        v,
        z,
    }
    private void Awake()
    {
        localRoatation = transform.localEulerAngles;
        localPos = transform.localPosition;
        localSacle = transform.localScale;       
    }

    public void Reset(Enum en)
    {
        if (!_listRestData.Exists(x=>x.en==en))
        {
            OnLeftRest();
            return;
        }

        RestData data = _listRestData.First(x=>x.en==en);
        transform.localEulerAngles = data.angle;
        transform.localPosition = data.pos;
        transform.localScale = data.zoom;
       //int index=  _listRestData.FindIndex(x => x.en == en);
     //  _listRestData.RemoveAt(index);
    }


    /// <summary>
   /// 记录复位信息
   /// </summary>
   /// <param name="en"></param>
   /// <param name="data"></param>
    public void RecordResetPos(RestData data)
    {
        if (!_listRestData.Exists(x=>x.en==data.en))
        {
            _listRestData.Add(data);
        }
    }

    public void RemovRestPos()
    {
        _listRestData.Clear();
    }

    /// <summary>
    /// 移出某一个状态
    /// </summary>
    /// <param name="en"></param>
    public void RemoveRestPos(Enum en)
    {
        int index = _listRestData.FindIndex(x => x.en == en);
        _listRestData.RemoveAt(index);
    }

    private void OnEnable()
    {
       AddListener();
    }
    private void OnDisable()
    {
        RemoveListener();
    }
    private void Update()
    {      
        if (PlateformData.GetCurrentPlatform() == PlatformType.PC)//鼠标移入时
        {
            if (_enbaleZoom)
            {
                PcScale();
            }
            if (_enableRotate)
            {
                PcRotate();
            }
            if (_enableMove)
            {
                //DragOther();
            }
            if (Input.GetMouseButtonDown(1))
            {
               // OnRightRest();
            }
        }
    }
    void AddListener()
    {
        //监听右手 右键
        ViveInput.AddPress(HandRole.RightHand, ControllerButton.Pad, onRightRightPress);
        ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Pad, onRightRightPressDown);
        ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Pad, onRightRightPressUp);
        //监听右手 左键持续按下
        ViveInput.AddPress(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPress);
        ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPressDown);
        ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPressUp);

        ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Grip, OnLeftRest);
    }
    void RemoveListener()
    {
        ViveInput.RemovePressDown(HandRole.RightHand, ControllerButton.Grip, OnLeftRest);
        ViveInput.RemovePress(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPress);
        ViveInput.RemovePressDown(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPressDown);
        ViveInput.RemovePressUp(HandRole.RightHand, ControllerButton.Trigger, OnRightLeftPressUp);
        ViveInput.RemovePress(HandRole.RightHand, ControllerButton.Pad, onRightRightPress);
        ViveInput.RemovePressDown(HandRole.RightHand, ControllerButton.Pad, onRightRightPressDown);
        ViveInput.RemovePressUp(HandRole.RightHand, ControllerButton.Pad, onRightRightPressUp);
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void OnLeftRest()
    {
       
        if (_listRestData.Count == 0)
        {
            transform.localEulerAngles = localRoatation;
            transform.localPosition = localPos;
            transform.localScale = localSacle;
            Debug.Log("OnLeftRest" + localPos);
        }
        else
        {
            int cout = _listRestData.Count - 1;
          RestData data=  _listRestData[cout];
          transform.localEulerAngles = data.angle;
          transform.localPosition = data.pos;
          transform.localScale = data.zoom;
            Debug.Log("OnLeftRest" + data.pos);
            //_listRestData.RemoveAt(cout);
        }

    }
    /// <summary>
    /// 检测是否点击到子节点
    /// </summary>
    /// <returns></returns>
    private OperationBaseItem isCheckChild()
    {
        OperationBaseItem zom = null;
        if (GlobeData._RightRaycaster._Result.gameObject != null)
        {
            if (GlobeData._RightRaycaster._Result.gameObject != gameObject)
            {
                zom = GlobeData._RightRaycaster._Result.gameObject.GetComponentInParent<OperationBaseItem>();
            }
        }
        return zom;
    }
    /// <summary>
    /// 检测 是协同，单体，分身 运动
    /// </summary>
    private void CheckAction()
    {
        if (_zoomGroup == null && _Tran != null)//表示物体需要和当前的对象协同缩放
        {
            action = -1;
            hoverScale = transform.localScale;
            otherScale = _Tran.localScale;
        }
        else//表示可能做 单体缩放，也可能做 分身缩放
        {
            if (_zoomGroup == null)//表示单体缩放
            {
                action = 0;
                hoverScale = transform.localScale;
            }
            else
            {
                //分身不等于空，并且 处于分散
                if (_zoomGroup._Diffuse)
                {
                    //Debug.Log("_zoomGroup:" + _zoomGroup._Diffuse);
                    action = 0;
                    hoverScale = transform.localScale;
                }
                else//分身不等于空，并且处于 聚合状态
                {
                    //Debug.Log("_zoomGroup:"+_zoomGroup._Diffuse);
                    action = 1;
                    otherScale = _zoomGroup.transform.localScale;
                }
            }
        }
    }
}
