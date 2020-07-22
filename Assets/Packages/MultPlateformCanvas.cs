using HTC.UnityPlugin.Pointer3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MultPlateformCanvas : MonoBehaviour
{
    private DisplayIndex _displayIndex;
    public DisplayIndex _DisplayIndex
    {
        get {

            return _displayIndex;
        }
        set {
            _displayIndex = value;
            AddMulitDisplayScript();
        }
    }
    private Canvas _canvas;
    public Canvas _Canvas
    {
        get {
            if(_canvas==null)
            {
                _canvas = GetComponent<Canvas>();
            }
            return _canvas;
        }
    }
    private List<InputField> AllInputFields = new List<InputField>();
    private List<Dropdown> AllDropdowns = new List<Dropdown>();
    [HideInInspector]
    public GraphicRaycaster graphic;
    [HideInInspector]
    public MultiDisplayEventSystem multiDisplayEvent;
    private void Awake()
    {
        _DisplayIndex = (DisplayIndex)GetComponent<Canvas>().targetDisplay;        
    }
    private void Start()
    {
        MultPlateformEvent.SwitchPlateform += SwitchPlateformChangeCanvas;
    }
    private void Reset()
    {
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = new Vector3(0.01053f, 0.01053f, 0.01053f);
    }

    private void SwitchPlateformChangeCanvas(PlatformType arg1, PlatformType arg2)
    {
        if ((arg1 == PlatformType.PC || arg1 == PlatformType.Auto) && arg2 == PlatformType.zSpace)
        {
            SetzSpaceEnvironment(); 
        }
        if ((arg1 == PlatformType.zSpace || arg1 == PlatformType.Auto) && arg2 == PlatformType.PC)
        {
            SetPcEnvironment();
        }
    }
    private void SetPcEnvironment()
    {
        this.transform.localScale = new Vector3(0.01053f, 0.01057f, 0.01053f);
        MultPlateformData.CurrentCamera = this.GetComponent<Canvas>().worldCamera = Camera.main;
    }
    private void SetzSpaceEnvironment()
    {
        this.transform.localScale = new Vector3(0.01053f, 0.01057f, 0.01053f);
        MultPlateformData.CurrentCamera = this.GetComponent<Canvas>().worldCamera = PlateformData.zCore._screenPointToRayCamera;
        
    }
    public void SetEnvironment()
    {
        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {
            SetzSpaceEnvironment();
        }
        else if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
        {
            SetPcEnvironment();
        }
    }
    private void OnDestroy()
    {
        MultPlateformEvent.SwitchPlateform -= SwitchPlateformChangeCanvas;
    }
    public void AddMulitDisplayScript()
    {

        gameObject.AddMissingComponent<CanvasRaycastTarget>();
        multiDisplayEvent = gameObject.AddMissingComponent<MultiDisplayEventSystem>();
        multiDisplayEvent.displayIndex = _DisplayIndex;
        gameObject.AddMissingComponent<MultiDisplayCanvasRaycastMethod>().DisplayIndex = _DisplayIndex;
        gameObject.AddMissingComponent<StandaloneInputModule>();
        graphic= gameObject.AddMissingComponent<GraphicRaycaster>();      
    }
    //private void Update()
    //{
    //    CheckDisplay();
    //}
    ///// <summary>
    ///// 当前屏激活
    ///// </summary>
    //private void CheckDisplay()
    //{
    //    //当前屏激活
    //    if (MultiDisplayUtil.GetCurrentDisplay() == _DisplayIndex)
    //    {
    //        if (!graphic.enabled || !multiDisplayEvent._lock)//只执行一次
    //        {
    //            graphic.enabled = true;
    //            multiDisplayEvent._lock = false;
    //        }
    //    }
    //    else
    //    {
    //        if (graphic.enabled)//只执行一次
    //        {
    //            graphic.enabled = false;
    //            multiDisplayEvent._lock = true;
    //        }
    //    }
    //}
}
