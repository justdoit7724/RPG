﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CustomMainShader"
{
    Properties
    {
		_HitFlash("HitFlash", Range(0,1)) = 0
		_Color03("Color03", Color) = (0,0,0,0)
		_Color02("Color02", Color) = (0,0,0,0)
		_Color01("Color01", Color) = (0,0,0,0)
		_PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
		_PolyArtMask("PolyArtMask", 2D) = "white" {}
		_Depth("Depth", 2D) = "black"{}
		_Emission("Emission", Range(0,1)) = 0
		_LightDir("LightDir", Vector) = (1,0,0,0)
    }
    SubShader
    {
		Tags{"RenderType" = "Opaque"  "Queue" = "Geometry+0" }
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "CustomShaderFunctions.cginc"
			#include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 lDiffuse : TEXCOORD1;
				float3 lSpec : TEXCOORD2;
				float2 pUV : TEXCOORD3;
            };


			uniform sampler2D _PolyArtAlbedo;
			uniform float4 _PolyArtAlbedo_ST;
			uniform sampler2D _PolyArtMask;
			uniform float4 _PolyArtMask_ST;
			uniform sampler2D _Depth;
			uniform float4 _Color01;
			uniform float4 _Color02;
			uniform float4 _Color03;
			uniform float _HitFlash;
			uniform float _Metallic;
			uniform float _Smoothness;
			uniform float _Emission;
			uniform float3 _LightDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				float3 wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				//투영 텍스쳐 계산
				float4 pPos = mul(UNITY_MATRIX_VP, float4(wPos,1));
				o.pUV = pPos.xy / pPos.w;
				o.pUV.y *= -1;

				//로컬 노멀 -> 월드 노멀
				float3 wNormal = mul((float3x3)transpose(unity_WorldToObject), v.normal);

				//로우 폴리 특성을 살려서, 버텍스 쉐이더에서 빛 연산 진행
				float3 lookDir = wPos - _WorldSpaceCameraPos;
				ComputeDirectionalLight(_LightDir, wNormal, -lookDir, o.lDiffuse, o.lSpec);

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
			float2 uv_PolyArtAlbedo = i.uv * _PolyArtAlbedo_ST.xy + _PolyArtAlbedo_ST.zw;
			float4 tex2DNode16 = tex2D(_PolyArtAlbedo, uv_PolyArtAlbedo);
			float2 uv_PolyArtMask = i.uv * _PolyArtMask_ST.xy + _PolyArtMask_ST.zw;
			float4 tex2DNode13 = tex2D(_PolyArtMask, uv_PolyArtMask);
			float4 temp_cast_0 = (tex2DNode13.r).xxxx;
			float4 temp_cast_1 = (tex2DNode13.g).xxxx;
			float4 temp_cast_2 = (tex2DNode13.b).xxxx;
			float4 blendOpSrc22 = tex2DNode16;
			float4 blendOpDest22 = (min(temp_cast_0 , _Color01) + min(temp_cast_1 , _Color02) + min(temp_cast_2 , _Color03));
			float4 lerpResult4 = lerp(tex2DNode16 , ((saturate((blendOpSrc22 * blendOpDest22))) * 2.0) , (tex2DNode13.r + tex2DNode13.g + tex2DNode13.b));
			float3 Albedo = lerpResult4.rgb;
			float3 Ambient = Lerp(0,lerpResult4.rgb, _Emission) + _HitFlash.xxx;
			Albedo *= i.lDiffuse;

			float curDepth = tex2D(_Depth, i.pUV);

			return float4(i.pUV,0, 1);
			return float4(Albedo + i.lSpec + Ambient, 1);
            }
            ENDCG
        }
    }
}
