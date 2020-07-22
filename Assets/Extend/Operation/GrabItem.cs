using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrabItem : MonoBehaviour
{
    private Vector3 _initialGrabOffset = Vector3.zero;
    private Quaternion _initialGrabRotation = Quaternion.identity;
    private float _initialGrabDistance = 0.0f;
    private bool isGrab = false;
    private float dis = 20;
    private Vector3 lastpos;
    public GameObject colliderObj;
    private Collider[] colliders;
    private ViveRaycaster raycaster;
    private float lastDis;
    /// <summary>
    /// 速度
    /// </summary>
    [SerializeField, HideInInspector]
    private float _speed = 20;
    public float _Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
        }
    }
    public bool IsGrab
    {
        get { return isGrab; }
        set { isGrab = value;
            if(isGrab)
            {
                farDis = _ViveRaycaster.FarDistance;
            }
            else
            {
                _ViveRaycaster.FarDistance = farDis;
            }
            SetCollider(!isGrab);
            if (isGrab)
            {
               
                transform.position = GlobeData._RightRaycaster.FirstRaycastResult().worldPosition;
                //dis = GlobeData._RightRaycaster.FirstRaycastResult().distance;
                dis = 20f;
                lastDis = 0;
                BeginGrab(gameObject, dis, GlobeData._RightRaycaster.BreakPoints[0],
                    GlobeData._RightRaycaster.transform.rotation);
            }
        }
    }
    public ViveRaycaster _ViveRaycaster
    {
        get {
            if(raycaster==null)
            {
                raycaster = GlobeData._RightRaycaster.GetComponentInChildren<ViveRaycaster>(true);
            }
            return raycaster;
        }
    }
    float farDis;
    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
    }
    private void SetCollider(bool bol)
    {
        if(colliders!=null)
        {
            for(int i=0;i<colliders.Length;i++)
            {
                colliders[i].enabled = bol;
            }
        }
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }  
   
    /// <summary>
    /// 中键按下后设置拖拽初始化数据
    /// </summary>
    /// <param name="hitObject"></param>
    /// <param name="hitDistance"></param>
    /// <param name="inputPosition"></param>
    /// <param name="inputRotation"></param>
    private void BeginGrab(GameObject hitObject, float hitDistance, Vector3 inputPosition, Quaternion inputRotation)
    {
        //Debug.Log("BeginGrab:"+hitObject.gameObject.name);
        Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * hitDistance));
        _initialGrabOffset = Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - inputEndPosition);
        _initialGrabRotation = Quaternion.Inverse(inputRotation) * hitObject.transform.rotation;
        _initialGrabDistance = hitDistance;            
    }

    /// <summary>
    /// 更新拖拽物体的位置旋转信息
    /// </summary>
    /// <param name="inputPosition"></param>
    /// <param name="inputRotation"></param>
    private void UpdateGrab(Vector3 inputPosition, Quaternion inputRotation)
    {
       
         float d = _initialGrabDistance;

        //if (GlobeData._RightRaycaster._Result.isValid)
        //{

        //    //d = GlobeData._RightRaycaster.FirstRaycastResult().distance;
        //    float newDis = GlobeData._RightRaycaster.FirstRaycastResult().distance;
        //    d = newDis;        
        //}
        Vector3 rayDir = GlobeData._RightRaycaster.BreakPoints[1] - GlobeData._RightRaycaster.BreakPoints[0];
        RaycastHit hit;
        if (Physics.Raycast(GlobeData._RightRaycaster.BreakPoints[0], rayDir, out hit))
        {
            d=hit.distance;
        }
        _ViveRaycaster.FarDistance = d;

        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {

            Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * d));
            Debug.DrawLine(inputPosition, inputEndPosition, Color.black);
            // Update the grab object's rotation.
            Quaternion objectRotation = inputRotation * _initialGrabRotation;
            // Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
            // Debug.DrawLine(inputPosition, objectPosition, Color.green);
            transform.position = inputEndPosition;
            transform.rotation = objectRotation;
           
        }
        else if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
        {
            Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * d));
            Debug.DrawLine(inputPosition, inputEndPosition, Color.black);
            // Update the grab object's rotation.
            Quaternion objectRotation = inputRotation * _initialGrabRotation;
            // Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
            // Debug.DrawLine(inputPosition, objectPosition, Color.green);
            transform.position = inputEndPosition;
            //transform.rotation = objectRotation;
        }
    }

    public void Update()
    {
        if (isGrab)
        {
            if (GlobeData._RightRaycaster.BreakPoints.Count > 0)
            {
                UpdateGrab(GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
            }
        }
    }
}
