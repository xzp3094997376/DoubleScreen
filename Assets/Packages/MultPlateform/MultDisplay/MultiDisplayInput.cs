using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class MultiDisplayInput : InputField
{
    protected override void Awake()
    {
        base.Awake();
        var ray = gameObject.GetComponent<MultiDisplayGraphicRaycaster>();
        InheritGraphicRaycaster(ray);
    }
    private void InheritGraphicRaycaster(MultiDisplayGraphicRaycaster caster)
    {
        var parentRaycaster = gameObject.GetComponentInParent<MultiDisplayGraphicRaycaster>();
        if (parentRaycaster != null)
        {
            caster.DisplayIndex = parentRaycaster.DisplayIndex;
        }
    }
}
