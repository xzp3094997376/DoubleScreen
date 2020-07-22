using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DisplayIndex : int
{
    None=-1,
    Display1 = 0,
    Display2,
}
public class MultiDisplayCanvasRaycastMethod : CanvasRaycastMethod
{
    public DisplayIndex DisplayIndex
    {
        get
        {
            return displayIndex;
        }
        set
        {
            displayIndex = value;
        }
    }
    private DisplayIndex displayIndex;
    public override void Raycast(Ray ray, float distance, List<RaycastResult> raycastResults)
    {
        if (MultiDisplayUtil.GetCurrentDisplay() != displayIndex)
            return;

        base.Raycast(ray, distance, raycastResults);
    }
}
