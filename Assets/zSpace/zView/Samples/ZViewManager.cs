using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zSpace.zView.Samples
{
    public class ZViewManager : MonoBehaviour
    {
        private ZView _zView = null;
        void Awake()
        {
            _zView = GameObject.FindObjectOfType<ZView>();
            if (_zView == null)
            {
                Debug.LogError("Unable to find reference to zSpace.zView.ZView Monobehaviour.");
                this.enabled = false;
                return;
            }
            MultPlateformEvent.OpenZView += OpenZView;
        }
        private void OnDestroy()
        {
            MultPlateformEvent.OpenZView -= OpenZView;
        }
        public void OpenZView()
        {
            IntPtr connection = _zView.GetCurrentActiveConnection();
            if (connection == IntPtr.Zero)
            {
                _zView.ConnectToDefaultViewer();
                _zView.SetConnectionMode(_zView.GetCurrentActiveConnection(), _zView.GetAugmentedRealityMode());
            }
            else
            {
                _zView.CloseConnection(
                        connection,
                        ZView.ConnectionCloseAction.None,
                        ZView.ConnectionCloseReason.UserRequested,
                        string.Empty);
            }
            
        }

    }
}
