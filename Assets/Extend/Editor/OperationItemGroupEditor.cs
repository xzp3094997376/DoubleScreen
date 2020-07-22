using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(OperationItemGroup))]
public class OperationItemGroupEditor : Editor
{
    OperationItemGroup _target;
    bool diffuse;
    bool together;

    public List<Transform> list = new List<Transform>();
    GUIStyle titleStyle1 = new GUIStyle();
    GUIStyle titleStyle2 = new GUIStyle();
    GUIStyle titleStyle3 = new GUIStyle();
    private Color c1 = new Color(46f, 163f, 256f, 256f) / 255;
    private Color c2 = new Color(192f, 192f, 192f, 256f) / 255;
    private Color c3 = new Color(105, 105, 105, 255) / 255;
    string des = "操作子对象做离散、聚和状态控制" + "\n" + "聚合状态: 整体拖拽、缩放，旋转" + "\n"
           + "离散状态: 子对象单体拖拽、缩放，旋转";
    string note = "如果改节点包含有子对象模型是组合模型，那么需要找出来并人为添加OperationBaseItem组件";
    protected void OnEnable()
    {
        _target = (OperationItemGroup)target;
        titleStyle1.fontSize = 12;
        titleStyle2.fontSize = 12;
        titleStyle2.normal.textColor = c2;
        titleStyle1.normal.textColor = c1;
        GUIStyleState normal = new GUIStyleState() { textColor = c3 };
        titleStyle3.normal = normal;
        titleStyle3.fontStyle = FontStyle.Italic;
        titleStyle3.fontSize = 10;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //DrawDefaultInspector();

        GUILayout.Label(des, titleStyle3);
        EditorGUILayout.Space();
        GUILayout.Label(note, titleStyle3);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("状态", titleStyle3);
        GUILayout.Label("离散", _target._Diffuse ? titleStyle1 : titleStyle2);
        GUILayout.Label("聚合", _target._Diffuse ? titleStyle2 : titleStyle1);
        _target._Diffuse = EditorGUILayout.Toggle(_target._Diffuse);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        if (!Application.isPlaying)
        {
            GUILayout.Label("编辑状态下使用功能", titleStyle3);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("添加拖拽旋转缩放组件"))
            {
                FindFunc(_target.transform, typeof(OperationBaseItem));
            }
            if (GUILayout.Button("复原"))
            {
                OperationBaseItem[] zooms = _target.transform.GetComponentsInChildren<OperationBaseItem>(true);
                if (zooms == null)
                {
                    return;
                }
                for (int i = 0; i < zooms.Length; i++)
                {
                    DestroyImmediate(zooms[i]);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();
        EditorUtility.SetDirty(_target);
    }

    /// <summary>
    /// transform类型递归查找子物体
    /// </summary>
    /// <returns>返回需要查找的子物体.</returns>
    /// <param name="parent">查找起点.</param>
    /// <param name="targetName">需要查找的子物体名字.</param>
    public void FindFunc(Transform parent, Type t)
    {
        //如果没有没有找到，说明没有在该子层级，则先遍历该层级所有transform，然后通过递归继续查找----再次调用该方法
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            //找到T类型组件，不再递归下去，表示该对象所有的子节点作为一个整体
            if (child.GetComponent(t) != null)
            {
                if (t == typeof(OperationBaseItem))
                {
                    OperationBaseItem zoom = child.GetComponent(t) as OperationBaseItem;
                    zoom._Tran = _target.transform;
                    if (child.GetComponent<Collider>() == null)
                    {
                        child.gameObject.AddComponent<MeshCollider>();
                    }
                }
                FindFunc(child, typeof(Collider));
            }
            //该对像不存在T组件，添加T组件，遍历其子节点 添加T组件
            else
            {
                if (t == typeof(Collider))
                {
                    child.gameObject.AddComponent<MeshCollider>();
                    FindFunc(child, t);
                }
                else if (t == typeof(OperationBaseItem))
                {
                    OperationBaseItem zoom = child.gameObject.AddComponent(t) as OperationBaseItem;
                    zoom._Tran = _target.transform;
                    if (child.GetComponent<Collider>() == null)
                    {
                        child.gameObject.AddComponent<MeshCollider>();
                    }
                    FindFunc(child, t);
                }
            }
        }
    }
}
