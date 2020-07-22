using UnityEngine;
using HTC.UnityPlugin.Vive;
public class GE:MonoBehaviour
{
    private static GE ins;
    public static GE Ins
    {
        get {
            if(ins==null)
            {
                GameObject go = new GameObject("GE");
                ins= go.AddComponent<GE>();
            }
            return ins;
        }
    }
    public delegate void VoidDelegateG(GameObject go);
    public  VoidDelegateG onClick;   
    public  void Update()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            if(PlateformData.GetCurrentPlatform()==PlatformType.PC)
            {
                if (GlobeData._RightRaycaster.FirstRaycastResult().gameObject != null)
                {
                    if(onClick!=null)
                    {
                        onClick(GlobeData._RightRaycaster.FirstRaycastResult().gameObject);
                    }
                }
            }                
        }
        if(ViveInput.GetPressDown(HandRole.RightHand,ControllerButton.Trigger))
        {
            if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
            {
                if (onClick != null)
                {
                    onClick(GlobeData._RightRaycaster.FirstRaycastResult().gameObject);
                }
            }
        }
    }
}
