using HTC.UnityPlugin.Pointer3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class MultInputField : MonoBehaviour
{
    private void Awake()
    {
        UIEvent.Get(this.gameObject, ButtonKey.Left).OnHover += (go, bHover) =>
        {
            if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace) return;
            PlateformData.SetCoexistValue(bHover);
        };
    }
}
