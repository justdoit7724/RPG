Shader "Custom/Transparency"
{
    Properties
    {
		 _Alpha("Alpha", Range(0,1)) = 0.5
		 _Color("Color", Color) = (0,0,0,0)
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

			uniform float _Alpha;
			uniform float4 _Color;

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
				float4 col = float4(_Color.xyz, _Alpha);

            return col;

            }
            ENDCG
        }
    }
}
