using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MultiDisplayEventSystem : EventSystem
{
    [HideInInspector]
    public DisplayIndex displayIndex;
    public bool _lock=false;

    protected override void Update()
    {

        EventSystem.current = null;
        if (_lock)
        {
            return;
        }
        if (MultiDisplayUtil.GetCurrentDisplay() == displayIndex)
        {
            if (current != this)
                current = this;
        }
        else
        {
            current = null;
            return;
        }
        if (!gameObject.GetComponent<StandaloneInputModule>())
                this.gameObject.AddComponent<StandaloneInputModule>();
        base.Update();
    }
}
