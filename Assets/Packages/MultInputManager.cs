using HTC.UnityPlugin.Pointer3D;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using zSpace.Core;

/*          按键说明
 * var midPressed = _zCore.IsTargetButtonPressed(ZCore.TargetType.Primary, 1);
 * var leftPressed = _zCore.IsTargetButtonPressed(ZCore.TargetType.Primary, 0);
 * var rightPressed = _zCore.IsTargetButtonPressed(ZCore.TargetType.Primary, 2);
 * 
 * deviceState.SetButtonPress(VRModuleRawButton.Trigger, leftPressed);
 * deviceState.SetButtonPress(VRModuleRawButton.Grip, midPressed);
 * deviceState.SetButtonPress(VRModuleRawButton.Touchpad, rightPressed);
 *
 */

public enum StylusState
{
    Idle = 0,
    Grab = 1,
}
public class MultInputManager : MonoBehaviour
{
    private Pointer3DRaycaster raycaster;
    private bool _wasButtonPressed = false;
    private Vector3 _initialGrabOffset = Vector3.zero;
    private Quaternion _initialGrabRotation = Quaternion.identity;
    private float _initialGrabDistance = 0.0f;
    public  StylusState StylusFunctionState = StylusState.Idle;
    public GameObject CurrObject;//当前物体
    private void Awake()
    {
        raycaster = this.GetComponent<Pointer3DRaycaster>();
    }

    void Update()
    {
        #region zspace

        #endregion
        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {
            bool isButtonPressed = ViveInput.GetPress(HandRole.RightHand, ControllerButton.Trigger)||
                                    ViveInput.GetPress(HandRole.RightHand, ControllerButton.Grip)||
                                    ViveInput.GetPress(HandRole.RightHand, ControllerButton.PadTouch);
            Vector3 rayDir = raycaster.BreakPoints[1] - raycaster.BreakPoints[0];
            switch (StylusFunctionState)
            {
                case StylusState.Idle:
                    {
                        // Perform a raycast on the entire scene to determine what the
                        // stylus is currently colliding with.
                        RaycastHit hit;

                        if (Physics.Raycast(raycaster.BreakPoints[0], rayDir, out hit))
                        {
                            // If the front stylus button was pressed, initiate a grab.
                            if (isButtonPressed && !_wasButtonPressed)
                            {
                                // Begin the grab.
                                this.BeginGrab(hit.collider.gameObject, hit.distance, raycaster.BreakPoints[0], this.transform.rotation);

                                StylusFunctionState = StylusState.Grab;
                            }
                        }
                    }
                    break;

                case StylusState.Grab:
                    {
                        // Update the grab.
                        this.UpdateGrab(raycaster.BreakPoints[0], this.transform.rotation);
                        
                        // End the grab if the front stylus button was released.
                        if (!isButtonPressed && _wasButtonPressed)
                        {
                            StylusFunctionState = StylusState.Idle;
                        }
                    }
                    break;

                default:
                    break;
            }

            // Cache state for next frame.
            _wasButtonPressed = isButtonPressed;
        }

    }
    #region Private Helpers
    private void UpdateGrab(Vector3 inputPosition, Quaternion inputRotation)
    {
        Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * _initialGrabDistance));

        // Update the grab object's rotation.
        Quaternion objectRotation = inputRotation * _initialGrabRotation;
        // Update the grab object's position.
        Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);

        if (CurrObject)
        {
            CurrObject.transform.rotation = objectRotation;
            CurrObject.transform.position = objectPosition;
        }
        

    }
    private void BeginGrab(GameObject hitObject, float hitDistance, Vector3 inputPosition, Quaternion inputRotation)
    {
        Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * hitDistance));

        // Cache the initial grab state.
        CurrObject = hitObject;
        _initialGrabOffset = Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - inputEndPosition);
        _initialGrabRotation = Quaternion.Inverse(inputRotation) * hitObject.transform.rotation;
        _initialGrabDistance = hitDistance;
    }
    #endregion
}
