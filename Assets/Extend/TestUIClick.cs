using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TestUIClick : MonoBehaviour
{
    public void Start()
    {
       
      //UIEvent.Get(gameObject, ButtonKey.Left).onClick = OnClick;
       // UIEvent.Get(gameObject, ButtonKey.Left).onDoubleClick = OnDoubleClick;
       UIEvent.Get(gameObject).OnHover = OnHover;
      //  UIEvent.Get(gameObject, ButtonKey.Left).onUp = OnUP;
      //  UIEvent.Get(gameObject, ButtonKey.Left).onDown = OnDown;
    }

    private void OnDown()
    {
        Debug.Log("OnDown：" + gameObject.name);
    }

    private void OnUP()
    {
        Debug.Log("OnUP："+gameObject.name);
    }
   
    private void OnDoubleClick(GameObject go)
    {
        Debug.Log("OnDoubleClick:" + go.name);
    }

    private void OnHover(GameObject go, bool bHover)
    {
        Debug.Log("OnHover:"+go.name+"bHover:"+bHover);

    }

    private void OnClick(GameObject go)
    {
        Debug.Log("OnClick:"+go.name);
    }
    public void Update()
    {
       
    }
}
