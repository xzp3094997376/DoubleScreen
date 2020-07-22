using UnityEngine;
//using HTC.UnityPlugin.Pointer3D;
using System;

public class TestClick : MonoBehaviour
{
    private void Start()
    {
        UIEvent.Get(gameObject).OnHover = OnHoverFun;
        //UIEvent.Get(gameObject, ButtonKey.Left).OnHover += OnHover;
    }

    private void OnHoverFun(GameObject go, bool bHover)
    {
        if (bHover)
        {
            this.GetComponent<HighlightingSystem.Highlighter>().ConstantOn(Color.red);
        }
        else
        {
            this.GetComponent<HighlightingSystem.Highlighter>().ConstantOff();
        }
    }

    private void OnHover()
    {
        
    }

    ///// <summary>
    ///// 鼠标按下
    ///// </summary>
    ///// <param name="eventData"></param>
    //public void OnPointer3DPressEnter(Pointer3DEventData eventData)
    //{
    //    Debug.Log("OnPointer3DPressEnter" + eventData.GetPressDown());
    //}
    ///// <summary>
    ///// 鼠标抬起
    ///// </summary>
    ///// <param name="eventData"></param>
    //public void OnPointer3DPressExit(Pointer3DEventData eventData)
    //{
    //    Debug.Log("OnPointer3DPressExit" + eventData.GetPressDown());
    //}
}
