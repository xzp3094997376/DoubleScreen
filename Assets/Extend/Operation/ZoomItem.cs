using UnityEngine;
/// <summary>
/// 缩放功能
/// </summary>
public partial class OperationBaseItem
{
    /// <summary>
    /// 是否可以缩放
    /// </summary>
    [SerializeField, HideInInspector]
    private bool _enbaleZoom = true;
    public bool _EnbaleZoom
    {
        get { return _enbaleZoom; }
        set { _enbaleZoom = value; }
    }
    /// <summary>
    /// 最小缩放
    /// </summary>
    [SerializeField, HideInInspector]
    private float _minScale = 0.5f;
    public float _MinScale
    {
        get { return _minScale; }
        set { _minScale = value; }
    }
    /// <summary>
    /// 记录模型初始缩放
    /// </summary>
    private Vector3 localSacle;
    /// <summary>
    /// 记录模型初始位置
    /// </summary>
    private Vector3 localPos;
    /// <summary>
    /// 记录模型初始旋转
    /// </summary>
    private Vector3 localRoatation;
    /// <summary>
    /// 最大缩放
    /// </summary>
    [SerializeField, HideInInspector]
    private float _maxScale = 3;
    public float _MaxScale
    {
        get { return _maxScale; }
        set { _maxScale = value; }
    }
    /// <summary>
    /// 速度
    /// </summary>
    [SerializeField, HideInInspector]
    private float _speed = 10;
    public float _Speed
    {
        get
        {
            return _speed;
        }
        set {
            _speed = value;
        }
    }
    /// <summary>
    /// 笔的位置基准点
    /// </summary>
    private Vector3 penPos;
    /// <summary>
    /// 当前物体缩放基准值
    /// </summary>
    private Vector3 hoverScale = Vector3.zero;
    /// <summary>
    /// 其它 物体缩放基准值
    /// </summary>
    private Vector3 otherScale = Vector3.zero;
    private float hoverDis = 0;

    #region 适配Zspace缩放
    private void onRightRightPressDown()
    {
       // Debug.Log("onRightRightPressDown");
        OperationBaseItem zom = isCheckChild();
        bool isChild = false;
        if (zom != null && zom == this)
        {
            isChild = true;
        }
        if (GlobeData._RightRaycaster._Result.gameObject == null)
        {
            validClick = false;
        }
        else if (GlobeData._RightRaycaster._Result.gameObject != gameObject)
        {

            validClick = isChild;
        }
        else
        {
            validClick = true;

        }
        if (!validClick)
        {
            return;
        }
        //记录触控笔的端点，用于计算下一帧 触控的端点移动的距离
        penPos = GlobeData._RightRaycaster.BreakPoints[0];
        hoverDis = Vector3.Distance(GlobeData._RightRaycaster.BreakPoints[0], GlobeData._RightRaycaster.BreakPoints[1]);
        CheckAction();
        if (action == 1)
        {
            GlobeData._DragObj = _Tran;
        }
        else
        {
            GlobeData._DragObj = transform;
        }
    }
    /// <summary>
    /// 监听 右手 右键 抬起
    /// </summary>
    private void onRightRightPressUp()
    {
        //Debug.Log("onRightRightPressUp");
        if (GlobeData._RightRaycaster._Result.gameObject == null)
        {
            return;
        }
        validClick = false;
        hoverDis = 0;
        hoverScale = Vector3.zero;
        otherScale = Vector3.zero;
        GlobeData._DragObj = null;
    }


    /// <summary>
    /// 处理模型缩放
    /// </summary>
    private void onRightRightPress()
    {
       // Debug.Log("onRightRightPress");
        if (!_enbaleZoom)
        {
            return;
        }
        if (!validClick)
        {
            return;
        }
        //计算方向向量
        Vector3 di = (GlobeData._RightRaycaster.BreakPoints[0] - penPos).normalized;
        //计算触控笔在缩放物体时是向前还是向后移动
        float dir = Vector3.Dot(penPos, di);
        if (dir == 0)
        {
            return;
        }
        dir = dir / Mathf.Abs(dir);
        //用笔按下时记录笔的端点，和当前笔的端点 计算长度
        float dis = dir * Vector3.Distance(penPos, GlobeData._RightRaycaster.BreakPoints[0]);
        //将笔移动的长度 + 笔按下时(计算出笔端点和检测到模型碰撞点之间)的距离
        //做比例运算
        float v = (dis + hoverDis) / hoverDis;

        if (action == -1)//协同缩放
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ClampVec(_minScale, hoverScale * v, _maxScale), Time.deltaTime * _speed);
            if (_Tran != null)
            {
                _Tran.localScale = Vector3.Lerp(_Tran.localScale, ClampVec(_minScale, otherScale * v, _maxScale), Time.deltaTime * _speed);
            }
        }
        else if (action == 0)//单体缩放
        {
            transform.localScale = Vector3.Lerp(transform.localScale, ClampVec(_minScale, hoverScale * v, _maxScale), Time.deltaTime * _speed);
        }
        else if (action == 1)//分身缩放
        {
            if (_Tran != null)
            {
                _Tran.localScale = Vector3.Lerp(_Tran.localScale, ClampVec(_minScale, otherScale * v, _maxScale), Time.deltaTime * _speed);
            }
        }
    }
    #endregion
    #region 适配PC缩放
    /// <summary>
    /// 适配普通PC缩放
    /// </summary>
    private void PcScale()
    {
        bool isScale = false;
        if (GlobeData._RightRaycaster._Result.gameObject == null)
        {
            isScale = false;
        }
        else if (GlobeData._RightRaycaster._Result.gameObject == gameObject)
        {
            isScale = true;
        }
        else
        {
            OperationBaseItem zoom = isCheckChild();
            if (zoom != null && zoom == this)
            {
                isScale = true;
            }
            else
            {
                isScale = false;
            }
        }
        if (isScale)
        {
            CheckAction();
            int dir = 1;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                dir = 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                dir = -1;

            }
            else
            {
                dir = 0;
            }
            //Debug.Log("dir:" + dir);
            if (dir != 0)
            {
                if (action == -1)//协同缩放
                {
                    Vector3 v = transform.localScale + dir * Vector3.one * Time.deltaTime * _speed;
                    transform.localScale = ClampVec(_minScale, v, _maxScale);
                    if (_Tran != null)
                    {
                        v = _Tran.localScale + dir * Vector3.one * Time.deltaTime * _speed;
                        _Tran.localScale = ClampVec(_minScale, v, _maxScale);
                    }
                }
                else if (action == 0)//单体缩放
                {

                    Vector3 v = transform.localScale + dir * Vector3.one * Time.deltaTime * _speed;
                    transform.localScale = ClampVec(_minScale, v, _maxScale);
                }
                else if (action == 1)//分身缩放
                {
                    if (_Tran != null)
                    {
                        Vector3 v = _Tran.localScale + dir * Vector3.one * Time.deltaTime * _speed;
                        _Tran.localScale = ClampVec(_minScale, v, _maxScale);
                    }
                }
            }
        }

    }
    #endregion
    private Vector3 ClampVec(float min, Vector3 v, float max)
    {        
        float newx = Mathf.Clamp(v.x, min*localSacle.x, max);
        float newy = Mathf.Clamp(v.y, min * localSacle.y, max);
        float newz = Mathf.Clamp(v.y, min * localSacle.z, max);
        return new Vector3(newx, newy, newz);
    }
}
