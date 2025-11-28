Shader "Custom/PixelAuraGlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.5)
        _MainTex ("Texture", 2D) = "white" {}
        _GlowPower ("Edge Softness/Glow", Range(1, 5)) = 2.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 100

        // Setup Alpha Blending
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GlowPower;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed3 finalRGB = _Color.rgb;

                // Combine texture alpha with material alpha
                float alpha = texColor.a * _Color.a;

                // Use power function to soften edges and create glow look
                // Higher GlowPower means a stronger, wider soft transition
                alpha = pow(alpha, 1.0 / _GlowPower);

                return fixed4(finalRGB, alpha);
            }
            ENDCG
        }
    }
}