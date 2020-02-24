Shader "Custom/TouchIndicator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_StartPt("StartPt", Vector) = (0,0,0,0)
		_EndPt("EndPt", Vector) = (0,0,0,0)
		_StartRad("StartRad", Range(20,100)) = 30
		_EndRad("EndRad", Range(10,60)) = 20
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Lighting Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "CustomShaderFunctions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 wPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			uniform float4 _StartPt;
			uniform float4 _EndPt;
			uniform float _StartRad;
			uniform float _EndRad;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			float3 subLineVec = _EndPt - _StartPt;
			float t = saturate(dot(subLineVec, i.wPos - _StartPt) / dot(subLineVec, subLineVec));
			float3 closestPt = Lerp(_StartPt, _EndPt, t);

			float rad = lerp(_StartRad, _EndRad, t);
			float tempShape = Circle(i.wPos, closestPt, rad);
			clip(tempShape);
			float radT = length(i.wPos - closestPt) / rad;
			float mRadT = (cos((radT * 1.8 + 1) * PI) + 1) / 2;
			float alpha = mRadT * tempShape;
			return float4(1,1,1, alpha);
            }
            ENDCG
        }
    }
}
