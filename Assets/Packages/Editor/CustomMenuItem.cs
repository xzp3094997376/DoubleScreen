using HTC.UnityPlugin.Pointer3D;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomMenuItem
{
    [MenuItem("Tools/GTAFramerwork/初始根画布")]
    public static void InitRootCnavas()
    {
        GameObject targetObj = Selection.activeGameObject;

        Canvas current = targetObj.GetComponent<Canvas>();
        if (current == null) return;
        current.renderMode = RenderMode.WorldSpace;
        current.worldCamera = Camera.main;
        current.transform.localScale = Vector3.one * 0.01049f; 

        if (targetObj.GetComponent<CanvasRaycastTarget>() == null)
            targetObj.AddComponent<CanvasRaycastTarget>();

        if (targetObj.GetComponent<MultPlateformCanvas>()==null) 
            targetObj.AddComponent<MultPlateformCanvas>();
    }
    [MenuItem("Tools/GTAFramerwork/初始根模型节点")]
    public static void InitRootModel()
    {

    }
    [MenuItem("Tools/GTAFramerwork/初始框架GTA_Framework预制体")]
    public static void InitFramerworkPrefab()
    {

    }
}
