Shader "Custom/Shockwave"
{
    Properties
    {
        _BottomColor("Bottom Color", Color) = (0,0,1,1)
        _TopColor("Top Color", Color) = (1,1,1,1)
        _GradientHeight("Gradient Height", Float) = 5
        _Center("Center", Vector) = (0,0,0,0)
        _Radius("Radius", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BottomColor;
            float4 _TopColor;
            float _GradientHeight;
            float4 _Center;
            float _Radius;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = i.worldPos - _Center.xyz;
                float dist = length(dir);
                float fade = 1 - saturate(dist / _Radius);
                float gradient = saturate((i.worldPos.y - _Center.y) / _GradientHeight);
                float4 gradColor = lerp(_BottomColor, _TopColor, gradient);
                gradColor.a *= fade;
                return gradColor;
            }
            ENDCG
        }
    }
}
