Shader "Custom/DepthShader"
{
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 pPos : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float3 wPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = v.uv;
                return o;
            }

			float4 frag(v2f i) : SV_Target
			{
				float4 pPos = mul(UNITY_MATRIX_VP, float4(i.wPos, 1));
				return float4(pPos.z / pPos.w,0,0,0);
            }
            ENDCG
        }
    }
}
