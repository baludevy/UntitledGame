Shader "Custom/TriplanarLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Tiling ("Tiling", Float) = 1
        _BlendSharpness ("Blend Sharpness", Range(1,10)) = 4
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Back
        ZWrite On
        Lighting On

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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            float _Tiling;
            float _BlendSharpness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldLightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = worldPos.xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            float4 SampleTriplanar(float3 worldPos, float3 normal)
            {
                normal = normalize(normal);
                float3 blend = pow(abs(normal), _BlendSharpness);
                blend /= (blend.x + blend.y + blend.z);

                float2 xUV = worldPos.zy * _Tiling;
                float2 yUV = worldPos.xz * _Tiling;
                float2 zUV = worldPos.xy * _Tiling;

                xUV = xUV * _MainTex_ST.xy + _MainTex_ST.zw;
                yUV = yUV * _MainTex_ST.xy + _MainTex_ST.zw;
                zUV = zUV * _MainTex_ST.xy + _MainTex_ST.zw;

                float4 xTex = tex2D(_MainTex, xUV);
                float4 yTex = tex2D(_MainTex, yUV);
                float4 zTex = tex2D(_MainTex, zUV);

                return xTex * blend.x + yTex * blend.y + zTex * blend.z;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = SampleTriplanar(i.worldPos, i.worldNormal) * _Tint;

                float3 N = normalize(i.worldNormal);
                float3 L = normalize(i.worldLightDir);
                float NdotL = max(0, dot(N, L));

                fixed shadow = LIGHT_ATTENUATION(i);
                fixed3 litColor = baseCol.rgb * (_LightColor0.rgb * NdotL * shadow + UNITY_LIGHTMODEL_AMBIENT.rgb);
                return fixed4(litColor, baseCol.a);
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ForwardAdd"
            }
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragAdd
            #pragma multi_compile_fwdadd
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            float _Tiling;
            float _BlendSharpness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 lightDir : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = worldPos.xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.lightDir = normalize(_WorldSpaceLightPos0.xyz);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            float4 SampleTriplanar(float3 worldPos, float3 normal)
            {
                normal = normalize(normal);
                float3 blend = pow(abs(normal), _BlendSharpness);
                blend /= (blend.x + blend.y + blend.z);

                float2 xUV = worldPos.zy * _Tiling;
                float2 yUV = worldPos.xz * _Tiling;
                float2 zUV = worldPos.xy * _Tiling;

                xUV = xUV * _MainTex_ST.xy + _MainTex_ST.zw;
                yUV = yUV * _MainTex_ST.xy + _MainTex_ST.zw;
                zUV = zUV * _MainTex_ST.xy + _MainTex_ST.zw;

                float4 xTex = tex2D(_MainTex, xUV);
                float4 yTex = tex2D(_MainTex, yUV);
                float4 zTex = tex2D(_MainTex, zUV);

                return xTex * blend.x + yTex * blend.y + zTex * blend.z;
            }

            fixed4 fragAdd(v2f i) : SV_Target
            {
                fixed4 baseCol = SampleTriplanar(i.worldPos, i.worldNormal) * _Tint;
                float3 N = normalize(i.worldNormal);
                float3 L = normalize(i.lightDir);
                float NdotL = max(0, dot(N, L));
                fixed shadow = LIGHT_ATTENUATION(i);
                fixed3 col = baseCol.rgb * _LightColor0.rgb * NdotL * shadow;
                return fixed4(col, 0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}