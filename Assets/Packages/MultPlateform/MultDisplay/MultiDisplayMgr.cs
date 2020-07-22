using HTC.UnityPlugin.Pointer3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SplitScreenMode
{
    None,     //
    Single,     //单屏
    Double,     //双屏
}
public class MultiDisplayInit : Singleton<MultiDisplayInit>
{
    public int displayCount;
    private List<MultPlateformCanvas> _plateformCanvas =new List<MultPlateformCanvas>();
    List<MultPlateformCanvas> display1 = new List<MultPlateformCanvas>();
    List<MultPlateformCanvas> display2 = new List<MultPlateformCanvas>();
    // Use this for initialization    
    public SplitScreenMode splitScreenMode = SplitScreenMode.Double;
    public Transform UICanvas;//底层画布 TaskNavPanel
    public Transform UICanvasSecond;//中间层画布 TaskOprtPanel**
    public Transform UICanvasTop;//顶层画布 TaskTipsPanel
    public void SelectSplitScreenMode(SplitScreenMode splitScreenMode)
    {
        this.splitScreenMode = splitScreenMode;
        if (splitScreenMode == SplitScreenMode.Single)
        {
            
        }
        else if (splitScreenMode == SplitScreenMode.Double)
        {         
            //扩展屏幕
            ActivateDisplays();
        }
    }
    
    /// <summary>
    /// 激活扩展屏
    /// </summary>
    public void ActivateDisplays()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
    /// <summary>
    /// 设置单屏 canvas rendermode 为worldspace模式
    /// 并设置canvas 环境
    /// </summary>
    /// <param name="canvas"></param>
    public void SetMainScreenCanvas(MultPlateformCanvas canvas)
    {
        if(canvas!=null)
        {
            AddCanvas(canvas);
            canvas._Canvas.targetDisplay = (int)DisplayIndex.Display1;
            canvas._DisplayIndex = DisplayIndex.Display1;
            canvas._Canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.localPosition = Vector3.zero;
            canvas.gameObject.AddMissingComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvas.SetEnvironment();
        }      
    }
    /// <summary>
    /// 设置主屏 canvas rendermode 为worldspace模式
    /// 并设置canvas 环境
    /// </summary>
    /// <param name="canvas"></param>
    public void SetMainScreenCanvas(MultPlateformCanvas[] canvas)
    {
        if(canvas!=null)
        {
            for (int i=0;i<canvas.Length;i++)
            {
                SetMainScreenCanvas( canvas[i]);
            }
        }     
    }
    /// <summary>
    /// 设置副屏 Canvas环境
    /// 
    /// </summary>
    /// <param name="canvas"></param>
    public void SetSubScreenCanvas(MultPlateformCanvas canvas)
    {
        if (canvas != null)
        {
            AddCanvas(canvas);
            canvas._Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas._Canvas.targetDisplay = (int)DisplayIndex.Display2;
            canvas._DisplayIndex = DisplayIndex.Display2;            
            canvas.gameObject.AddMissingComponent<CanvasScaler>().referenceResolution = new Vector2(1920,1080);
        }
    }
    /// <summary>
    /// 设置副屏 Canvas环境
    /// 
    /// </summary>
    /// <param name="canvas"></param>
    public void SetSubScreenCanvas(MultPlateformCanvas[] canvas)
    {
        if (canvas != null)
        {
            for (int i = 0; i < canvas.Length; i++)
            {
                SetSubScreenCanvas(canvas[i]);
            }
        }
    }
    /// <summary>
    /// 管理canvas
    /// </summary>
    /// <param name="canvas"></param>
    public void AddCanvas(MultPlateformCanvas canvas)
    {
        if(!_plateformCanvas.Exists(x=>x==canvas))
        {
            _plateformCanvas.Add(canvas);
        }
        for(int i=0;i<_plateformCanvas.Count;i++)
        {
            if(_plateformCanvas[i]==null)
            {
                _plateformCanvas.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 处理同屏下的Canvas层级 以及加锁
    /// </summary>
    /// <param name="canvas"></param>
    public void SameScreenDisplayUISort(MultPlateformCanvas canvas)
    {     
        MultPlateformCanvas[] cs= _plateformCanvas.Where(x => x._DisplayIndex == canvas._DisplayIndex)as MultPlateformCanvas[];
        canvas.multiDisplayEvent._lock = false;

        canvas._Canvas.sortingOrder = 10;
        if (cs!=null)
        {
            canvas._Canvas.sortingOrder = cs.Length + 1;
            for (int i = 0; i < cs.Length; i++)
            {
                cs[i].multiDisplayEvent._lock = true;
                cs[i]._Canvas.sortingOrder = i;
            }
        }
        
    }
    public override void Update(float fTime, float fDTime)
    {
        base.Update(fTime, fDTime);
        CheckCanvasByScreen();
    }
    private void CheckCanvasByScreen()
    {
        if(_plateformCanvas.Count==0)
        {
            return;
        }
         int length = _plateformCanvas.Count;
       
        if(splitScreenMode==SplitScreenMode.Single)//直接处理
        {
            //全部加锁
            SetCanvasEnable(_plateformCanvas, false,true);
            //取层级最高的canvas 打开检查事件，锁关闭
            _plateformCanvas.Sort((x, y) => { return x._Canvas.sortingOrder.CompareTo(y._Canvas.sortingOrder); });
            _plateformCanvas[length - 1].graphic.enabled = true;
            _plateformCanvas[length - 1].multiDisplayEvent._lock = false;
        }
        else if(splitScreenMode == SplitScreenMode.Double)
        {
            display1.Clear();
            display2.Clear();
            display1 = _plateformCanvas.Where(x => x._DisplayIndex == DisplayIndex.Display1).ToList();
            display2 = _plateformCanvas.Where(x => x._DisplayIndex == DisplayIndex.Display2).ToList();
            if (MultiDisplayUtil.GetCurrentDisplay() == DisplayIndex.Display1)
            {               
                if(display1!=null)
                {
                    SetCanvasEnable(display1, false, true);
                    display1.Sort((x, y) => { return x._Canvas.sortingOrder.CompareTo(y._Canvas.sortingOrder); });
                    length = display1.Count;
                    if (length >= 1)
                    {
                        display1[length - 1].graphic.enabled = true;
                        display1[length - 1].multiDisplayEvent._lock = false;
                    }
                }              
                SetCanvasEnable(display2, false, true);
            }
            else if (MultiDisplayUtil.GetCurrentDisplay() == DisplayIndex.Display2)
            {
                if (display2 != null)
                {
                    SetCanvasEnable(display2, false, true);
                    display2.Sort((x, y) => { return x._Canvas.sortingOrder.CompareTo(y._Canvas.sortingOrder); });
                    length = display2.Count;
                    if(length>=1)
                    {
                        display2[length - 1].graphic.enabled = true;
                        display2[length - 1].multiDisplayEvent._lock = false;
                    }
                }
                SetCanvasEnable(display1, false, true);
            }
        }
    }
    private void SetCanvasEnable(List<MultPlateformCanvas> list,bool graphic ,bool _lock)
    {
        if(list!=null)
        {
            int length = list.Count;
            for (int i = 0; i < length; i++)
            {
                list[i].graphic.enabled = graphic;
                list[i].multiDisplayEvent._lock = _lock;
            }
        }      
    }
}
