// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ExtendDisplay/SideBySideStereo"
{
    Properties
    {
        _LeftTex ("Left Source", 2D) = "" {}
        _RightTex ("Right Source", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    sampler2D _LeftTex;
    sampler2D _RightTex;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        // Expand the u-coordinate of the UV coordinates of either the left
        // half of the render buffer or the right half of the render buffer to
        // take up the full 0.0 to 1.0 range (rather than just 0.0 to 0.5 or
        // 0.5 to 1.0).  These modified UV coordinates are then used to sample
        // either the left texture or the right texture.  This has the effect
        // of horizontally compressing the left texture into the left side of
        // the render buffer and the right texture into the right side of the
        // render buffer.

        float uOffset = i.uv.x <= 0.5 ? 0.0 : 0.5;
        float2 modifiedUv = float2((i.uv.x - uOffset) * 2.0, i.uv.y);

        half4 resultColor;

        if (i.uv.x <= 0.5)
        {
            resultColor = tex2D(_LeftTex, modifiedUv);
        }
        else
        {
            resultColor = tex2D(_RightTex, modifiedUv);
        }

        return resultColor;
    }

    ENDCG

    Subshader
    {
        ZTest Always
        Cull Off
        ZWrite Off

        Fog
        {
            Mode off
        }

        Pass
        {
            CGPROGRAM

            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
    }

    Fallback off
}