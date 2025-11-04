Shader "Hidden/VolumetricFogPost"
{
    Properties
    {
        _Color("Fog Color", Color) = (0.7,0.8,0.9,1)
        _Density("Density", Range(0,1)) = 0.05
        _Steps("Steps", Int) = 32
        _StartDistance("Start Distance", Range(0,100)) = 10
        _FadeLength("Fade Length", Range(0,500)) = 50
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Opaque" }
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4x4 _CameraInverseProjection;
            float4x4 _CameraInverseView;
            float4 _Color;
            float _Density;
            int _Steps;
            float _StartDistance;
            float _FadeLength;

            float3 ReconstructViewDir(float2 uv)
            {
                float4 ndc = float4(uv * 2 - 1, 0, 1);
                float4 view = mul(_CameraInverseProjection, ndc);
                view /= view.w;
                float3 dir = normalize(view.xyz);
                return mul((float3x3)_CameraInverseView, dir);
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float depth = LinearEyeDepth(rawDepth);
                float3 rayDir = ReconstructViewDir(i.uv);
                float maxDist = depth;
                float stepSize = maxDist / _Steps;

                float fogStartFade = smoothstep(_StartDistance, _StartDistance + _FadeLength, depth);
                float fogAccum = 0;

                for (int s = 0; s < _Steps; s++)
                {
                    float dist = s * stepSize;
                    float densitySample = exp(-dist * _Density);
                    fogAccum += (1 - densitySample) * (_Density * 0.1);
                }

                float fogFactor = saturate(fogAccum * fogStartFade);
                float4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(_Color.rgb, col.rgb, 1 - fogFactor);
                return col;
            }
            ENDCG
        }
    }
}
