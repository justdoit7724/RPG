Shader "Custom/RangeIndicatorShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,0)
		_Center("Center", Vector) = (0,0,0,0)
		[HideInInspector] _ViewDir("ViewDir", Vector) =(1,0,0,0)
		 _ViewAngle("ViewAngle", Range(0,360)) = 360
		 _MaxRad("MaxRad", Range(0,25)) = 1
		 _Value("Value", Range(0,1)) = 0
    }


    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite Off
		ZTest LEqual


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
				float3 wPos:TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

			uniform float4 _Color;
			uniform float4 _Center;
			uniform float _MaxRad;
			uniform float _Value;
			uniform float4 _ViewDir;
			uniform float _ViewAngle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				i.wPos.y = 0;
			_Center.y = 0;

			float outShape = Circle(i.wPos, _Center.xyz, _MaxRad) * 0.3f;
			float inShape = Circle(i.wPos, _Center.xyz, _MaxRad* saturate(_Value)) * 0.2f;

			float3 pixelDir = normalize(i.wPos - _Center.xyz);
			float offset = 0.1f;
			float viewShape = saturate(ceil((_ViewAngle*0.5f)-AngleBetweenDir(pixelDir, _ViewDir)));

			float totalShape = (outShape + inShape) * viewShape;
			clip(totalShape);

			fixed4 col = float4(_Color.xyz, totalShape);

            return col;

            }
            ENDCG
        }
    }
}
