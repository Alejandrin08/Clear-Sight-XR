Shader "Meta/PCA/ShaderFalseColor_Jet" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float luminance(float3 color) {
                return dot(color, float3(0.299, 0.587, 0.114));
            }

            float4 jet(float x) {
                x = saturate(x);
                float r = saturate(1.5 - abs(4.0 * (x - 0.75)));
                float g = saturate(1.5 - abs(4.0 * (x - 0.5)));
                float b = saturate(1.5 - abs(4.0 * (x - 0.25)));
                return float4(r, g, b, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target {
                float3 original = tex2D(_MainTex, i.uv).rgb;
                float lum = luminance(original);
                return jet(lum);
            }
            ENDCG
        }
    }
}
