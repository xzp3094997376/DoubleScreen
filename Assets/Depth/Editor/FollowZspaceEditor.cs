//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(FollowZspace))]
//public class FollowZspaceEditor : Editor
//{
//    private FollowZspace m_fz;
//    //private bool UseSimulateZspace;
//    private void OnEnable()
//    {
//        m_fz = this.target as FollowZspace;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        this.serializedObject.Update();

//        EditorGUILayout.Space();

//        //UseSimulateZspace=EditorGUILayout.PropertyField(this.serializedObject.FindProperty("UseSimulateZspace"), new GUIContent("模拟zspace视角"), true);
//        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("zCamera"), new GUIContent("zspace相机"), true);
//        //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("pCamera"), new GUIContent("pc相机"), true);

//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_listCamera"), new GUIContent("深度与RGB相机"), true);

//        EditorGUILayout.Space();

//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_depth"), new GUIContent("深度参数修改"));

//        if (this.serializedObject.FindProperty("m_depth").objectReferenceValue)
//            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_followDepthScale"), new GUIContent("指定深度缩放"));

//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_widthOffset"), new GUIContent("相机视锥宽度偏移"));

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();
//        EditorGUILayout.LabelField("----编辑器专用----");

//        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_zspaceCamera"), new GUIContent("待取样的相机"));

//        if (this.serializedObject.FindProperty("m_zspaceCamera").objectReferenceValue)
//        {
//            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_farClip"), new GUIContent("相机剪裁远端距离"));

//            EditorGUILayout.Space();
//            EditorGUILayout.BeginHorizontal();

//            if (GUILayout.Button("保存PC数据"))
//            {
//                m_fz.SaveCamPrjMatrixCacheData(PlatformType.PC);
//            }

//            if (GUILayout.Button("读取PC数据"))
//            {
//                m_fz.ReadCamPrjMatrixCacheData(PlatformType.PC);
//            }

//            EditorGUILayout.EndHorizontal();
//            EditorGUILayout.BeginHorizontal();

//            if (GUILayout.Button("保存zSpace数据"))
//            {
//                m_fz.SaveCamPrjMatrixCacheData(PlatformType.zSpace);
//            }

//            if (GUILayout.Button("读取zSpace数据"))
//            {
//                m_fz.ReadCamPrjMatrixCacheData(PlatformType.zSpace);
//            }

//            EditorGUILayout.EndHorizontal();
//        }
//        else
//        {
//            EditorGUILayout.Space();

//            if (GUILayout.Button("读取PC数据"))
//            {
//                m_fz.ReadCamPrjMatrixCacheData(PlatformType.PC);
//            }

//            if (GUILayout.Button("读取zSpace数据"))
//            {
//                m_fz.ReadCamPrjMatrixCacheData(PlatformType.zSpace);
//            }
//        }

//        this.serializedObject.ApplyModifiedProperties();
//    }
//}
