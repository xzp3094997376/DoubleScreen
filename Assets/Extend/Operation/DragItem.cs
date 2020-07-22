using UnityEngine;
/// <summary>
/// 拖拽功能
/// </summary>
public partial class OperationBaseItem
{
    private Vector3 _initialGrabOffset = Vector3.zero;
    private Quaternion _initialGrabRotation = Quaternion.identity;
    private float _initialGrabDistance = 0.0f;
    private Vector3 lastpos;
    /// <summary>
    /// 是否可以移动
    /// </summary>
    [SerializeField, HideInInspector]
    private bool _enableMove = true;
    public bool _EnableMove
    {
        get { return _enableMove; }
        set { _enableMove = value;
            if(_enableMove)
            {
                _enableRotate = true;
               _enableX= true;
                _enableY = true;
            }
        }
    }
    private void OnRightLeftPressUp()
    {
        GlobeData._DragObj = null;
        validClick = false;
        action = -10;
    }
    #region 适配Zspace拖拽
    /// <summary>
    /// 监听左键按下
    /// </summary>
    private void OnRightLeftPressDown()
    {
        if (GlobeData._RightRaycaster._Result.gameObject == null)
        {
            validClick = false;
            return;
        }
        if (GlobeData._RightRaycaster._Result.gameObject.layer == 5)
        {
            validClick = false;
            return;
        }
        if (GlobeData._RightRaycaster._Result.gameObject == gameObject)
        {
            validClick = true;
        }
        else
        {
            OperationBaseItem zom = isCheckChild();
            bool isChild = false;
            if (zom != null && zom == this)
            {
                isChild = true;
            }
            validClick = isChild;
        }
        if (!validClick)
        {
            return;
        }
        CheckAction();
        Vector3 rayDir = GlobeData._RightRaycaster.BreakPoints[1] - GlobeData._RightRaycaster.BreakPoints[0];
        RaycastHit hit;
        if (Physics.Raycast(GlobeData._RightRaycaster.BreakPoints[0], rayDir, out hit))
        {
            // If the front stylus button was pressed, initiate a grab.           
            // Begin the grab.   
            if (hit.collider.gameObject == gameObject)
            {
                // GlobeData._DragObj = transform;
                //if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
                //{
                    BeginGrab(hit.collider.gameObject, hit.distance, GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
                //}
               // else if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
               // {
                   // BeginGrab(hit.collider.gameObject, hit.distance, GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
                //}
            }
            else
            {
                //点击对象是该对象的子节点                              
                // Debug.Log(name + "_:" + hit.collider.gameObject.GetComponentInParent<Zoom>().name);
                //if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
               // {
                    BeginGrab(gameObject, hit.distance, GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
               // }
               // else if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
               // {
                    //BeginGrab(gameObject, hit.distance, GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
               // }
            }
        }
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
        Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * hitDistance));
        _initialGrabOffset = Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - inputEndPosition);
        _initialGrabRotation = Quaternion.Inverse(inputRotation) * hitObject.transform.rotation;
        _initialGrabDistance = hitDistance;
        GlobeData._DragObj = transform;
        //设置单轴旋转基准点
       // roaPenPos =GlobeData._ZspaceCamera.WorldToScreenPoint(inputPosition);
       // lastPenPos = roaPenPos;
        lastPenPos = GlobeData.GetCurrentEventCamera().WorldToScreenPoint(inputPosition);
       // Debug.Log("press 屏幕坐标点："+ roaPenPos);
        if (action == 1 && _Tran != null)//分身缩放
        {
            GlobeData._DragObj = _Tran;
            _initialGrabOffset = Quaternion.Inverse(_Tran.rotation) * (_Tran.position - inputEndPosition);
            _initialGrabRotation = Quaternion.Inverse(inputRotation) * _Tran.rotation;
        }
    }
    /// <summary>
    /// 左键按下 持续拖动
    /// </summary>
    private void OnRightLeftPress()
    {
        if (validClick)
        {
            UpdateGrab(GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.transform.rotation);
        }
    }
    private void Rotation(Transform t,Quaternion quaternion)
    {
        if (_enableRotate)
        {
            if(_enableX&_enableY)
            {
                transform.rotation = quaternion;
               
            }
            else
            {
                RotationAxis(t);
            }            
        }
    }
    /// <summary>
    /// 更新拖拽物体的位置旋转信息
    /// </summary>
    /// <param name="inputPosition"></param>
    /// <param name="inputRotation"></param>
    private void UpdateGrab(Vector3 inputPosition, Quaternion inputRotation)
    {
        Vector3 inputEndPosition = inputPosition + (inputRotation * (Vector3.forward * _initialGrabDistance));
        // Update the grab object's rotation.
        Quaternion objectRotation = inputRotation * _initialGrabRotation;
        // Update the grab object's position.    
        if (PlateformData.GetCurrentPlatform() == PlatformType.zSpace)
        {
            if (action == 1 && _Tran != null)
            {
                Rotation(_Tran, objectRotation);
                if (_enableMove)
                {
                    Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
                    _Tran.position = objectPosition;
                }
            }
            else //如果等于-1不处理 直接拖拽 
            {
                Rotation(transform,objectRotation);
                if (_enableMove)
                {
                    Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
                    transform.position = objectPosition;
                }
            }
        }
        else if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
        {
            if (action == 1 && _Tran != null)
            {
                //Rotation(_Tran, objectRotation);
                if (_enableMove)
                {
                    Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
                    _Tran.position = objectPosition;
                }
            }
            else //如果等于-1不处理 直接拖拽 
            {
               // Rotation(transform, objectRotation);
                if (_enableMove)
                {
                    Vector3 objectPosition = inputEndPosition + (objectRotation * _initialGrabOffset);
                    transform.position = objectPosition;
                }
            }

        }
    }
    #endregion
    #region 适配PC拖拽
    /// <summary>
    /// 检测是否拖动子节点
    /// </summary>
    private void DragOther()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(GlobeData._RightRaycaster._Result.gameObject);
            OperationBaseItem zom = isCheckChild();
            if (zom != null && zom == this)
            {
                isChild = true;
                CheckAction();
                GlobeData._DragObj = transform;
                //三维物体坐标转屏幕坐标
                Vector3 screenSpace = GlobeData.GetCurrentEventCamera().WorldToScreenPoint(GlobeData._DragObj.position);
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
                //物体的位置，屏幕坐标转换为世界坐标
                Vector3 objectPosition = GlobeData.GetCurrentEventCamera().ScreenToWorldPoint(mousePosition);
                lastpos = objectPosition;
            }
        }
        if (Input.GetMouseButton(0) && GlobeData._DragObj != null && GlobeData._RightRaycaster._Result.gameObject != null)
        {
            if (isChild)
            {
                OnMouseDrag();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            GlobeData._DragObj = null;
            isChild = false;
        }
    }
    private void OnMouseDown()
    {
        if (PlateformData.GetCurrentPlatform() == PlatformType.PC && _enableMove)
        {
            CheckAction();
            if (action == 1)
            {
                GlobeData._DragObj = _Tran;
            }
            else
            {
                GlobeData._DragObj = transform;
            }
            //三维物体坐标转屏幕坐标
            Vector3 screenSpace = GlobeData.GetCurrentEventCamera().WorldToScreenPoint(transform.position);
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            //物体的位置，屏幕坐标转换为世界坐标
            Vector3 objectPosition = GlobeData.GetCurrentEventCamera().ScreenToWorldPoint(mousePosition);
            lastpos = objectPosition;
        }
    }
    private void OnMouseUp()
    {
        if (PlateformData.GetCurrentPlatform() == PlatformType.PC)
        {
            GlobeData._DragObj = null;
        }
    }
    private void OnMouseDrag()
    {
        //Debug.Log("OnMouseDrag:"+Time.time );
        if(!enabled)
        {
            return;
        }
        if (PlateformData.GetCurrentPlatform() == PlatformType.PC && _enableMove)
        {
            //三维物体坐标转屏幕坐标
            Vector3 screenSpace = GlobeData.GetCurrentEventCamera().WorldToScreenPoint(transform.position);
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            //物体的位置，屏幕坐标转换为世界坐标
            Vector3 objectPosition = GlobeData.GetCurrentEventCamera().ScreenToWorldPoint(mousePosition);
            Vector3 offset = objectPosition - lastpos;
            //Vector3 pos = GlobeData._DragObj.position + offset;
            // GlobeData._DragObj.position = Vector3.Lerp(GlobeData._DragObj.position, pos, speed * Time.deltaTime * 10);

            // transform.position += offset;
            if (action == 1 && _Tran != null)
            {
                Vector3 pos = _Tran.position + offset;
                _Tran.position = Vector3.Lerp(_Tran.position, pos, _speed * Time.deltaTime * 10);
            }
            else
            {
                Vector3 pos = transform.position + offset;
                transform.position = Vector3.Lerp(transform.position, pos, _speed * Time.deltaTime * 10);
            }
            lastpos = objectPosition;
        }
    }
    #endregion
}
