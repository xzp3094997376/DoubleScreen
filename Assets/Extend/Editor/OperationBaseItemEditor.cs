using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(OperationBaseItem))]
public class OperationBaseItemEditor : Editor
{
    OperationBaseItem _target;
    GUIStyle titleStyle3 = new GUIStyle();
    private Color c3 = new Color(105, 105, 105, 255) / 255;
    string des = "适配PC、Zspace 平台操作定义："+"\n"+
        "拖拽: PC 左键  Zspace 物理中键"+"\n"+
        "旋转：PC 右键  "+"\n"+
        "缩放：PC 滚轮  Zspace  物理左键";
    string line = "-----------------------------------------------------------------";
    private void OnEnable()
    {
        _target = (OperationBaseItem)target;
        GUIStyleState normal = new GUIStyleState() { textColor = c3 };
        titleStyle3.normal = normal;
        titleStyle3.fontStyle = FontStyle.Italic;
        titleStyle3.fontSize = 10;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        GUILayout.Label(des, titleStyle3);
        //DrawDefaultInspector();    
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("操作选项", titleStyle3);
        GUILayout.Label("缩放");
        _target._EnbaleZoom = EditorGUILayout.Toggle(_target._EnbaleZoom);
       
        GUILayout.Label("旋转");
        _target._EnableRotate = EditorGUILayout.Toggle(_target._EnableRotate);                   
        GUILayout.Label("移动");
        _target._EnableMove = EditorGUILayout.Toggle(_target._EnableMove);
        EditorGUILayout.EndHorizontal();
        if (_target._EnbaleZoom)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("缩放属性" + line, titleStyle3);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("缩放速度：", titleStyle3);
            _target._Speed = EditorGUILayout.Slider(_target._Speed, 0, 100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        if (_target._EnableRotate)
        {            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("旋转属性"+ line, titleStyle3);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("旋转速度：", titleStyle3);
            _target._RotationSpeed = EditorGUILayout.Slider(_target._RotationSpeed, 0, 100);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("限制旋转轴", titleStyle3);
            GUILayout.Label("旋转X");
           
            _target._EnableX = EditorGUILayout.Toggle(_target._EnableX);
            
            GUILayout.Label("旋转Y");
            _target._EnableY = EditorGUILayout.Toggle(_target._EnableY);
            if(_target._EnableX==false&&_target._EnableY==false)
            {
                _target._EnableRotate = false;
                _target._EnableX = true;
                _target._EnableY = true;
            }
            //GUILayout.Label("旋转Z");
            //_target._EnableZ = EditorGUILayout.Toggle(_target._EnableZ);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }       
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("缩放大小控制",titleStyle3);
        GUILayout.Label("最小值");
        _target._MinScale = EditorGUILayout.FloatField(_target._MinScale);
        GUILayout.Label("最大值");
        _target._MaxScale = EditorGUILayout.FloatField(_target._MaxScale);
        EditorGUILayout.EndHorizontal();
        EditorUtility.SetDirty(_target);
    }
}
