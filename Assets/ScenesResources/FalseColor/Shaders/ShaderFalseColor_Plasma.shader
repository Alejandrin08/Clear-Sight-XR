Shader "Custom/ShaderFalseColor_Plasma"
{
    Properties
    {
        _MainTex ("Passthrough", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 plasma(float value)
            {
                float3 c = float3(
                    sin(3.0 * value),
                    sin(2.0 * value + 0.5),
                    sin(1.0 * value + 2.0)
                );
                return saturate(c);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float grayscale = tex2D(_MainTex, i.uv).r;
                return float4(plasma(grayscale), 1.0);
            }
            ENDCG
        }
    }
}
