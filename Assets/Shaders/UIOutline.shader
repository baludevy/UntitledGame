Shader "Custom/UIOutline"
{
    Properties
    {
        [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float alpha = col.a;

                if (alpha <= 0.001)
                {
                    float2 offset = _MainTex_TexelSize.xy * _OutlineThickness;
                    float neighborAlpha = 0;
                    neighborAlpha += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                    neighborAlpha += tex2D(_MainTex, i.uv - float2(offset.x, 0)).a;
                    neighborAlpha += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                    neighborAlpha += tex2D(_MainTex, i.uv - float2(0, offset.y)).a;

                    if (neighborAlpha > 0)
                        return _OutlineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
