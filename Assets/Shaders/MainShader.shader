
Shader "Custom/MainShader"
{
	Properties
	{
		_HitFlash("HitFlash", Range(0,1))=0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Color03("Color03", Color) = (0,0,0,0)
		_Color02("Color02", Color) = (0,0,0,0)
		_Color01("Color01", Color) = (0,0,0,0)
		_PolyArtAlbedo("PolyArtAlbedo", 2D) = "white" {}
		_PolyArtMask("PolyArtMask", 2D) = "white" {}
		_Depth("Depth", 2D) = "black"{}
		_Emission("Emission", Range(0,1))=0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		Lighting Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		#include "CustomShaderFunctions.cginc"
		struct Input
		{
			float2 uv_texcoord;
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

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PolyArtAlbedo = i.uv_texcoord * _PolyArtAlbedo_ST.xy + _PolyArtAlbedo_ST.zw;
			float4 tex2DNode16 = tex2D( _PolyArtAlbedo, uv_PolyArtAlbedo );
			float2 uv_PolyArtMask = i.uv_texcoord * _PolyArtMask_ST.xy + _PolyArtMask_ST.zw;
			float4 tex2DNode13 = tex2D( _PolyArtMask, uv_PolyArtMask );
			float4 temp_cast_0 = (tex2DNode13.r).xxxx;
			float4 temp_cast_1 = (tex2DNode13.g).xxxx;
			float4 temp_cast_2 = (tex2DNode13.b).xxxx;
			float4 blendOpSrc22 = tex2DNode16;
			float4 blendOpDest22 = ( min( temp_cast_0 , _Color01 ) + min( temp_cast_1 , _Color02 ) + min( temp_cast_2 , _Color03 ) );
			float4 lerpResult4 = lerp( tex2DNode16 , ( ( saturate( ( blendOpSrc22 * blendOpDest22 ) )) * 2.0 ) , ( tex2DNode13.r + tex2DNode13.g + tex2DNode13.b ));
			o.Albedo = lerpResult4.rgb;
			o.Emission = Lerp(0,lerpResult4.rgb, _Emission) + _HitFlash.xxx;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}