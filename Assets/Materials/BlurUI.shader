Shader "Custom/BlurUI"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0.0, 100.0)) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float _BlurSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Apply blur
                fixed4 blurredColor = fixed4(0, 0, 0, 0);
                float blurAmount = _BlurSize / _ScreenParams.y;
                float2 blurDir = float2(blurAmount, 0);

                for (int j = -4; j <= 4; j++)
                {
                    blurredColor += tex2D(_MainTex, i.uv + j * blurDir);
                }

                blurredColor /= 9.0;

                // Preserve original alpha
                blurredColor.a = col.a;

                return blurredColor;
            }

            ENDCG
        }
    }
}
