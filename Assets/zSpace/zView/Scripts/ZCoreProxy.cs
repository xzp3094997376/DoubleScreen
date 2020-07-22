//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

using UnityEngine;


namespace zSpace.zView
{
    /// <summary>
    /// ZCoreProxy is a light-weight wrapper around the zSpace Core API.
    /// It exposes a subset of the zSpace Core API required to properly
    /// implement both zView standard and augmented reality modes.
    /// 
    /// The main purpose of this proxy layer is through reflection, to allow 
    /// for the zView Unity plugin and its associated scripts to be packaged 
    /// independently from the zSpace Core Unity plugin. In the scenario of 
    /// implementing custom virtual cameras for either standard or augmented 
    /// reality modes, it is highly recommended to reference zSpace.Core.ZCore
    /// directly (if it already exists within the project) in lieu of
    /// zSpace.zView.ZCoreProxy.
    /// </summary>
    public class ZCoreProxy
    {
        //////////////////////////////////////////////////////////////////
        // Enumerations
        //////////////////////////////////////////////////////////////////

        public enum CoordinateSpace
        {
            Tracker  = 0,
            Display  = 1,
            Viewport = 2,
            Camera   = 3,
            World    = 4,
        }

        public enum Eye
        {
            Left   = 0,
            Right  = 1,
            Center = 2,
        }


        //////////////////////////////////////////////////////////////////
        // Public API
        //////////////////////////////////////////////////////////////////

        public static ZCoreProxy Instance
        {
            get
            {
                // If the ZCoreProxy instance has not been created, create it.
                if (_instance == null)
                {
                    _instance = new ZCoreProxy();
                }

                // Attempt to find the first instance of the zSpace.Core.ZCore
                // Monobehaviour script if it hasn't already been found.
                if (_instance._core == null)
                {
                    if (_instance._coreType != null)
                    {
                        _instance._core = GameObject.FindObjectOfType(_instance._coreType) as Component;
                    }
                }

                return _instance;
            }
        }

        public static void DestroyInstance()
        {
            _instance = null;
        }

        public GameObject CoreObject
        {
            get
            {
                if (_core == null)
                {
                    return null;
                }

                return _core.gameObject;
            }
        }

        public GameObject CurrentCameraObject
        {
            get
            {
                if (_core == null || _currentCameraObject == null)
                {
                    return null;
                }

                return (GameObject)_currentCameraObject.GetValue(_core);
            }
        }

        public float ViewerScale
        {
            get
            {
                if (_core == null || _viewerScale == null)
                {
                    return 1.0f;
                }

                return (float)_viewerScale.GetValue(_core);
            }
        }

        public Vector2 GetDisplaySize()
        {
            if (_core == null || _getDisplaySize == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getDisplaySize.Invoke(_core, null);
        }

        public Vector2 GetDisplayPosition()
        {
            if (_core == null || _getDisplayPosition == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getDisplayPosition.Invoke(_core, null);
        }

        public Vector2 GetDisplayNativeResolution()
        {
            if (_core == null || _getDisplayNativeResolution == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getDisplayNativeResolution.Invoke(_core, null);
        }

        public Vector3 GetDisplayAngle()
        {
            if (_core == null || _getDisplayAngle == null)
            {
                return Vector2.zero;
            }

            return (Vector3)_getDisplayAngle.Invoke(_core, null);
        }

        public Vector2 GetViewportPosition()
        {
            if (_core == null || _getViewportPosition == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getViewportPosition.Invoke(_core, null);
        }

        public Vector2 GetViewportSize()
        {
            if (_core == null || _getViewportSize == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getViewportSize.Invoke(_core, null);
        }

        public Vector3 GetViewportWorldCenter()
        {
            if (_core == null || _getViewportWorldCenter == null)
            {
                return Vector3.zero;
            }

            return (Vector3)_getViewportWorldCenter.Invoke(_core, null);
        }

        public Quaternion GetViewportWorldRotation()
        {
            if (_core == null || _getViewportWorldRotation == null)
            {
                return Quaternion.identity;
            }

            return (Quaternion)_getViewportWorldRotation.Invoke(_core, null);
        }

        public Vector2 GetViewportWorldSize()
        {
            if (_core == null || _getViewportWorldSize == null)
            {
                return Vector2.zero;
            }

            return (Vector2)_getViewportWorldSize.Invoke(_core, null);
        }

        public Matrix4x4 GetCoordinateSpaceTransform(CoordinateSpace a, CoordinateSpace b)
        {
            if (_core == null || _getCoordinateSpaceTransform == null)
            {
                return Matrix4x4.identity;
            }

            return (Matrix4x4)_getCoordinateSpaceTransform.Invoke(
                _core,
                new object[]
                {
                    Enum.ToObject(_coordinateSpaceType, (int)a),
                    Enum.ToObject(_coordinateSpaceType, (int)b)
                });
        }

        public Matrix4x4 GetFrustumViewMatrix(Eye eye)
        {
            if (_core == null || _getFrustumViewMatrix == null)
            {
                return Matrix4x4.identity;
            }

            return (Matrix4x4)_getFrustumViewMatrix.Invoke(
                _core,
                new object[]
                {
                    Enum.ToObject(_eyeType, (int)eye)
                });
        }

        public Matrix4x4 GetFrustumProjectionMatrix(Eye eye)
        {
            if (_core == null || _getFrustumProjectionMatrix == null)
            {
                return Matrix4x4.identity;
            }

            return (Matrix4x4)_getFrustumProjectionMatrix.Invoke(
                _core,
                new object[]
                {
                    Enum.ToObject(_eyeType, (int)eye)
                });
        }


        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private ZCoreProxy()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                _coreType = assembly.GetType("zSpace.Core.ZCore");
                if (_coreType != null)
                {
                    ///////////////////////////////
                    // Types
                    ///////////////////////////////

                    // Get the Eye nested type.
                    _eyeType = _coreType.GetNestedType("Eye");
                    if (_eyeType == null)
                    {
                        Debug.LogError("Failed to find zSpace.Core.ZCore.Eye type.");
                    }

                    // Get the CoordinateSpace nested type.
                    _coordinateSpaceType = _coreType.GetNestedType("CoordinateSpace");
                    if (_coordinateSpaceType == null)
                    {
                        Debug.LogError("Failed to find zSpace.Core.ZCore.CoordinateSpace type.");
                    }


                    ///////////////////////////////
                    // Fields
                    ///////////////////////////////

                    // Get the CurrentCameraObject field info.
                    _currentCameraObject = _coreType.GetField(
                        "CurrentCameraObject",
                        BindingFlags.Public | BindingFlags.Instance);

                    if (_currentCameraObject == null)
                    {
                        Debug.LogError("Failed to find zSpace.Core.ZCore.CurrentCameraObject field.");
                    }

                    // Get the ViewerScale field info.
                    _viewerScale = _coreType.GetField(
                        "ViewerScale",
                        BindingFlags.Public | BindingFlags.Instance);

                    if (_viewerScale == null)
                    {
                        Debug.LogError("Failed to find zSpace.Core.ZCore.ViewerScale field.");
                    }


                    ///////////////////////////////
                    // Methods
                    ///////////////////////////////
                    
                    // Get the GetDisplaySize method info.
                    _getDisplaySize = _coreType.GetMethod(
                        "GetDisplaySize",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getDisplaySize == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetDisplaySize method.");
                    }

                    // Get the GetDisplayPosition method info.
                    _getDisplayPosition = _coreType.GetMethod(
                        "GetDisplayPosition",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getDisplayPosition == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetDisplayPosition method.");
                    }

                    // Get the GetDisplayNativeResolution method info.
                    _getDisplayNativeResolution = _coreType.GetMethod(
                        "GetDisplayNativeResolution",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getDisplayNativeResolution == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetDisplayNativeResolution method.");
                    }

                    // Get the GetDisplayAngle method info.
                    _getDisplayAngle = _coreType.GetMethod(
                        "GetDisplayAngle",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getDisplayAngle == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetDisplayAngle method.");
                    }

                    // Get the GetViewportPosition method info.
                    _getViewportPosition = _coreType.GetMethod(
                        "GetViewportPosition",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getViewportPosition == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetViewportPosition method.");
                    }

                    // Get the GetViewportSize method info.
                    _getViewportSize = _coreType.GetMethod(
                        "GetViewportSize",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getViewportSize == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetViewportSize method.");
                    }

                    // Get the GetViewportWorldCenter method info.
                    _getViewportWorldCenter = _coreType.GetMethod(
                        "GetViewportWorldCenter",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getViewportWorldCenter == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetViewportWorldCenter method.");
                    }

                    // Get the GetViewportWorldRotation method info.
                    _getViewportWorldRotation = _coreType.GetMethod(
                        "GetViewportWorldRotation",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getViewportWorldRotation == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetViewportWorldRotation method.");
                    }

                    // Get the GetViewportWorldSize method info.
                    _getViewportWorldSize = _coreType.GetMethod(
                        "GetViewportWorldSize",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { },
                        null);

                    if (_getViewportWorldSize == null)
                    {
                        Debug.LogError("Failed to find the zSpace.Core.ZCore.GetViewportWorldSize method.");
                    }

                    // Get the GetCoordinateSpaceTransform method info.
                    if (_coordinateSpaceType != null)
                    {
                        _getCoordinateSpaceTransform = _coreType.GetMethod(
                            "GetCoordinateSpaceTransform",
                            BindingFlags.Public | BindingFlags.Instance,
                            null,
                            new Type[] { _coordinateSpaceType, _coordinateSpaceType },
                            null);

                        if (_getCoordinateSpaceTransform == null)
                        {
                            Debug.LogError("Failed to find the zSpace.Core.ZCore.GetCoordinateSpaceTransform method.");
                        }
                    }

                    // Get the GetFrustumViewMatrix method info.
                    if (_eyeType != null)
                    {
                        _getFrustumViewMatrix = _coreType.GetMethod(
                            "GetFrustumViewMatrix",
                            BindingFlags.Public | BindingFlags.Instance,
                            null,
                            new Type[] { _eyeType },
                            null);

                        if (_getFrustumViewMatrix == null)
                        {
                            Debug.LogError("Failed to find the zSpace.Core.ZCore.GetFrustumViewMatrix method.");
                        }
                    }

                    // Get the GetFrustumProjectionMatrix method info.
                    if (_eyeType != null)
                    {
                        _getFrustumProjectionMatrix = _coreType.GetMethod(
                            "GetFrustumProjectionMatrix",
                            BindingFlags.Public | BindingFlags.Instance,
                            null,
                            new Type[] { _eyeType },
                            null);

                        if (_getFrustumProjectionMatrix == null)
                        {
                            Debug.LogError("Failed to find the zSpace.Core.ZCore.GetFrustumProjectionMatrix method.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to find zSpace.Core.ZCore type in executing assembly.");
                }
            }
            else
            {
                Debug.LogError("Failed to get executing assembly for ZCoreProxy.");
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static ZCoreProxy _instance = null;

        private Component _core = null;

        private Type _coreType             = null;
        private Type _coordinateSpaceType  = null;
        private Type _eyeType              = null;

        private FieldInfo _currentCameraObject = null;
        private FieldInfo _viewerScale         = null;

        private MethodInfo _getDisplaySize              = null;
        private MethodInfo _getDisplayPosition          = null;
        private MethodInfo _getDisplayNativeResolution  = null;
        private MethodInfo _getDisplayAngle             = null;
        private MethodInfo _getViewportPosition         = null;
        private MethodInfo _getViewportSize             = null;
        private MethodInfo _getViewportWorldCenter      = null;
        private MethodInfo _getViewportWorldRotation    = null;
        private MethodInfo _getViewportWorldSize        = null;
        private MethodInfo _getCoordinateSpaceTransform = null;
        private MethodInfo _getFrustumViewMatrix        = null;
        private MethodInfo _getFrustumProjectionMatrix  = null;
    }
}