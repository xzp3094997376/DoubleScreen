using HTC.UnityPlugin.Pointer3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewButton : Button
    , IPointer3DPressEnterHandler
    , IPointer3DPressExitHandler
{
    private float lastButton0DownTime;
    private Vector3 lastLineEnd0Position;
    public override void OnPointerDown(PointerEventData eventData) { }

    public override void OnPointerUp(PointerEventData eventData) { }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //PC平台下鼠标会进入、Zspace平台下鼠标会进入
        base.OnPointerClick(eventData);
    }
    
    public virtual void OnPointer3DPressEnter(Pointer3DEventData eventData)
    {
        //Zspace平台下射线方式进入
        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {
            lastButton0DownTime = Time.time;
            lastLineEnd0Position = eventData.position3D;
        }
        
    }

    public virtual void OnPointer3DPressExit(Pointer3DEventData eventData)
    {
        //Zspace平台下射线方式进入
        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {
            base.OnPointerUp(eventData);
            if (!eventData.GetPress() && Time.time - lastButton0DownTime < 0.3f && (eventData.position3D - lastLineEnd0Position).magnitude < 0.1f)
            {
                base.OnPointerClick(eventData);
            }
        }
    }

}