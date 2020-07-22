//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using UnityEngine;


namespace zSpace.zView
{
    public partial class ZView : MonoBehaviour
    {
        //////////////////////////////////////////////////////////////////
        // Enumerations
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Defines the error codes returned by all native plugin zView 
        /// API functions.
        /// </summary>
        public enum PluginError
        {
            Unknown = -1,

            /// <summary>
            /// No error occurred.
            /// </summary>
            Ok = 0,

            /// <summary>
            /// An error of an unspecified type occurred.
            /// </summary>
            Failed = 1,

            /// <summary>
            /// A zView API function that is not implemented was called.
            /// </summary>
            /// 
            /// <remarks>
            /// This may occur when running code written against a version of the zView
            /// API that is newer than the version of the zView runtime being used.
            /// </remarks>
            NotImplemented = 2,

            /// <summary>
            /// A zView API function was called before the zView runtime has
            /// been initialized.
            /// </summary>
            NotInitialized = 3,

            /// <summary>
            /// A view API function was called with an invalid parameter value.
            /// </summary>
            InvalidParameter = 4,

            /// <summary>
            /// The zView runtime failed to allocate additional memory while
            /// performing some operation.
            /// </summary>
            OutOfMemory = 5,

            /// <summary>
            /// A view API function was called with a buffer that is too small.
            /// </summary>
            BufferTooSmall = 6,

            /// <summary>
            /// The zview runtime DLL could not be found.
            /// </summary>
            RuntimeNotFound = 7,

            /// <summary>
            /// A required symbol within the zView runtime DLL could not be
            /// found.
            /// </summary>
            SymbolNotFound = 8,

            /// <summary>
            /// A zView API function was called in a way that is not compatible
            /// with the version of the zView runtime being used.
            /// </summary>
            RuntimeIncompatible = 9,

            /// <summary>
            /// A zView API function was called in a way that is not allowed for
            /// the current or specified node type.
            /// </summary>
            InvalidNodeType = 10,

            /// <summary>
            /// A zView API function was called with an invalid context.
            /// </summary>
            InvalidContext = 11,

            /// <summary>
            /// A zView API function was called in a way that is not allowed
            /// while the associated zView context is in its current state.
            /// </summary>
            InvalidContextState = 12,

            /// <summary>
            /// A zView API function was called with an invalid mode spec
            /// handle.
            /// </summary>
            InvalidModeSpec = 13,

            /// <summary>
            /// A zView API function was called with an invalid mode handle.
            /// </summary>
            InvalidMode = 14,

            /// <summary>
            /// A zView API function was called with an invalid
            /// ZVModeAttributeKey enum value.
            /// </summary>
            InvalidModeAttributeKey = 15,

            /// <summary>
            /// An operation failed because an invalid connection specification
            /// was specified.
            /// </summary>
            InvalidConnectionSpec = 16,

            /// <summary>
            /// A zView API function was called with an invalid ZVConnection
            /// handle.
            /// </summary>
            InvalidConnection = 17,

            /// <summary>
            /// A zView API function was called in a way that is not allowed
            /// while the associated zView connection is in its current state.
            /// </summary>
            InvalidConnectionState = 18,

            /// <summary>
            /// A zView API function was called with an invalid setting key
            /// enum value.
            /// </summary>
            InvalidSettingKey = 19,

            /// <summary>
            /// A zView API function was called with an invalid Stream enum
            /// value.
            /// </summary>
            InvalidStream = 20,

            /// <summary>
            /// A zView API function was called with an invalid frame
            /// handle.
            /// </summary>
            InvalidFrame = 21,

            /// <summary>
            /// A zView API function was called with an invalid FrameDataKey
            /// enum value.
            /// </summary>
            InvalidFrameDataKey = 22,

            /// <summary>
            /// A zView API function was called in a way that is not allowed
            /// while the associated zView connection is in its current video recording
            /// state.
            /// </summary>
            InvalidVideoRecordingState = 23,

            /// <summary>
            /// An operation failed because the associated zView context has
            /// already been shut down.
            /// </summary>
            Shutdown = 24,

            /// <summary>
            /// A zView API function was called with a mode or mode spec
            /// handle representing a zView mode that is not supported.
            /// </summary>
            UnsupportedMode = 25,

            /// <summary>
            /// A zView API function was called in a way that requires a
            /// capability that is not supported.
            /// </summary>
            UnsupportedCapability = 26,

            /// <summary>
            /// An operation failed because a low-level network I/O error
            /// occurred.
            /// </summary>
            Network = 27,

            /// <summary>
            /// An operation failed because a zView communication protocol error
            /// occurred.
            /// </summary>
            Protocol = 28,

            /// <summary>
            /// A zView connection could not be established because a
            /// communication protocol version supported by all nodes does not exist.
            /// </summary>
            NoSupportedProtocolVersion = 29,
        }
        
        public enum PluginEvent
        {
            SendFrame = 20000,
            DestroyResources = 20001,
        }


        //////////////////////////////////////////////////////////////////
        // Public API
        //////////////////////////////////////////////////////////////////

        public static void IssuePluginEvent(PluginEvent pluginEvent)
        {
        #if (UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
            GL.IssuePluginEvent((int)pluginEvent);
        #else
            IntPtr renderEventFunc = zvuGetRenderEventFunc();
            if (renderEventFunc != IntPtr.Zero)
            {
                GL.IssuePluginEvent(renderEventFunc, (int)pluginEvent);
            }
        #endif
        }


        //////////////////////////////////////////////////////////////////
        // Compound Types
        //////////////////////////////////////////////////////////////////

        [StructLayout(LayoutKind.Sequential)]
        private struct ZVSupportedMode
        {
            public IntPtr mode;
            public ModeAvailability modeAvailability;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ZSVector3
        {
            [FieldOffset(0)]
            public float x;

            [FieldOffset(4)]
            public float y;

            [FieldOffset(8)]
            public float z;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ZSMatrix4
        {
            [FieldOffset(0)]
            public float m00;

            [FieldOffset(4)]
            public float m10;

            [FieldOffset(8)]
            public float m20;

            [FieldOffset(12)]
            public float m30;

            [FieldOffset(16)]
            public float m01;

            [FieldOffset(20)]
            public float m11;

            [FieldOffset(24)]
            public float m21;

            [FieldOffset(28)]
            public float m31;

            [FieldOffset(32)]
            public float m02;

            [FieldOffset(36)]
            public float m12;

            [FieldOffset(40)]
            public float m22;

            [FieldOffset(44)]
            public float m32;

            [FieldOffset(48)]
            public float m03;

            [FieldOffset(52)]
            public float m13;

            [FieldOffset(56)]
            public float m23;

            [FieldOffset(60)]
            public float m33;
        }


        //////////////////////////////////////////////////////////////////
        // General API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr zvuGetRenderEventFunc();

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zvuGetPluginVersion(
            out int major,
            out int minor,
            out int patch);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern void zvuSetCurrentFrameInfo(
            IntPtr frame,
            IntPtr texturePtr);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuInitialize(
            NodeType nodeType,
            out IntPtr context);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuShutDown(
            IntPtr context);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetRuntimeVersion(
            IntPtr context,
            out int major,
            out int minor,
            out int patch);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetErrorStringSize(
            PluginError error,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetErrorString(
            PluginError error,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeType(
            IntPtr context,
            out NodeType type);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeIdSize(
            IntPtr context,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeId(
            IntPtr context,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeNameSize(
            IntPtr context,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeName(
            IntPtr context,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetNodeName(
            IntPtr context,
            byte[] name);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeStatusSize(
            IntPtr context,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNodeStatus(
            IntPtr context,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetNodeStatus(
            IntPtr context,
            byte[] status);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNumSupportedModes(
            IntPtr context,
            out int numModes);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSupportedMode(
            IntPtr context,
            int modeIndex,
            out ZVSupportedMode mode);

        // TODO: Double check this.
        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSupportedModes(
            IntPtr context,
            ZVSupportedMode[] modes,
            int numModes);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNumSupportedCapabilities(
            IntPtr context,
            out int numCapabilities);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSupportedCapability(
            IntPtr context,
            int capabilityIndex,
            out Capability capability);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSupportedCapabilities(
            IntPtr context,
            Capability[] capabilities,
            int numCapabilities);


        //////////////////////////////////////////////////////////////////
        // Mode API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuCreateModeSpec(
            IntPtr context,
            out IntPtr modeSpec);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuDestroyModeSpec(
            IntPtr modeSpec);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetModeSpecAttributeU32(
            IntPtr modeSpec,
            ModeAttributeKey key,
            out UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetModeSpecAttributeU32(
            IntPtr modeSpec,
            ModeAttributeKey key,
            UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetModeForSpec(
            IntPtr modeSpec,
            out IntPtr mode);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetModeAttributeU32(
            IntPtr mode,
            ModeAttributeKey key,
            out UInt32 value);


        //////////////////////////////////////////////////////////////////
        // Connection Management API Imports
        //////////////////////////////////////////////////////////////////

        // TODO: Remove listeningSpec parameter
        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuStartListeningForConnections(
            IntPtr context,
            byte[] listeningSpec);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuStopListeningForConnections(
            IntPtr context);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuConnectToDefaultViewer(
            IntPtr context,
            IntPtr connectionUserData);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuCloseConnection(
            IntPtr connection,
            ConnectionCloseAction action,
            ConnectionCloseReason reason,
            byte[] reasonDetails);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuAcceptConnection(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuDestroyConnection(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuUpdateConnectionList(
            IntPtr context);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNumConnections(
            IntPtr context,
            out int numConnections);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnection(
            IntPtr context,
            int connectionIndex,
            out IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuUpdateConnection(
            IntPtr connection);


        //////////////////////////////////////////////////////////////////
        // Connection Property API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionState(
            IntPtr connection,
            out ConnectionState state);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionError(
            IntPtr connection,
            out PluginError error);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuWasConnectionLocallyInitiated(
            IntPtr connection,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool wasLocallyInitiated);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeIdSize(
            IntPtr connection,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeId(
            IntPtr connection,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeNameSize(
            IntPtr connection,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeName(
            IntPtr connection,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeStatusSize(
            IntPtr connection,
            out int size);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectedNodeStatus(
            IntPtr connection,
            byte[] buffer,
            int bufferSize);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuDoesConnectionSupportCapability(
            IntPtr connection,
            Capability capability,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isSupported);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNumConnectionSupportedModes(
            IntPtr connection,
            out int numSupportedModes);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionSupportedMode(
            IntPtr connection,
            int supportedModeIndex,
            out ZVSupportedMode supportedMode);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionMode(
            IntPtr connection,
            out IntPtr mode);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetConnectionMode(
            IntPtr connection,
            IntPtr mode);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionUserData(
            IntPtr connection,
            out IntPtr userData);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetConnectionUserData(
            IntPtr connection,
            IntPtr userData);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionCloseAction(
            IntPtr connection,
            out ConnectionCloseAction action);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionCloseReason(
            IntPtr connection,
            out ConnectionCloseReason reason);


        //////////////////////////////////////////////////////////////////
        // Connection Phase Change API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetConnectionModeSetupPhase(
            IntPtr connection,
            out ModeSetupPhase phase,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool isAwaitingCompletion);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuCompleteModeSetupPhase(
            IntPtr connection,
            ModeSetupPhase phase);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuPauseMode(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuResumeMode(
            IntPtr connection);


        //////////////////////////////////////////////////////////////////
        // Connection Settings API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuBeginSettingsBatch(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuEndSettingsBatch(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingB(
            IntPtr connection,
            SettingKey key,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingI8(
            IntPtr connection,
            SettingKey key,
            out sbyte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingI16(
            IntPtr connection,
            SettingKey key,
            out Int16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingI32(
            IntPtr connection,
            SettingKey key,
            out Int32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingI64(
            IntPtr connection,
            SettingKey key,
            out Int64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingU8(
            IntPtr connection,
            SettingKey key,
            out byte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingU16(
            IntPtr connection,
            SettingKey key,
            out UInt16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingU32(
            IntPtr connection,
            SettingKey key,
            out UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingU64(
            IntPtr connection,
            SettingKey key,
            out UInt64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingF32(
            IntPtr connection,
            SettingKey key,
            out float value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingF64(
            IntPtr connection,
            SettingKey key,
            out double value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingV3(
            IntPtr connection,
            SettingKey key,
            out ZSVector3 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingM4(
            IntPtr connection,
            SettingKey key,
            out ZSMatrix4 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingB(
            IntPtr connection,
            SettingKey key,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingI8(
            IntPtr connection,
            SettingKey key,
            sbyte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingI16(
            IntPtr connection,
            SettingKey key,
            Int16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingI32(
            IntPtr connection,
            SettingKey key,
            Int32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingI64(
            IntPtr connection,
            SettingKey key,
            Int64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingU8(
            IntPtr connection,
            SettingKey key,
            byte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingU16(
            IntPtr connection,
            SettingKey key,
            UInt16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingU32(
            IntPtr connection,
            SettingKey key,
            UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingU64(
            IntPtr connection,
            SettingKey key,
            UInt64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingF32(
            IntPtr connection,
            SettingKey key,
            float value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingF64(
            IntPtr connection,
            SettingKey key,
            double value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingV3(
            IntPtr connection,
            SettingKey key,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZSVector3 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetSettingM4(
            IntPtr connection,
            SettingKey key,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZSMatrix4 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetSettingState(
            IntPtr connection,
            SettingKey key,
            out SettingState state);


        //////////////////////////////////////////////////////////////////
        // Connection Frame Data API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuReceiveFrame(
            IntPtr connection,
            Stream stream,
            out IntPtr frame);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuReleaseReceivedFrame(
            IntPtr frame);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetNextFrameToSend(
            IntPtr connection,
            Stream stream,
            out IntPtr frame);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSendFrame(
            IntPtr frame);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataB(
            IntPtr frame,
            FrameDataKey key,
            [param: MarshalAs(UnmanagedType.Bool), Out()]
            out bool value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataI8(
            IntPtr frame,
            FrameDataKey key,
            out sbyte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataI16(
            IntPtr frame,
            FrameDataKey key,
            out Int16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataI32(
            IntPtr frame,
            FrameDataKey key,
            out Int32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataI64(
            IntPtr frame,
            FrameDataKey key,
            out Int64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataU8(
            IntPtr frame,
            FrameDataKey key,
            out byte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataU16(
            IntPtr frame,
            FrameDataKey key,
            out UInt16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataU32(
            IntPtr frame,
            FrameDataKey key,
            out UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataU64(
            IntPtr frame,
            FrameDataKey key,
            out UInt64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataF32(
            IntPtr frame,
            FrameDataKey key,
            out float value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataF64(
            IntPtr frame,
            FrameDataKey key,
            out double value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataV3(
            IntPtr frame,
            FrameDataKey key,
            out ZSVector3 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameDataM4(
            IntPtr frame,
            FrameDataKey key,
            out ZSMatrix4 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataB(
            IntPtr frame,
            FrameDataKey key,
            [param: MarshalAs(UnmanagedType.Bool)]
            bool value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataI8(
            IntPtr frame,
            FrameDataKey key,
            sbyte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataI16(
            IntPtr frame,
            FrameDataKey key,
            Int16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataI32(
            IntPtr frame,
            FrameDataKey key,
            Int32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataI64(
            IntPtr frame,
            FrameDataKey key,
            Int64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataU8(
            IntPtr frame,
            FrameDataKey key,
            byte value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataU16(
            IntPtr frame,
            FrameDataKey key,
            UInt16 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataU32(
            IntPtr frame,
            FrameDataKey key,
            UInt32 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataU64(
            IntPtr frame,
            FrameDataKey key,
            UInt64 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataF32(
            IntPtr frame,
            FrameDataKey key,
            float value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataF64(
            IntPtr frame,
            FrameDataKey key,
            double value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataV3(
            IntPtr frame,
            FrameDataKey key,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZSVector3 value);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSetFrameDataM4(
            IntPtr frame,
            FrameDataKey key,
            [param: MarshalAs(UnmanagedType.LPStruct)]
            ZSMatrix4 value);

        // TODO: Experiment with marshaling the buffer.
        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetFrameBuffer(
            IntPtr frame,
            FrameBufferKey key,
            out IntPtr buffer); // ZVUInt8**


        //////////////////////////////////////////////////////////////////
        // Video Recording API Imports
        //////////////////////////////////////////////////////////////////

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetVideoRecordingState(
            IntPtr connection,
            out VideoRecordingState state);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetVideoRecordingError(
            IntPtr connection,
            out PluginError error);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuClearVideoRecordingError(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuStartVideoRecording(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuFinishVideoRecording(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuPauseVideoRecording(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuResumeVideoRecording(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuSaveVideoRecording(
            IntPtr connection,
            byte[] fileName);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuDiscardVideoRecording(
            IntPtr connection);

        [DllImport("zViewUnity", CallingConvention = CallingConvention.StdCall)]
        private static extern PluginError zvuGetVideoRecordingTime(
            IntPtr connection,
            out UInt64 timeInMilliseconds);


        //////////////////////////////////////////////////////////////////
        // Private Helpers
        //////////////////////////////////////////////////////////////////

        private PluginException NewPluginException(PluginError error)
        {
            switch (error)
            {
                case PluginError.Failed:
                    return new FailedException();
                case PluginError.NotImplemented:
                    return new NotImplementedException();
                case PluginError.NotInitialized:
                    return new NotInitializedException();
                case PluginError.InvalidParameter:
                    return new InvalidParameterException();
                case PluginError.OutOfMemory:
                    return new OutOfMemoryException();
                case PluginError.BufferTooSmall:
                    return new BufferTooSmallException();
                case PluginError.RuntimeNotFound:
                    return new RuntimeNotFoundException();
                case PluginError.SymbolNotFound:
                    return new SymbolNotFoundException();
                case PluginError.RuntimeIncompatible:
                    return new RuntimeIncompatibleException();
                case PluginError.InvalidNodeType:
                    return new InvalidNodeTypeException();
                case PluginError.InvalidContext:
                    return new InvalidContextException();
                case PluginError.InvalidContextState:
                    return new InvalidContextStateException();
                case PluginError.InvalidModeSpec:
                    return new InvalidModeSpecException();
                case PluginError.InvalidMode:
                    return new InvalidModeException();
                case PluginError.InvalidModeAttributeKey:
                    return new InvalidModeAttributeKeyException();
                case PluginError.InvalidConnectionSpec:
                    return new InvalidConnectionSpecException();
                case PluginError.InvalidConnection:
                    return new InvalidConnectionException();
                case PluginError.InvalidConnectionState:
                    return new InvalidConnectionStateException();
                case PluginError.InvalidSettingKey:
                    return new InvalidSettingKeyException();
                case PluginError.InvalidStream:
                    return new InvalidStreamException();
                case PluginError.InvalidFrame:
                    return new InvalidFrameException();
                case PluginError.InvalidFrameDataKey:
                    return new InvalidFrameDataKeyException();
                case PluginError.InvalidVideoRecordingState:
                    return new InvalidVideoRecordingStateException();
                case PluginError.Shutdown:
                    return new ShutdownException();
                case PluginError.UnsupportedMode:
                    return new UnsupportedModeException();
                case PluginError.UnsupportedCapability:
                    return new UnsupportedCapabilityException();
                case PluginError.Network:
                    return new NetworkException();
                case PluginError.Protocol:
                    return new ProtocolException();
                case PluginError.NoSupportedProtocolVersion:
                    return new NoSupportedProtocolVersionException();
                default:
                    return new PluginException(error);
            }
        }
    }

    [Serializable]
    public class PluginException : Exception
    {
        public ZView.PluginError PluginError { get; private set; }

        public PluginException(ZView.PluginError pluginError)
            : base()
        {
            this.PluginError = pluginError;
        }

        public PluginException(ZView.PluginError pluginError, string message)
            : base(message)
        {
            this.PluginError = pluginError;
        }

        protected PluginException(ZView.PluginError pluginError, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.PluginError = pluginError;
        }
    }

    [Serializable]
    public class FailedException : PluginException
    {
        public FailedException()
            : base(ZView.PluginError.Failed)
        {
        }

        public FailedException(string message)
            : base(ZView.PluginError.Failed, message)
        {
        }

        protected FailedException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.Failed, info, context)
        {
        }
    }

    [Serializable]
    public class NotImplementedException : PluginException
    {
        public NotImplementedException()
            : base(ZView.PluginError.NotImplemented)
        {
        }

        public NotImplementedException(string message)
            : base(ZView.PluginError.NotImplemented, message)
        {
        }

        protected NotImplementedException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.NotImplemented, info, context)
        {
        }
    }

    [Serializable]
    public class NotInitializedException : PluginException
    {
        public NotInitializedException()
            : base(ZView.PluginError.NotInitialized)
        {
        }

        public NotInitializedException(string message)
            : base(ZView.PluginError.NotInitialized, message)
        {
        }

        protected NotInitializedException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.NotInitialized, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidParameterException : PluginException
    {
        public InvalidParameterException()
            : base(ZView.PluginError.InvalidParameter)
        {
        }

        public InvalidParameterException(string message)
            : base(ZView.PluginError.InvalidParameter, message)
        {
        }

        protected InvalidParameterException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidParameter, info, context)
        {
        }
    }

    [Serializable]
    public class OutOfMemoryException : PluginException
    {
        public OutOfMemoryException()
            : base(ZView.PluginError.OutOfMemory)
        {
        }

        public OutOfMemoryException(string message)
            : base(ZView.PluginError.OutOfMemory, message)
        {
        }

        protected OutOfMemoryException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.OutOfMemory, info, context)
        {
        }
    }

    [Serializable]
    public class BufferTooSmallException : PluginException
    {
        public BufferTooSmallException()
            : base(ZView.PluginError.BufferTooSmall)
        {
        }

        public BufferTooSmallException(string message)
            : base(ZView.PluginError.BufferTooSmall, message)
        {
        }

        protected BufferTooSmallException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.BufferTooSmall, info, context)
        {
        }
    }

    [Serializable]
    public class RuntimeNotFoundException : PluginException
    {
        public RuntimeNotFoundException()
            : base(ZView.PluginError.RuntimeNotFound)
        {
        }

        public RuntimeNotFoundException(string message)
            : base(ZView.PluginError.RuntimeNotFound, message)
        {
        }

        protected RuntimeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.RuntimeNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class SymbolNotFoundException : PluginException
    {
        public SymbolNotFoundException()
            : base(ZView.PluginError.SymbolNotFound)
        {
        }

        public SymbolNotFoundException(string message)
            : base(ZView.PluginError.SymbolNotFound, message)
        {
        }

        protected SymbolNotFoundException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.SymbolNotFound, info, context)
        {
        }
    }

    [Serializable]
    public class RuntimeIncompatibleException : PluginException
    {
        public RuntimeIncompatibleException()
            : base(ZView.PluginError.RuntimeIncompatible)
        {
        }

        public RuntimeIncompatibleException(string message)
            : base(ZView.PluginError.RuntimeIncompatible, message)
        {
        }

        protected RuntimeIncompatibleException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.RuntimeIncompatible, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidNodeTypeException : PluginException
    {
        public InvalidNodeTypeException()
            : base(ZView.PluginError.InvalidNodeType)
        {
        }

        public InvalidNodeTypeException(string message)
            : base(ZView.PluginError.InvalidNodeType, message)
        {
        }

        protected InvalidNodeTypeException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidNodeType, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidContextException : PluginException
    {
        public InvalidContextException()
            : base(ZView.PluginError.InvalidContext)
        {
        }

        public InvalidContextException(string message)
            : base(ZView.PluginError.InvalidContext, message)
        {
        }

        protected InvalidContextException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidContext, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidContextStateException : PluginException
    {
        public InvalidContextStateException()
            : base(ZView.PluginError.InvalidContextState)
        {
        }

        public InvalidContextStateException(string message)
            : base(ZView.PluginError.InvalidContextState, message)
        {
        }

        protected InvalidContextStateException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidContextState, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidModeSpecException : PluginException
    {
        public InvalidModeSpecException()
            : base(ZView.PluginError.InvalidModeSpec)
        {
        }

        public InvalidModeSpecException(string message)
            : base(ZView.PluginError.InvalidModeSpec, message)
        {
        }

        protected InvalidModeSpecException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidModeSpec, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidModeException : PluginException
    {
        public InvalidModeException()
            : base(ZView.PluginError.InvalidMode)
        {
        }

        public InvalidModeException(string message)
            : base(ZView.PluginError.InvalidMode, message)
        {
        }

        protected InvalidModeException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidMode, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidModeAttributeKeyException : PluginException
    {
        public InvalidModeAttributeKeyException()
            : base(ZView.PluginError.InvalidModeAttributeKey)
        {
        }

        public InvalidModeAttributeKeyException(string message)
            : base(ZView.PluginError.InvalidModeAttributeKey, message)
        {
        }

        protected InvalidModeAttributeKeyException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidModeAttributeKey, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidConnectionSpecException : PluginException
    {
        public InvalidConnectionSpecException()
            : base(ZView.PluginError.InvalidConnectionSpec)
        {
        }

        public InvalidConnectionSpecException(string message)
            : base(ZView.PluginError.InvalidConnectionSpec, message)
        {
        }

        protected InvalidConnectionSpecException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidConnectionSpec, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidConnectionException : PluginException
    {
        public InvalidConnectionException()
            : base(ZView.PluginError.InvalidConnection)
        {
        }

        public InvalidConnectionException(string message)
            : base(ZView.PluginError.InvalidConnection, message)
        {
        }

        protected InvalidConnectionException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidConnection, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidConnectionStateException : PluginException
    {
        public InvalidConnectionStateException()
            : base(ZView.PluginError.InvalidConnectionState)
        {
        }

        public InvalidConnectionStateException(string message)
            : base(ZView.PluginError.InvalidConnectionState, message)
        {
        }

        protected InvalidConnectionStateException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidConnectionState, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidSettingKeyException : PluginException
    {
        public InvalidSettingKeyException()
            : base(ZView.PluginError.InvalidSettingKey)
        {
        }

        public InvalidSettingKeyException(string message)
            : base(ZView.PluginError.InvalidSettingKey, message)
        {
        }

        protected InvalidSettingKeyException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidSettingKey, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidStreamException : PluginException
    {
        public InvalidStreamException()
            : base(ZView.PluginError.InvalidStream)
        {
        }

        public InvalidStreamException(string message)
            : base(ZView.PluginError.InvalidStream, message)
        {
        }

        protected InvalidStreamException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidStream, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidFrameException : PluginException
    {
        public InvalidFrameException()
            : base(ZView.PluginError.InvalidFrame)
        {
        }

        public InvalidFrameException(string message)
            : base(ZView.PluginError.InvalidFrame, message)
        {
        }

        protected InvalidFrameException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidFrame, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidFrameDataKeyException : PluginException
    {
        public InvalidFrameDataKeyException()
            : base(ZView.PluginError.InvalidFrameDataKey)
        {
        }

        public InvalidFrameDataKeyException(string message)
            : base(ZView.PluginError.InvalidFrameDataKey, message)
        {
        }

        protected InvalidFrameDataKeyException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidFrameDataKey, info, context)
        {
        }
    }

    [Serializable]
    public class InvalidVideoRecordingStateException : PluginException
    {
        public InvalidVideoRecordingStateException()
            : base(ZView.PluginError.InvalidVideoRecordingState)
        {
        }

        public InvalidVideoRecordingStateException(string message)
            : base(ZView.PluginError.InvalidVideoRecordingState, message)
        {
        }

        protected InvalidVideoRecordingStateException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.InvalidVideoRecordingState, info, context)
        {
        }
    }

    [Serializable]
    public class ShutdownException : PluginException
    {
        public ShutdownException()
            : base(ZView.PluginError.Shutdown)
        {
        }

        public ShutdownException(string message)
            : base(ZView.PluginError.Shutdown, message)
        {
        }

        protected ShutdownException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.Shutdown, info, context)
        {
        }
    }

    [Serializable]
    public class UnsupportedModeException : PluginException
    {
        public UnsupportedModeException()
            : base(ZView.PluginError.UnsupportedMode)
        {
        }

        public UnsupportedModeException(string message)
            : base(ZView.PluginError.UnsupportedMode, message)
        {
        }

        protected UnsupportedModeException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.UnsupportedMode, info, context)
        {
        }
    }

    [Serializable]
    public class UnsupportedCapabilityException : PluginException
    {
        public UnsupportedCapabilityException()
            : base(ZView.PluginError.UnsupportedCapability)
        {
        }

        public UnsupportedCapabilityException(string message)
            : base(ZView.PluginError.UnsupportedCapability, message)
        {
        }

        protected UnsupportedCapabilityException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.UnsupportedCapability, info, context)
        {
        }
    }

    [Serializable]
    public class NetworkException : PluginException
    {
        public NetworkException()
            : base(ZView.PluginError.Network)
        {
        }

        public NetworkException(string message)
            : base(ZView.PluginError.Network, message)
        {
        }

        protected NetworkException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.Network, info, context)
        {
        }
    }

    [Serializable]
    public class ProtocolException : PluginException
    {
        public ProtocolException()
            : base(ZView.PluginError.Protocol)
        {
        }

        public ProtocolException(string message)
            : base(ZView.PluginError.Protocol, message)
        {
        }

        protected ProtocolException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.Protocol, info, context)
        {
        }
    }

    [Serializable]
    public class NoSupportedProtocolVersionException : PluginException
    {
        public NoSupportedProtocolVersionException()
            : base(ZView.PluginError.NoSupportedProtocolVersion)
        {
        }

        public NoSupportedProtocolVersionException(string message)
            : base(ZView.PluginError.NoSupportedProtocolVersion, message)
        {
        }

        protected NoSupportedProtocolVersionException(SerializationInfo info, StreamingContext context)
            : base(ZView.PluginError.NoSupportedProtocolVersion, info, context)
        {
        }
    }
}

