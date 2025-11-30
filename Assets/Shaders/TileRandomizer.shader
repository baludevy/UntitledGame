Shader "Custom/TileRandomizer"
{
    Properties
    {
        _TexA ("Texture A", 2D) = "white" {}
        _TexB ("Texture B", 2D) = "white" {}
        _TileSize ("Tile Size", Float) = 4
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows

        sampler2D _TexA;
        sampler2D _TexB;
        float _TileSize;
        fixed4 _Tint;

        float hash(float2 p)
        {
            p = frac(p * float2(0.3183099, 0.3678794));
            p += dot(p, p + 33.33);
            return frac(p.x * p.y);
        }

        struct Input
        {
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 uvFull = IN.worldPos.xz / _TileSize;
            
            float2 tileID = floor(uvFull); 
            
            float2 uv = uvFull - tileID; 

            float2 dx = ddx(uvFull);
            float2 dy = ddy(uvFull);

            fixed pick = step(0.5, hash(tileID));
            
            fixed4 colA = tex2Dgrad(_TexA, uv, dx, dy);
            fixed4 colB = tex2Dgrad(_TexB, uv, dx, dy);

            fixed4 c = lerp(colA, colB, pick) * _Tint;

            o.Albedo = c.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}