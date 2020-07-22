using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultPlateformEvent
{
    public static Action<PlatformType, PlatformType> SwitchPlateform;
    public static Action OpenZView;
    public static Action OpenLy3d;
}
public class MultPlateformData
{
    public static GameObject CurrObject;//当前物体
    public static StylusState StylusFunctionState = StylusState.Idle;
    public static float RotationSpeed;
    public static Camera CurrentCamera;
}