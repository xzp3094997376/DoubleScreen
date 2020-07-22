// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthShader" {
	Properties {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
		_Cutoff ("Alpha cutoff", Range (0,1)) = 0.9
		}
	SubShader{
			//Queue设置为AlphaTest，就能取到所有的深度，包括透明物体的深度
		Tags{"Queue"="AlphaTest" "IgnoreProjector"="True"  "RenderType"="Transparent" }
		ZTest On
		
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend SrcColor OneMinusSrcColor
		//AlphaTest Greater [_Cutoff]
		//ZWrite off 
		//ZWrite On
		//Cull Off //Cull Off or Cull Front is OK.But Cull Back cannot get target renderTexture

  //      Pass{
		//	ZWrite On
		//	ColorMask 0
		//}
		Pass{
		Tags { "LightMode"="ForwardBase"}
		CGPROGRAM 
		#pragma vertex vert
		#pragma fragment frag
		//#pragma multi_compile_fwdbase
		#include "UnityCG.cginc"
		sampler2D _MainTex; 
        fixed _Cutoff;
        float4 _MainTex_ST;

		//该值是起到平移深度的作用
		uniform float _DepthShaderCameraNear;

		//该值能使得物体的深度成比例缩放。比如该值为0.5时，本来在Near处深度为0，在Far处深度为1，
		//除以这个值之后，在Near处为0，在(Far-Near)/2到Far处深度都为1.
		uniform float _DepthShaderCameraFar;
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			//float4 scrPos:TEXCOORD0;
			float lengthInCamera :TEXCOORD1;
			float2 uv:TEXCOORD2;
		};

		//Vertex Shader  逐顶点
		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			//o.scrPos =  ComputeScreenPos(o.pos);
			//o.lengthInCamera = 1-length(o.pos)/100; //这是采用摄像距离的 增强数值越小出屏越好 但是由于采用顶点来得到深度 很大的物体又顶点过少(如超级巨大的盒子)的就需要把这个数值调大
			
			float4 mvPos = mul(UNITY_MATRIX_MV, v.vertex);
			o.lengthInCamera = 1 - (_DepthShaderCameraNear - mvPos.z)/ _DepthShaderCameraFar; //Near是摄像机Z轴最近裁剪面距离 Far是最远裁剪面距离 自然Z缓冲
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			//for some reason, the y position of the depth texture comes out inverted
			//o.scrPos.y = 1 - o.scrPos.y;
			return o;
		}

		//Fragment Shader  逐像素
		half4 frag(v2f i) : SV_TARGET0{
			//float depthValue = 1 - Linear01Depth(tex2D(_CameraDepthTexture, (i.scrPos)).r);
			////float depthValue = (tex2D(_CameraDepthTexture, i.uv).r)/(50-0.3);
			half4 colorMT = tex2D(_MainTex, UNITY_PROJ_COORD(i.uv));
			
			float depthValue = i.lengthInCamera;
			half4 depth;

			depth.a = colorMT.a;
			//clip(depth.a - _Cutoff); //发布出 裁剪不成功 蛋疼 
			//不能使用clip函数，因为在AplhaTest下，clip函数发布出来不成功。
			if(colorMT.a >_Cutoff) //模拟裁剪 要不就是不透 要不就是全透
			{
			    depth.a = 1;
			}
			else
			{
				depth.a = 0;
			}
			depth.r = depthValue;
			depth.g = depthValue;
			depth.b = depthValue;
			return depth;
		}
		ENDCG
		}
		
	}
		FallBack "Diffuse"
}