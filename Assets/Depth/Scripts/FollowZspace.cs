using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
public class PcCameraRenderCallbacks:MonoBehaviour
{
    public delegate void EventHandler(Camera sender);

    public event EventHandler PreRender;
    public event EventHandler PostRender;

    void OnPreRender()
    {
        if (this.PreRender != null)
        {
            this.PreRender(this.GetComponent<Camera>());
        }
    }

    void OnPostRender()
    {
        if (this.PostRender != null)
        {
            this.PostRender(this.GetComponent<Camera>());
        }
    }
}
public class FollowZspace : MonoBehaviour
{
    #region 字段


#if UNITY_EDITOR

    [Header("按K键记录zspace视角")]
    public bool RecordZspaceData;
    [Header("相机剪裁远端距离")]
    public float m_farClip;
#endif

    [Header("PC相机")]
    public Camera pCamera;
    [Header("深度与RGB相机")]
    public List<Camera> m_listCamera;
    [SerializeField]
    [Header("深度参数修改")]
    public GetCameraDepth m_depth;
    [SerializeField]
    [Header("指定深度缩放")]
    public float m_followDepthScale;
    [Header("相机视锥宽度偏移")]
    public int m_widthOffset;

    private Camera zCamera;
    private List<Matrix4x4> listM4;
    private Matrix4x4 followOnceM4;
    private Matrix4x4 m4;
    private Vector3 m_zAngle;
    private FollowOnceObjectToB m_zSpaceData;
    private zSpace.Core.ZCore m_zCore;
    private Quaternion zspaceQuaternion = Quaternion.identity;
    private bool onceCome = false;//只进入一次
    #endregion

    #region 属性
    /// <summary>
    /// 深度相机视锥宽度偏移。
    /// </summary>
    public int WidthOffset
    {
        set { m_widthOffset = value; }
        get
        {
            return m_widthOffset;
        }
    } //get => m_widthOffset; set => m_widthOffset = value; }
    #endregion
    
    void Awake()
    {
        ////DontDestroyOnLoad(this);
        m_zSpaceData = FollowOnceObjectToB.DeSerializeNow();
        PlateformData.zspaceData = ReadCamPrjMatrixCacheData(m_zSpaceData);
        m_listCamera[0].depthTextureMode = DepthTextureMode.Depth;
    }


    
    
    void Start()
    {
        listM4 = new List<Matrix4x4>();
        for (int i = 0; i < m_listCamera.Count; i++)
        {
            listM4.Add(m_listCamera[i].projectionMatrix);
        }
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (!RecordZspaceData) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveCamPrjMatrixCacheData();
        }
#endif
    }
    /// <summar>y
    /// 获取已经校准好了的相机视角数据。
    /// </summary>
    /// <param name="data">要加载的数据。</param>
    public FollowOnceObject ReadCamPrjMatrixCacheData(FollowOnceObjectToB data)
    {
        FollowOnceObject fo = new FollowOnceObject();
        m4 = Matrix4x4.identity;
        m4.m00 = data.m00 * (1980f / m_widthOffset);
        m4.m01 = data.m01;
        m4.m02 = data.m02 * (1980f / m_widthOffset);
        m4.m03 = data.m03;
        m4.m10 = data.m10;
        m4.m11 = data.m11;
        m4.m12 = data.m12;
        m4.m13 = data.m13;
        m4.m20 = data.m20;
        m4.m21 = data.m21;
        m4.m22 = data.m22;
        m4.m23 = data.m23;
        m4.m30 = data.m30;
        m4.m31 = data.m31;
        m4.m32 = data.m32;
        m4.m33 = data.m33;
        followOnceM4 = m4;

        fo.farClip = data.farClip;
        fo.poisition= new Vector3(data.positionX, data.positionY, data.positionZ);
        fo.eulerAngles= new Vector3(data.eX, data.eY, data.eZ);
        fo.projectionMatrix = m4;
        
        this.transform.position = fo.poisition;
        this.transform.eulerAngles = fo.eulerAngles;

        if (m_depth != null)
        {
            m_depth.ReplaceDepthScale(m_followDepthScale);
        }

        for (int i = 0; i < m_listCamera.Count; i++)
        {
            m_listCamera[i].projectionMatrix = followOnceM4;
        }

        if (data.farClip > 0)
        {
            m_listCamera[1].farClipPlane = data.farClip;
        }

        this.gameObject.GetComponent<ExtendDisplay>().followDynamicCamera = false;
        return fo;
    }
#if UNITY_EDITOR
    /// <summary>
    /// 存储相机视角数据
    /// </summary>
    public void SaveCamPrjMatrixCacheData()
    {
        FollowOnceObjectToB b = new FollowOnceObjectToB();
        if (zCamera == null)
            zCamera = GameObject.Find("ScreenPointToRayCamera").GetComponent<Camera>();

        Camera current = zCamera;

        b.m00 = current.projectionMatrix.m00;
        b.m01 = current.projectionMatrix.m01;
        b.m02 = current.projectionMatrix.m02;
        b.m03 = current.projectionMatrix.m03;
        b.m10 = current.projectionMatrix.m10;
        b.m11 = current.projectionMatrix.m11;
        b.m12 = current.projectionMatrix.m12;
        b.m13 = current.projectionMatrix.m13;
        b.m20 = current.projectionMatrix.m20;
        b.m21 = current.projectionMatrix.m21;
        b.m22 = current.projectionMatrix.m22;
        b.m23 = current.projectionMatrix.m23;
        b.m30 = current.projectionMatrix.m30;
        b.m31 = current.projectionMatrix.m31;
        b.m32 = current.projectionMatrix.m32;
        b.m33 = current.projectionMatrix.m33;



        if (m_depth != null)
        {
            m_depth.ReplaceDepthScale(m_followDepthScale);
        }
        for (int i = 0; i < m_listCamera.Count; i++)
        {
            m_listCamera[i].transform.localPosition = Vector3.zero;
            m_listCamera[i].transform.localEulerAngles = Vector3.zero;
            m_listCamera[i].projectionMatrix = current.projectionMatrix;
        }
        gameObject.GetComponent<ExtendDisplay>().followDynamicCamera = false;

        b.positionX = current.transform.position.x;
        b.positionY = current.transform.position.y;
        b.positionZ = current.transform.position.z;

        b.eX = current.transform.eulerAngles.x;
        b.eY = current.transform.eulerAngles.y;
        b.eZ = current.transform.eulerAngles.z;

        b.farClip = m_farClip;

        FollowOnceObjectToB.SerializeNow(b);

    }
#endif
}
