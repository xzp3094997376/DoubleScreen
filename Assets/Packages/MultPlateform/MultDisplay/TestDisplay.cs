using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisplay : MonoBehaviour
{
    /// <summary>
    /// 主屏
    /// </summary>
    public MultPlateformCanvas mainCanvas;
    /// <summary>
    /// 副屏
    /// </summary>
    public MultPlateformCanvas subCanvas;
    // Start is called before the first frame update
    void Start()
    {
        //MultiDisplayInit.Instance.AddCanvas(mainCanvas);
        //MultiDisplayInit.Instance.AddCanvas(subCanvas);
    }

   

    // Update is called once per frame
    void Update()
    {
        MultiDisplayInit.Instance.Update(Time.time,Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.A))
        {
            MultiDisplayInit.Instance.SelectSplitScreenMode(SplitScreenMode.Single);
            mainCanvas.SetMainScreenCanvas();
            subCanvas.SetMainScreenCanvas(); ;
            subCanvas.SameScreenDisplayUISort();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MultiDisplayInit.Instance.SelectSplitScreenMode(SplitScreenMode.Double);
            subCanvas.SetSubScreenCanvas();
            subCanvas.SameScreenDisplayUISort();
        }
    }
}
