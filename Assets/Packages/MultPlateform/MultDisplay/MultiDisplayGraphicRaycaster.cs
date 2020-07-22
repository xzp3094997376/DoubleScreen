using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MultiDisplayGraphicRaycaster : GraphicRaycaster
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

    [SerializeField]
    private DisplayIndex displayIndex;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (MultiDisplayUtil.GetCurrentDisplay() != displayIndex)
            return;

        base.Raycast(eventData, resultAppendList);
    }
}
