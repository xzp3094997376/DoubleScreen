// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

Shader "zSpace/zView/CompositorRGB" 
{
    Properties 
    {
        _MainTex("", 2D) = "" {}
    }

    CGINCLUDE

        #include "UnityCG.cginc"

        struct v2f 
        {
            float4 pos : POSITION;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;
        sampler2D _MaskDepthTexture;
        sampler2D _NonEnvironmentDepthTexture;
        float4    _MaskColor;

        v2f vert(appdata_img v) 
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = v.texcoord.xy;
            return o;
        }

        float4 depthMask(v2f pixelData) : COLOR0
        {
            float maskDepth = DecodeFloatRGBA(tex2D(_MaskDepthTexture, pixelData.uv));
            float nonEnvironmentDepth = DecodeFloatRGBA(tex2D(_NonEnvironmentDepthTexture, pixelData.uv));
            float4 color = tex2D(_MainTex, pixelData.uv);

            if (nonEnvironmentDepth < maskDepth || maskDepth > 0.999)
            {
                color.a = 1.0;
                return color;
            }
            else
            {
                return _MaskColor;
            }
        }

    ENDCG 

    Subshader 
    {
        Pass 
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }

            CGPROGRAM
            
                #pragma glsl
                #pragma fragmentoption ARB_precision_hint_fastest 
                #pragma vertex vert
                #pragma fragment depthMask
                #pragma target 3.0

            ENDCG
        }
    }

    Fallback off
}