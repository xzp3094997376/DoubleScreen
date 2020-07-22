//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using UnityEngine;


namespace zSpace.zView
{
    public partial class ZView : MonoBehaviour
    {
        private class GlobalState
        {
            /// <summary>
            /// 
            /// </summary>
            public static GlobalState Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new GlobalState();
                    }

                    return _instance;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static void DestroyInstance()
            {
                if (_instance != null)
                {
                    _instance.ShutDown();
                }

                _instance = null;
            }

            /// <summary>
            /// Returns a reference to the zView SDK's context.
            /// </summary>
            public IntPtr Context
            {
                get
                {
                    return _context;
                }
            }

            /// <summary>
            /// Returns a reference to the standard mode handle.
            /// </summary>
            public IntPtr ModeStandard
            {
                get
                {
                    return _modeStandard;
                }
            }

            /// <summary>
            /// Returns a reference to the augmented reality mode handle.
            /// </summary>
            public IntPtr ModeAugmentedReality
            {
                get
                {
                    return _modeAugmentedReality;
                }
            }

            /// <summary>
            /// Return a reference to the current active connection.
            /// </summary>
            public IntPtr Connection
            {
                get
                {
                    return _connection;
                }
                set
                {
                    _connection = value;
                }
            }

            /// <summary>
            /// Returns whether the zSpace zView SDK was properly initialized.
            /// </summary>
            public bool IsInitialized
            {
                get
                {
                    return _isInitialized;
                }
            }


            private GlobalState()
            {
                // Initialize the zView context.
                PluginError error = zvuInitialize(NodeType.Presenter, out _context);
                if (error == PluginError.Ok)
                {
                    // Set the context's node name.
                    error = zvuSetNodeName(_context, ZView.StringToNativeUtf8(this.GetProjectName()));
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to set node name: ({0})", error));
                    }

                    // Set the context's node status.
                    error = zvuSetNodeStatus(_context, ZView.StringToNativeUtf8(string.Empty));
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to set node status: ({0})", error));
                    }

                    // Get both standard and augmented reality modes.
                    List<ZVSupportedMode> supportedModes = new List<ZVSupportedMode>();

                    _modeStandard = this.GetMode(_context, CompositingMode.None, CameraMode.LocalHeadTracked);
                    if (_modeStandard != IntPtr.Zero)
                    {
                        supportedModes.Add(
                            new ZVSupportedMode
                            {
                                mode = _modeStandard,
                                modeAvailability = ModeAvailability.Available
                            });
                    }

                    _modeAugmentedReality = this.GetMode(_context, CompositingMode.AugmentedRealityCamera, CameraMode.RemoteMovable);
                    if (_modeAugmentedReality != IntPtr.Zero)
                    {
                        supportedModes.Add(
                            new ZVSupportedMode
                            {
                                mode = _modeAugmentedReality,
                                modeAvailability = ModeAvailability.Available
                            });
                    }

                    // Set the context's supported modes.
                    error = zvuSetSupportedModes(_context, supportedModes.ToArray(), supportedModes.Count);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to set supported modes: ({0})", error));
                    }

                    // Set the context's supported capabilities.
                    error = zvuSetSupportedCapabilities(_context, null, 0);
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to set supported capabilities: ({0})", error));
                    }

                    // Start listening for new connections.
                    error = zvuStartListeningForConnections(_context, ZView.StringToNativeUtf8(string.Empty));
                    if (error != PluginError.Ok)
                    {
                        Debug.LogError(string.Format("Failed to start listening for connections: ({0})", error));
                    }

                    _isInitialized = true;
                }
                else
                {
                    Debug.LogWarning(string.Format("Failed to initialize zView context: ({0})", error));
                    _isInitialized = false;
                }
            }

            ~GlobalState()
            {
                ShutDown();
            }

            private void ShutDown()
            {
                if (_isInitialized)
                {
                    PluginError error = zvuShutDown(_context);
                    if (error != PluginError.Ok)
                    {
                        // Shut down the zView context.
                        Debug.LogWarning(string.Format("Failed to shut down zView context: ({0})", error));
                    }

                    // Clear out handles.
                    _context = IntPtr.Zero;
                    _modeStandard = IntPtr.Zero;
                    _modeAugmentedReality = IntPtr.Zero;
                    _connection = IntPtr.Zero;

                    _isInitialized = false;
                }
            }

            private IntPtr GetMode(IntPtr context, CompositingMode compositingMode, CameraMode cameraMode)
            {
                PluginError error = PluginError.Unknown;
                IntPtr modeSpec = IntPtr.Zero;

                // Create the mode spec.
                error = zvuCreateModeSpec(context, out modeSpec);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to create mode spec: ({0})", error));
                    return IntPtr.Zero;
                }

                // Specify the mode spec's attributes.
                error = zvuSetModeSpecAttributeU32(modeSpec, ModeAttributeKey.Version, 0);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to set version attribute: ({0})", error));
                }

                error = zvuSetModeSpecAttributeU32(modeSpec, ModeAttributeKey.CompositingMode, (UInt32)compositingMode);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to set compositing mode attribute: ({0})", error));
                }

                error = zvuSetModeSpecAttributeU32(modeSpec, ModeAttributeKey.PresenterCameraMode, (UInt32)cameraMode);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to set presenter camera mode attribute: ({0})", error));
                }

                error = zvuSetModeSpecAttributeU32(modeSpec, ModeAttributeKey.ImageRowOrder, (UInt32)ImageRowOrder.BottomToTop);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to set image row order attribute: ({0})", error));
                }

                error = zvuSetModeSpecAttributeU32(modeSpec, ModeAttributeKey.ColorImagePixelFormat, (UInt32)PixelFormat.R8G8B8A8);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to set color image pixel format attribute: ({0})", error));
                }

                // Get the mode for the specified spec.
                IntPtr mode = IntPtr.Zero;
                error = zvuGetModeForSpec(modeSpec, out mode);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to get mode for mode spec: ({0})", error));
                }

                // Destroy the mode spec since it's no longer being used.
                error = zvuDestroyModeSpec(modeSpec);
                if (error != PluginError.Ok)
                {
                    Debug.LogError(string.Format("Failed to destroy mode spec: ({0})", error));
                }

                return mode;
            }

            private string GetProjectName()
            {
                string projectName = string.Empty;

                string[] s = Application.dataPath.Split('/');
                if (s.Length > 1)
                {
                    projectName = s[s.Length - 2];
                }

                return projectName;
            }


            //////////////////////////////////////////////////////////////////
            // Private Members
            //////////////////////////////////////////////////////////////////

            private static GlobalState _instance;

            private IntPtr _context              = IntPtr.Zero;
            private IntPtr _modeStandard         = IntPtr.Zero;
            private IntPtr _modeAugmentedReality = IntPtr.Zero;
            private IntPtr _connection           = IntPtr.Zero;
            private bool   _isInitialized        = false;
        }
    }
}

