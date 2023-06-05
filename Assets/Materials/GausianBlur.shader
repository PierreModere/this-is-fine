Shader "Custom/BlurGaussian" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 10.0)) = 1.0
    }
 
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float _BlurSize;
 
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target {
                fixed4 color = tex2D(_MainTex, i.uv);
                fixed4 blurredColor = fixed4(0, 0, 0, 0);
                float blurSize = _BlurSize;
                float2 texelSize = 1.0 / _ScreenParams.xy;
 
                // Calculer le flou gaussien en utilisant un kernel
                for (float x = -blurSize; x <= blurSize; x += 1.0) {
                    for (float y = -blurSize; y <= blurSize; y += 1.0) {
                        float2 offset = float2(x, y) * texelSize;
                        blurredColor += tex2D(_MainTex, i.uv + offset);
                    }
                }
 
                blurredColor /= ((2 * blurSize + 1) * (2 * blurSize + 1));
                return blurredColor;
            }
            ENDCG
        }
    }
}