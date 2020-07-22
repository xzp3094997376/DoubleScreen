using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// 定义物理按键枚举
/// 值和Vive插件的ControllerButton对应
/// </summary>
public enum ButtonKey
{
    /// <summary>
    /// 左键
    /// </summary>
    Left=0,
    /// <summary>
    /// 中键
    /// </summary>
    middle=2,
    /// <summary>
    /// 右键
    /// </summary>
    right=1
}
/// <summary>
/// 事件监听要用 +=
/// 取消监听用-=
/// </summary>
public class UIEvent : EventTrigger
{
    public delegate void VoidDelegate();
    public delegate void VoidDelegateG(GameObject go);
    public delegate void VoidDelegate_(GameObject go, PointerEventData data);
    public delegate void VoidHoverDelegate(GameObject go, bool bHover);

    public VoidHoverDelegate OnHover;
    public VoidDelegateG onClick;
    public VoidDelegateG onDoubleClick;
    public VoidDelegate onDown;
    public VoidDelegate onUp;
    public VoidDelegateG onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate_ onClick_;
    public VoidDelegate_ onUp_;
    public VoidDelegate_ onBeginDrag;
    public VoidDelegate_ onDrag;
    /// <summary>
    /// 定义物理按键  左、中、右
    /// </summary>
    private ButtonKey buttonKey;
    static public UIEvent Get(GameObject go, ButtonKey buttonKey)
    {
        UIEvent listener = go.GetComponent<UIEvent>();
        if (listener == null) listener = go.AddComponent<UIEvent>();
        listener.buttonKey = buttonKey;
        return listener;
    }
    static public UIEvent Get(Transform transform,ButtonKey buttonKey)
    {
        UIEvent listener = transform.GetComponent<UIEvent>();
        if (listener == null) listener = transform.gameObject.AddComponent<UIEvent>();
        listener.buttonKey = buttonKey;
        return listener;
    }
    static public UIEvent Get(GameObject go)
    {
        UIEvent listener = go.GetComponent<UIEvent>();
        if (listener == null) listener = go.AddComponent<UIEvent>();
        return listener;
    }
    static public UIEvent Get(Transform transform)
    {
        UIEvent listener = transform.GetComponent<UIEvent>();
        if (listener == null) listener = transform.gameObject.AddComponent<UIEvent>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick:"+ eventData.pointerId);
        if (!ButtonKeySitch(eventData.pointerId))
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2)
        {
            if (onDoubleClick != null)
            {
                onDoubleClick(gameObject);
            }
        }
        else if ( eventData.clickCount == 1)
        {          
            if (onClick != null) onClick(gameObject);
            if (onClick_ != null) onClick_(gameObject, eventData);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (ButtonKeySitch(eventData.pointerId))
        {
            if (onDown != null) onDown();
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter："+gameObject.name+Time.time);
        if (OnHover != null) OnHover(gameObject, true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit：" + gameObject.name + Time.time);
        if (OnHover != null) OnHover(gameObject, false);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (ButtonKeySitch(eventData.pointerId))
        {
            if (onUp != null) onUp();
            if (onUp_ != null) onUp_(gameObject, eventData);
        }     
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect();
       
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if(onBeginDrag!=null)
        {
            onBeginDrag(gameObject, eventData);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if(onDrag!=null)
        {
            onDrag(gameObject, eventData);
        }
    }   
    private bool ButtonKeySitch(int pointerid)
    {
        //PointerEventData .pointerId
        //鼠标点击时的id= -1,-2,-3分别对应鼠标左键，右键和中键
        //zspace 点击时id=-5，-6，-7， 分别对应鼠标左键，右键和中键
        int key =-10;
        switch (pointerid)
        {
            case -1:
                key = 0;
                break;
            case -2:
                key = 1;
                break;
            case -3:
                key = 2;
                break;
            case -5:
                key = 0;
                break;
            case -6:
                key = 1;
                break;
            case -7:
                key = 2;
                break;
        }
      return  (int)buttonKey == key;
    }
}
