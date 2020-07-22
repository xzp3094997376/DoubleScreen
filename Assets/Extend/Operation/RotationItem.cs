using UnityEngine;
/// <summary>
/// 旋转功能,Zspeace 旋转和拖拽粘合在一起没有分开
/// 只是把PC旋转给独立出来
/// </summary>
public partial class OperationBaseItem
{
    /// <summary>
    /// 是否可选择
    /// </summary>
    [SerializeField, HideInInspector]
    private bool _enableRotate = true;
    public bool _EnableRotate
    {
        get
        {
            return _enableRotate;
        }
        set { _enableRotate = value;
            //_enableX = value;
            //_enableY = value;
        }
    }
    [SerializeField, HideInInspector]
    private bool _enableX = true;
    public bool _EnableX {
        get { return _enableX; }
        set { _enableX = value;
           
        }
    }

    [SerializeField, HideInInspector]
    private bool _enableY = true;
    public bool _EnableY
    {
        get { return _enableY; }
        set { _enableY = value;
            
        }
    }
    [SerializeField, HideInInspector]
    private bool _enableZ = false;
    public bool _EnableZ
    {
        get { return _enableZ; }
        set { _enableZ = value;            
        }
    }
    [SerializeField,HideInInspector]
    private float _rotationSpeed=8f;
    /// <summary>
    /// 旋转速度
    /// </summary>
    public float _RotationSpeed
    {
        get {
            return  _rotationSpeed;
        }
        set {
            _rotationSpeed = Mathf.Abs(value);
        }
    }
    /// <summary>
    /// 笔按下时记录笔端位置作为基准点，用作 判断移动方向
    /// </summary>
    private Vector2 roaPenPos;
 
    /// <summary>
    /// 记录笔上一帧移动的点，用作判断移动距离
    /// </summary>
    private Vector3 lastPenPos;

    private Vector3 lastMousePos;
    #region 适配PC旋转
    private void PcRotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
            if (GlobeData._RightRaycaster._Result.gameObject == gameObject)
            {
                CheckAction();
                validClick = true;
            }
            else
            {
                OperationBaseItem zoom = isCheckChild();
                if (zoom != null && zoom == this)
                {
                    CheckAction();
                    validClick = true;
                }
                else
                {
                    validClick = false;
                }
            }
            if (action == 1)
            {
                GlobeData._DragObj = _Tran;
            }
            else
            {
                GlobeData._DragObj = transform;
            }
  
        }
        if (Input.GetMouseButton(1) && validClick&&_enableRotate)
        {
            Vector2 offset = lastMousePos - Input.mousePosition;
            if (action == 1 && _Tran != null)
            {
                if (_enableX & _enableY)
                {
                    _Tran.Rotate(-Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.World);
                    _Tran.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.World);
                }
                else
                {
                    if (_enableX)
                    {
                        _Tran.Rotate(-Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.Self);
                    }
                    if (_enableY)
                    {
                        _Tran.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.Self);
                    }
                }              
            }
            else
            {
                if (_enableX & _enableY)
                {               
                    transform.Rotate(-Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.World);
                    transform.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.World);
                }
                else
                {
                    if (_enableX)
                    {

                        transform.Rotate(-Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.Self);
                    }
                    if (_enableY)
                    {
                        transform.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.Self);
                    }
                }
               
            }
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            GlobeData._DragObj = null;
            validClick = false;
        }
    }

    #endregion
    private void RotationAxis(Transform t)
    {
        //计算方向向量       
        Vector3 nowPos = GlobeData.GetCurrentEventCamera().WorldToScreenPoint(GlobeData._RightRaycaster.BreakPoints[0]);
        Vector3 offset = lastPenPos - nowPos;
        CheckAction();
        if (action == 1 && _Tran != null)
        {
            if (_enableX)
            {
                _Tran.Rotate(Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.Self);
            }
            if (_enableY)
            {
                _Tran.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.Self);
            }
        }
        else
        {
            if (_enableX)
            {
                transform.Rotate(Vector3.right * offset.y * Time.deltaTime * _RotationSpeed, Space.Self);
            }
            if (_enableY)
            {
                transform.Rotate(Vector3.up * offset.x * Time.deltaTime * _RotationSpeed, Space.Self);
            }
        }      
        lastPenPos = nowPos;
    }
}
