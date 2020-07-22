using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomAlwaysIncludedShaders
{
    [MenuItem("ExtendCamera/自定义AlwaysIncludedShaders")]
    public static void SetIncludedShader()
    {
        string[] includeShaders = new string[]{
         "ExtendDisplay/SideBySideStereo"
        };

        SerializedObject graphicsSettings = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]
            );
        SerializedProperty it = graphicsSettings.GetIterator();
        SerializedProperty dataPoint;
        while (it.NextVisible(true))
        {
            if (it.name == "m_AlwaysIncludedShaders")
            {
                for (int i = 0; i < includeShaders.Length; i++)
                {
                    bool canAdd = true;
                    for (int j = 0; j < it.arraySize; j++)
                    {
                        if (it.GetArrayElementAtIndex(j).objectReferenceValue.name == includeShaders[i])
                        {
                            canAdd = false;
                        }
                    }
                    if(!canAdd)
                        continue;
                    it.InsertArrayElementAtIndex(it.arraySize-1);
                    dataPoint = it.GetArrayElementAtIndex(it.arraySize-1);
                    dataPoint.objectReferenceValue = Shader.Find(includeShaders[i]);
                }
                graphicsSettings.ApplyModifiedProperties();
                graphicsSettings.UpdateIfRequiredOrScript();
            }
        }
    }
}
