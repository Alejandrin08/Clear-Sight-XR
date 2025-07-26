Shader "Custom/ShaderFalseColor_Cividis"
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

            float3 cividis(float value)
            {
                float3 cividis = lerp(
                    lerp(float3(0.0, 0.32, 0.62), float3(0.86, 0.86, 0.56), value),
                    float3(0.90, 0.90, 0.20),
                    pow(value, 3)
                );
                return cividis;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float grayscale = tex2D(_MainTex, i.uv).r;
                return float4(cividis(grayscale), 1.0);
            }
            ENDCG
        }
    }
}
