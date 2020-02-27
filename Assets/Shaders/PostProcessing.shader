Shader "Custom/PostProcessing"
{
    Properties
    {
		[HideInInspector] _MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Value("Value", Range(0,1)) = 0
		_Interval("Interval", Range(0.001, 0.2)) = 0.01
    }
    SubShader
    {
			Pass
		{
		Cull Off ZWrite Off ZTest Always

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
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _Noise;
			uniform float _Value;
			uniform float _Interval;

			fixed4 frag(v2f i) : SV_Target
			{
				float noise = tex2D(_Noise, i.uv).x;
				float t = (cos((_Value + 1) * PI) + 1) * 0.5f;
				float noiseT = noise + t;
				float finalT = saturate(ceil(noiseT - 0.999f));

				float3 col = 0;

				//쉐이더에서 조건문 최대한 자제 (최적화)
				//if(finalT==0)
				//{
					col = tex2D(_MainTex, i.uv).xyz * (1 - finalT);
				//}
				//else
				//{
					float hInterval = _Interval * 0.5f;

					float2 mUV = i.uv;
					mUV -= 0.5f;
					mUV.x *= -1.0f;
					mUV += 0.5f;

					col += finalT*tex2D(_MainTex, mUV + float2(-hInterval, 0));
					col += finalT*tex2D(_MainTex, mUV + float2(hInterval, 0));
					col += finalT*tex2D(_MainTex, mUV + float2(0, -hInterval));
					col += finalT*tex2D(_MainTex, mUV + float2(0, hInterval));

					col = col * (0.25f+0.75f*(1.0f-finalT));
					col = dot(col, float3(0.450, 0.86, 0.16)) * finalT + col*(1- finalT);
				//}

				return float4(col,1);
			}
			ENDCG
		}
    }
}
