Shader "Custom/PostProcessing"
{
    Properties
    {
		[HideInInspector] _MainTex("MainTex", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Value("Value", Range(0,1)) = 0
		_WaveValue("WaveValue", Range(0,1))=0
		_WaveCenter("WaveCenter", Vector) = (0,0,0,0)
		_WaveHWidth("WaveHWidth", Range(0.001,0.2)) = 0.005
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
				float3 wPos : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _Noise;
			uniform float _Value;
			uniform float _WaveValue;
			uniform float4 _WaveCenter;
			uniform float _WaveHWidth;

			fixed4 frag(v2f i) : SV_Target
			{
				float noise = tex2D(_Noise, i.uv).x*0.9999f;
				float t = (cos((_Value + 1) * PI) + 1) * 0.5f;
				float noiseT = saturate(ceil(noise + t - 0.9999f));

				float3 col = 0;
				float2 UV = i.uv;

				// 1.-------------------------------------------------------
				// Apply Left-Right reverse / Noise
				UV -= 0.5f;
				UV.x *= 1 - 2 * noiseT; // 1 ~ -1
				UV += 0.5f;


				float waveRad = lerp(0, 0.7f, _WaveValue);
				float2 waveCenterSubVec = UV - _WaveCenter.xy;
				waveCenterSubVec.y *= 2280.0f/1080.0f; // screen ratio
				float2 waveCenterDir = normalize(waveCenterSubVec);

				float waveDist = length(waveCenterSubVec);
				float maskT = Range01(waveDist, waveRad - _WaveHWidth, waveRad + _WaveHWidth);
				float distT = maskT * abs(waveDist-waveRad)/ _WaveHWidth;
				float waveT = abs(sin(distT * PI)) + 1; // calculus of f(x) = (1 - cos(x * PI)) * 0.5f 
				
				float curveIntensity = (1-_WaveValue)*0.3f;

				// 2.----------------------------------------------------
				// Apply Wave
				UV -= _WaveCenter.xy;
				UV *= ((waveT - 1) * curveIntensity * maskT + 1);
				UV += _WaveCenter.xy;

				// 3.-----------------------------------------------------
				// Apply Left-Right reverse after Wave
				UV -= 0.5f;
				UV.x *= 1 - 2 * noiseT;
				UV += 0.5f;


				col = tex2D(_MainTex, UV).xyz * (1 - noiseT);
				
				// 4.------------------------------------------------------
				UV -= 0.5f;
				UV.x *= -1.0f;
				UV += 0.5f;

				float intervalT = pow((cos(_Time * 180.0f) + 1) * 0.5f, 7);
				float hInterval = lerp(0.01f, 0.0225f, intervalT) * 0.5f;
				col += noiseT * tex2D(_MainTex, UV + float2(-hInterval, 0));
				col += noiseT * tex2D(_MainTex, UV + float2(hInterval, 0));
				col += noiseT * tex2D(_MainTex, UV + float2(0, -hInterval));
				col += noiseT * tex2D(_MainTex, UV + float2(0, hInterval));
				col = col * (0.25f + 0.75f * (1.0f - noiseT));
				col = dot(col, float3(0.450, 0.86, 0.16)) * noiseT + col * (1 - noiseT);



				return float4(col,1);
			}
			ENDCG
		}
    }
}
