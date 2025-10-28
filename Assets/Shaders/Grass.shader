Shader "Custom/GrassShaderIndirect"
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
        Tags
        {
            "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True"
        }
        LOD 150
        Cull Off
        Offset 0, 1

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _TopColor;
            float4 _BottomColor;
            float _WindStrength;
            float _WindSpeed;
            float _Cutoff;
            float4 _LightColor0;

            StructuredBuffer<float4> _Positions;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;

                float4 inst = _Positions[instanceID];
                float3 basePosWS = inst.xyz;
                float rotRad = radians(inst.w);
                float s = sin(rotRad);
                float c = cos(rotRad);

                float3 lp = v.vertex.xyz;
                
                float3 rp;
                rp.x = lp.x * c + lp.z * s;
                rp.y = lp.y;
                rp.z = -lp.x * s + lp.z * c;
                
                float wind = sin((basePosWS.x + basePosWS.z) + _Time.y * _WindSpeed) * _WindStrength * lp.y;
                rp.x += wind * 0.1;
                rp.z += wind * 0.05;
                
                float4 world = float4(basePosWS + rp, 1);
                
                o.pos = mul(UNITY_MATRIX_VP, world);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.height = saturate(lp.y);

                UNITY_TRANSFER_FOG(o, o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                clip(tex.a - _Cutoff);

                fixed4 grad = lerp(_BottomColor, _TopColor, i.height);
                
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = saturate(L.y);

                float shadowAtten = SHADOW_ATTENUATION(i);
                float atten = lerp(1.0, shadowAtten, _LightShadowData.x);

                float3 lighting = UNITY_LIGHTMODEL_AMBIENT.rgb + (_LightColor0.rgb * ndotl * atten);

                fixed4 col = tex * grad;
                col.rgb *= lighting;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }

    FallBack Off
}