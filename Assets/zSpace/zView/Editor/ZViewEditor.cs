//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Reflection;

using UnityEditor;
using UnityEngine;


namespace zSpace.zView
{
    [CustomEditor(typeof(ZView))]
    public class ZViewEditor : Editor 
    {
        //////////////////////////////////////////////////////////////////
        // Serialized Properties
        //////////////////////////////////////////////////////////////////

        SerializedProperty StandardModeIgnoreLayersProperty;

        SerializedProperty ARModeIgnoreLayersProperty;
        SerializedProperty ARModeEnvironmentLayersProperty;
        SerializedProperty ARModeMaskLayerProperty;
        SerializedProperty ARModeMaskRenderQueueProperty;
        SerializedProperty ARModeMaskSizeProperty;
        SerializedProperty ARModeShowMaskProperty;
        SerializedProperty ARModeEnableTransparencyProperty;


        //////////////////////////////////////////////////////////////////
        // Unity Callbacks
        //////////////////////////////////////////////////////////////////

        void OnEnable()
        {
            this.LoadIconTextures();
            this.FindSerializedProperties();
        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            this.InitializeGUIStyles();
            this.UpdateLayerNames();

            this.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            this.CheckZViewInitialized();
            this.DrawInfoSection();
            this.DrawStandardModeSection();
            this.DrawARModeSection();
            this.DrawConnectionsSection();

            this.serializedObject.ApplyModifiedProperties();
        }


        //////////////////////////////////////////////////////////////////
        // Section Draw Helpers
        //////////////////////////////////////////////////////////////////

        private void CheckZViewInitialized()
        {
            ZView zView = (ZView)this.target;

            if (!zView.IsInitialized())
            {
                EditorGUILayout.HelpBox(
                    "Failed to properly initialize the zSpace zView SDK. As a result, most " +
                        "zView functionality will be disabled. Please make sure that the zView " +
                        "SDK runtime has been properly installed on your machine.",
                    MessageType.Error);
                EditorGUILayout.Space();
            }
        }

        private void DrawInfoSection()
        {
            ZView zView = (ZView)this.target;

            _isInfoSectionExpanded = this.DrawSectionHeader("General Info", _infoIconTexture, _isInfoSectionExpanded);
            if (_isInfoSectionExpanded)
            {
                string pluginVersion = zView.GetPluginVersion();
                string runtimeVersion = zView.IsInitialized() ? zView.GetRuntimeVersion() : "Unknown";

                EditorGUILayout.LabelField("Plugin Version: " + pluginVersion);
                EditorGUILayout.LabelField("Runtime Version: " + runtimeVersion);
                EditorGUILayout.Space();
            }
        }

        private void DrawStandardModeSection()
        {
            _isStandardModeSectionExpanded = this.DrawSectionHeader("Standard Mode", _standardModeIconTexture, _isStandardModeSectionExpanded);
            if (_isStandardModeSectionExpanded)
            {
                this.DrawLayerMask("Ignore Layers", this.StandardModeIgnoreLayersProperty);
                EditorGUILayout.Space();
            }
        }

        private void DrawARModeSection()
        {
            _isARModeSectionExpanded = this.DrawSectionHeader("Augmented Reality Mode", _arModeIconTexture, _isARModeSectionExpanded);
            if (_isARModeSectionExpanded)
            {
                this.DrawLayerMask("Ignore Layers", this.ARModeIgnoreLayersProperty);
                this.DrawLayerMask("Environment Layers", this.ARModeEnvironmentLayersProperty);
                EditorGUILayout.Space();

                this.DrawLayerField("Mask Layer", this.ARModeMaskLayerProperty);
                this.DrawIntField("Mask Render Queue", this.ARModeMaskRenderQueueProperty);
                this.DrawVector3Field("Mask Size", this.ARModeMaskSizeProperty);
                EditorGUILayout.Space();

                this.DrawToggleLeft("Show Mask", this.ARModeShowMaskProperty);
                this.DrawToggleLeft("Enable Transparency", this.ARModeEnableTransparencyProperty);

                if (this.ARModeEnableTransparencyProperty.boolValue)
                {
                    EditorGUILayout.HelpBox(
                        "Enabling transparency may produce visual artifacts when rendering the " +
                            "augmented reality overlay. If enabled, please ensure that all materials " +
                            "associated with opaque geometry have their alpha channel set to 1.0 and " +
                            "that all shaders associated with transparent geometry properly write their " +
                            "alpha channel to the frame buffer.",
                        MessageType.Warning);
                }
                EditorGUILayout.Space();
            }
        }

        private void DrawConnectionsSection()
        {
            ZView zView = (ZView)this.target;

            _isConnectionsSectionExpanded = this.DrawSectionHeader("Connections", _connectionsIconTexture, _isConnectionsSectionExpanded, true);
            if (_isConnectionsSectionExpanded)
            {
                bool doActiveConnectionsExist = false;

                try
                {
                    // For each connection:
                    int numConnections = zView.GetNumConnections();
                    for (int i = 0; i < numConnections; ++i)
                    {
                        // Get the connection.
                        IntPtr connection = zView.GetConnection(i);

                        // Get the connection's state.
                        ZView.ConnectionState connectionState = zView.GetConnectionState(connection);
                        if (connectionState != ZView.ConnectionState.Error)
                        {
                            // Display the connection's information.
                            int connectionNumber = i + 1;
                            string initiationStatusString = zView.WasConnectionLocallyInitiated(connection) ? "locally" : "remotely";
                            EditorGUILayout.LabelField(string.Format("Connection {0}  (initiated {1})", connectionNumber, initiationStatusString));

                            EditorGUI.indentLevel++;
                            {
                                EditorGUILayout.LabelField("Name: " + zView.GetConnectedNodeName(connection));
                                EditorGUILayout.LabelField("Status: " + zView.GetConnectedNodeStatus(connection));
                                EditorGUILayout.LabelField("State: " + connectionState);

                                IntPtr mode = zView.GetConnectionMode(connection);
                                string modeString = "Unknown";
                                if (mode == IntPtr.Zero)
                                {
                                    modeString = "None";
                                }
                                else if (mode == zView.GetStandardMode())
                                {
                                    modeString = "Standard";
                                }
                                else if (mode == zView.GetAugmentedRealityMode())
                                {
                                    modeString = "Augmented Reality";
                                }

                                EditorGUILayout.LabelField("Mode: " + modeString);
                            }
                            EditorGUI.indentLevel--;
                            EditorGUILayout.Space();

                            doActiveConnectionsExist = true;
                        }
                    }
                }
                catch
                {
                    // TODO: Add warning.
                }

                if (!doActiveConnectionsExist)
                {
                    EditorGUILayout.LabelField("No active connections");
                    EditorGUILayout.Space();
                }
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Helper Methods
        //////////////////////////////////////////////////////////////////

        private void LoadIconTextures()
        {
            if (_refreshIconTexture == null)
            {
                _refreshIconTexture = this.LoadIconTexture("RefreshIcon.png");
            }

            if (_infoIconTexture == null)
            {
                _infoIconTexture = this.LoadIconTexture("InfoIcon.png");
            }

            if (_standardModeIconTexture == null)
            {
                _standardModeIconTexture = this.LoadIconTexture("StandardModeIcon.png");
            }
            
            if (_arModeIconTexture == null)
            {
                _arModeIconTexture = this.LoadIconTexture("ARModeIcon.png");
            }

            if (_connectionsIconTexture == null)
            {
                _connectionsIconTexture = this.LoadIconTexture("ConnectionsIcon.png");
            }
        }

        private Texture2D LoadIconTexture(string iconName)
        {
            return AssetDatabase.LoadAssetAtPath(INSPECTOR_ICON_PATH + iconName, typeof(Texture2D)) as Texture2D;
        }

        private void FindSerializedProperties()
        {
            this.StandardModeIgnoreLayersProperty = this.serializedObject.FindProperty("StandardModeIgnoreLayers");

            this.ARModeIgnoreLayersProperty = this.serializedObject.FindProperty("ARModeIgnoreLayers");
            this.ARModeEnvironmentLayersProperty = this.serializedObject.FindProperty("ARModeEnvironmentLayers");
            this.ARModeMaskLayerProperty = this.serializedObject.FindProperty("ARModeMaskLayer");
            this.ARModeMaskRenderQueueProperty = this.serializedObject.FindProperty("ARModeMaskRenderQueue");
            this.ARModeMaskSizeProperty = this.serializedObject.FindProperty("ARModeMaskSize");
            this.ARModeShowMaskProperty = this.serializedObject.FindProperty("ARModeShowMask");
            this.ARModeEnableTransparencyProperty = this.serializedObject.FindProperty("ARModeEnableTransparency");
        }

        private void InitializeGUIStyles()
        {
            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout);
                _foldoutStyle.fontStyle = FontStyle.Bold;
                _foldoutStyle.fixedWidth = 2000.0f;
            }

            if (_lineStyle == null)
            {
                _lineStyle = new GUIStyle(GUI.skin.box);
                _lineStyle.border.top     = 1;
                _lineStyle.border.bottom  = 1;
                _lineStyle.margin.top     = 1;
                _lineStyle.margin.bottom  = 1;
                _lineStyle.padding.top    = 1;
                _lineStyle.padding.bottom = 1;
            }
        }

        private void UpdateLayerNames()
        {
            for (int i = 0; i < NUM_LAYERS; ++i)
            {
                _layerNames[i] = LayerMask.LayerToName(i);
                if (_layerNames[i] == string.Empty)
                {
                    string layerType = (i < 8) ? "Builtin" : "User";
                    _layerNames[i] = string.Format("{0} Layer {1}", layerType, i);
                }
            }
        }

        private bool DrawSectionHeader(string name, Texture2D icon, bool isExpanded)
        {
            return this.DrawSectionHeader(name, icon, isExpanded, false);
        }

        private bool DrawSectionHeader(string name, Texture2D icon, bool isExpanded, bool enableRefresh)
        {
            // Create the divider line.
            GUILayout.Box(GUIContent.none, _lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(1.0f));

            EditorGUILayout.BeginHorizontal();
            {
                // Create the foldout (AKA expandable section).
                Rect position = GUILayoutUtility.GetRect(40.0f, 2000.0f, 16.0f, 16.0f, _foldoutStyle);
                isExpanded = EditorGUI.Foldout(position, isExpanded, new GUIContent(" " + name, icon), true, _foldoutStyle);

                // Create the refresh button if enabled.
                if (enableRefresh && isExpanded)
                {
                    GUILayout.Button(
                        new GUIContent(_refreshIconTexture, "Refresh " + name),
                        GUIStyle.none,
                        GUILayout.Width(15.0f),
                        GUILayout.Height(15.5f));
                } 
            }
            EditorGUILayout.EndHorizontal();

            return isExpanded;
        }

        private void DrawToggleRight(string label, SerializedProperty property)
        {
            property.boolValue = EditorGUILayout.Toggle(new GUIContent(label), property.boolValue);
        }

        private void DrawToggleLeft(string label, SerializedProperty property)
        {
            property.boolValue = EditorGUILayout.ToggleLeft(new GUIContent(" " + label), property.boolValue);
        }

        private void DrawLayerMask(string label, SerializedProperty property)
        {
            property.intValue = EditorGUILayout.MaskField(label, property.intValue, _layerNames);
        }

        private void DrawLayerField(string label, SerializedProperty property)
        {
            property.intValue = EditorGUILayout.LayerField(label, property.intValue);
        }

        private void DrawIntField(string label, SerializedProperty property)
        {
            property.intValue = EditorGUILayout.IntField(label, property.intValue);
        }

        private void DrawVector3Field(string label, SerializedProperty property)
        {
            property.vector3Value = EditorGUILayout.Vector3Field(label, property.vector3Value);
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly string INSPECTOR_ICON_PATH = "Assets/zSpace/zView/Editor/Icons/";
        private static readonly int NUM_LAYERS = 32;

        private Texture2D _refreshIconTexture      = null;
        private Texture2D _infoIconTexture         = null;
        private Texture2D _standardModeIconTexture = null;
        private Texture2D _arModeIconTexture       = null;
        private Texture2D _connectionsIconTexture  = null;

        private GUIStyle _foldoutStyle = null;
        private GUIStyle _lineStyle    = null;

        private bool _isInfoSectionExpanded         = true;
        private bool _isStandardModeSectionExpanded = true;
        private bool _isARModeSectionExpanded       = true;
        private bool _isConnectionsSectionExpanded  = true;

        private string[] _layerNames = new string[NUM_LAYERS];
    }
}

