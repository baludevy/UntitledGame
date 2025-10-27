Shader "Custom/GrassShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopColor ("Top Color", Color) = (0.2, 0.8, 0.2, 1)
        _BottomColor ("Bottom Color", Color) = (0.1, 0.5, 0.1, 1)
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.2
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
        LOD 100
        
        Cull Off
        
        Offset 0, 1

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _TopColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BottomColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _WindStrength)
                UNITY_DEFINE_INSTANCED_PROP(float, _WindSpeed)
                UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                float windStrength = UNITY_ACCESS_INSTANCED_PROP(Props, _WindStrength);
                float windSpeed = UNITY_ACCESS_INSTANCED_PROP(Props, _WindSpeed);
                
                float3 pos = v.vertex.xyz;
                float wind = sin((pos.x + pos.z + _Time.y * windSpeed)) * windStrength * v.vertex.y;
                pos.x += wind * 0.1;
                pos.z += wind * 0.05;
                
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.height = saturate(v.vertex.y);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float2 paddedUV = i.uv * 0.98 + 0.01;
                fixed4 texColor = tex2D(_MainTex, paddedUV);
                
                float cutoff = UNITY_ACCESS_INSTANCED_PROP(Props, _Cutoff) + 0.1;
                clip(texColor.a - cutoff);
                
                float4 topColor = UNITY_ACCESS_INSTANCED_PROP(Props, _TopColor);
                float4 bottomColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BottomColor);
                fixed4 gradient = lerp(bottomColor, topColor, i.height);
                fixed4 col = texColor * gradient;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}