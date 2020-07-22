//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.PoseTracker;
using HTC.UnityPlugin.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;
using zSpace.Core;

namespace HTC.UnityPlugin.Vive
{
    [AddComponentMenu("HTC/VIU/Device Tracker/Vive Pose Tracker (Transform)", 7)]
    // Simple component to track Vive devices.
    public class VivePoseTracker : BasePoseTracker, INewPoseListener, IViveRoleComponent
    {
        [Serializable]
        public class UnityEventBool : UnityEvent<bool> { }

        private bool m_isValid;

        public Transform origin;

        [SerializeField]
        private ViveRoleProperty m_viveRole = ViveRoleProperty.New(HandRole.RightHand);

        public UnityEventBool onIsValidChanged;

        [HideInInspector]
        [Obsolete("Use VivePoseTracker.viveRole instead")]
        public DeviceRole role = DeviceRole.Invalid;

        public ViveRoleProperty viveRole { get { return m_viveRole; } }

        public bool isPoseValid { get { return m_isValid; } }

        protected void SetIsValid(bool value, bool forceSet = false)
        {
            if (ChangeProp.Set(ref m_isValid, value) || forceSet)
            {
                if (onIsValidChanged != null)
                {
                    onIsValidChanged.Invoke(value);
                }
            }
        }

        protected virtual void Start()
        {
            
            SetIsValid(VivePose.IsValid(m_viveRole), true);
        }
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // change old DeviceRole value to viveRole value
            var serializedObject = new UnityEditor.SerializedObject(this);

            var roleValueProp = serializedObject.FindProperty("role");
            var oldRoleValue = roleValueProp.intValue;

            if (oldRoleValue != (int)DeviceRole.Invalid)
            {
                Type newRoleType;
                int newRoleValue;

                if (oldRoleValue == -1)
                {
                    newRoleType = typeof(DeviceRole);
                    newRoleValue = (int)DeviceRole.Hmd;
                }
                else
                {
                    newRoleType = typeof(HandRole);
                    newRoleValue = oldRoleValue;
                }

                if (Application.isPlaying)
                {
                    roleValueProp.intValue = (int)DeviceRole.Invalid;
                    m_viveRole.Set(newRoleType, newRoleValue);
                }
                else
                {
                    roleValueProp.intValue = (int)DeviceRole.Invalid;
                    serializedObject.ApplyModifiedProperties();
                    m_viveRole.Set(newRoleType, newRoleValue);
                    serializedObject.Update();
                }
            }
            serializedObject.Dispose();
        }
#endif
        private PlatformType currentPlatform;
        protected virtual void OnEnable()
        {
            
            VivePose.AddNewPosesListener(this);
        }

        protected virtual void OnDisable()
        {
            VivePose.RemoveNewPosesListener(this);

            SetIsValid(false);
        }

        public virtual void BeforeNewPoses() { }

        public virtual void OnNewPoses()
        {
            currentPlatform = (PlatformType)Enum.Parse(typeof(PlatformType), PlayerPrefs.GetString("CurrentPlateform", "PC"));
            if (currentPlatform== PlatformType.zSpace)
            {
                TrackZspacePose();
            }
            else if(currentPlatform == PlatformType.SteamVR)
            {
                TrackVivePose();
            }
            else if (currentPlatform == PlatformType.PC)
            {
                TrackPcPose();
            }
        }
        private void TrackPcPose()
        {
            if (MultiDisplayUtil.GetCurrentDisplay() == DisplayIndex.Display2)
                return;
            var deviceIndex = m_viveRole.GetDeviceIndex();
            var isValid = true;

            if (isValid)
                TrackPose(GetPcCameraPose(), origin);
            SetIsValid(true);
        }
        private void TrackVivePose()
        {
            var deviceIndex = m_viveRole.GetDeviceIndex();
            var isValid = VivePose.IsValid(deviceIndex);

            if (isValid)
            {
                TrackPose(VivePose.GetPose(deviceIndex), origin);
            }

            SetIsValid(isValid);
        }
        private void TrackZspacePose()
        {
            _zCore = GameObject.FindObjectOfType<ZCore>();
            if (_zCore == null)
                return;
            bool isValid = _zCore.IsTargetVisible(ZCore.TargetType.Primary);

            if (isValid)
                TrackPose(GetZspacePose(), origin);
            
            SetIsValid(isValid);
        }
        public static RigidPose GetPcCameraPose(Transform origin = null)
        {
            Camera current=GlobeData.GetCurrentEventCamera();
#if UNITY_EDITOR
            Ray hitRay = current.ScreenPointToRay(Input.mousePosition);
#else
            Ray hitRay = current.ScreenPointToRay(Display.RelativeMouseAt(Input.mousePosition));
#endif
            Vector3 firPoint = current.transform.position;
            Quaternion dirVector3 = Quaternion.identity;
            dirVector3.SetLookRotation(hitRay.direction);
            //Debug.DrawRay(firPoint, hitRay.GetPoint(100), Color.yellow);
            RigidPose zP = new RigidPose(firPoint, dirVector3);
            return zP;

        }  
        private static ZCore _zCore;
        public static RigidPose GetZspacePose(Transform origin = null)
        {
            ZCore.Pose zPose = _zCore.GetTargetPose(ZCore.TargetType.Primary, ZCore.CoordinateSpace.World);

            Vector3 firPoint = zPose.Position;
            Quaternion dirVector3 = zPose.Rotation;
            RigidPose zP = new RigidPose(firPoint, dirVector3);
            return zP;

        }
        public virtual void AfterNewPoses() { }
    }
}