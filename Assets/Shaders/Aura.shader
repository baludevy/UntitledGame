Shader "Custom/AuraSphere"
{
    Properties
    {
        _AuraColor("Aura Color", Color) = (0.3, 0.7, 1.0, 0.6)
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _Thickness("Outline Thickness", Float) = 0.15
        _Center("Center", Vector) = (0,0,0,0)
        _Intensity("Tint Intensity", Range(0,2)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        GrabPass { "_GrabTex" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTex;
            float4 _GrabTex_TexelSize;

            float4 _AuraColor;
            float4 _OutlineColor;
            float _Thickness;
            float4 _Center;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                fixed4 sceneCol = tex2D(_GrabTex, uv);

                float dist = distance(i.worldPos, _Center.xyz);
                float rim = smoothstep(0.0, _Thickness, abs(_Thickness - dist));
                float inside = saturate(1 - rim);

                // tint the scene color if inside sphere
                float3 tinted = lerp(sceneCol.rgb, _AuraColor.rgb, inside * _Intensity);

                // add a bright outline
                float outline = smoothstep(0.0, _Thickness * 2, abs(dist - _Thickness));
                float3 col = lerp(tinted, _OutlineColor.rgb, outline);

                float alpha = saturate(inside * _AuraColor.a + outline * 0.5);

                return float4(col, alpha);
            }
            ENDCG
        }
    }
}
