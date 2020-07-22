// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

Shader "zSpace/zView/DepthRender" 
{
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        Pass 
        {
            Fog { Mode Off }
        
            CGPROGRAM
        
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct v2f 
                {
                    float4 pos : SV_POSITION;
                    float2 depth : TEXCOORD0;
                };

                float _Log2FarPlusOne;

                v2f vert(appdata_base v) 
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.depth.x = -mul(UNITY_MATRIX_MV, v.vertex).z;
                    o.depth.y = 0;
                    return o;
                }

                float4 frag(v2f i) : COLOR 
                {
                    return EncodeFloatRGBA(clamp(log2(i.depth.x + 1) / _Log2FarPlusOne, 0, 0.999999));
                }
            
            ENDCG
        }
    }
}