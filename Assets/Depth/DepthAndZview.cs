using System;
using UnityEngine;
using UnityEngine.UI;

public class DepthAndZview : MonoBehaviour
{
    public void OnClickZview()
    {
        if (MultPlateformEvent.OpenZView != null)
        {
            MultPlateformEvent.OpenZView();
        }

    }

    public void OnClickLy()
    {
        if (MultPlateformEvent.OpenLy3d != null)
        {
            MultPlateformEvent.OpenLy3d();
        }
    }
}
