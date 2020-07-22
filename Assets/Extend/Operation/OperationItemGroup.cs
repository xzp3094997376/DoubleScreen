using UnityEngine;
using System.Collections;
/// <summary>
///操作子对象做离散、聚和 状态控制
///聚合状态 整体拖拽、缩放，旋转
///离散状态 子对象单体拖拽、缩放，旋转
/// </summary>
public class OperationItemGroup : MonoBehaviour
{
    [Tooltip("当前对象所有子物体是否散开")]
    /// <summary>
    /// 子物体散开、合并
    /// </summary>
    [SerializeField,HideInInspector]
    private bool _diffuse;
    /// <summary>
    /// 设置聚合=false、分散=true
    /// </summary>
    public bool _Diffuse
    {
        get {
            return _diffuse;
        }
        set {
            _diffuse = value;
        }
    }         
}
