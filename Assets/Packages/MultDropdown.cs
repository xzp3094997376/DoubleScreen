using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Vive;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
[RequireComponent(typeof(Dropdown))]
public class MultDropdown : MonoBehaviour
{
    private Dropdown selfDropDown;
    private GameObject Template;
    private void Awake()
    {
        selfDropDown = this.GetComponent<Dropdown>();
        Template = selfDropDown.GetComponentInChildren<ScrollRect>(true).gameObject;

        ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Trigger, EmptyClick);
        UIEvent.Get(this.gameObject, ButtonKey.Left).OnHover += (go, bHover) =>
        {
            if (bHover == false) return;
            
            if (Template.GetComponent<CanvasRaycastTarget>()==null) 
            {
                Template.gameObject.AddComponent<CanvasRaycastTarget>();
            };
        };
        for (int i = 0; i < selfDropDown.GetComponentsInChildren<Toggle>().Length; i++)
        {
            int j = i;
            Toggle[] allToggles = selfDropDown.GetComponentsInChildren<Toggle>();
            UIEvent.Get(allToggles[j].gameObject, ButtonKey.Left).onClick_ += (go, eventData) =>
            {
                if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
                {
                    selfDropDown.OnSelect(eventData);
                }
            };
        }
        
    }
    private void OnDestroy()
    {
        ViveInput.RemovePressUp(HandRole.RightHand, ControllerButton.Trigger, EmptyClick);
    }
    private void EmptyClick()
    {
        if (GlobeData._RightRaycaster._Result.gameObject == null || !GlobeData._RightRaycaster._Result.gameObject.transform.IsChildOf(this.transform))
        {
            selfDropDown.Hide();
        }
    }
}
