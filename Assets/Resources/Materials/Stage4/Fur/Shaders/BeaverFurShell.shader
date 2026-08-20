Shader "Beaver/Fur Shell"
{
    Properties
    {
        _MainTex ("Beaver Texture", 2D) = "white" {}
        _FurColor ("Fur Color", Color) = (0.55, 0.32, 0.15, 1)

        _FurLength ("Fur Length", Range(0.0, 0.05)) = 0.008
        _FurDensity ("Fur Density", Range(0.0, 1.0)) = 0.65
        _FurLayers ("Fur Layers", Range(1, 8)) = 5

        _TipTransparency ("Tip Transparency", Range(0.0, 1.0)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma target 4.0
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _FurColor;
            float _FurLength;
            float _FurDensity;
            float _FurLayers;
            float _TipTransparency;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float alpha : TEXCOORD1;
            };

            v2g vert(appdata v)
            {
                v2g o;

                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            float Random(float3 p)
            {
                return frac(sin(dot(p, float3(
                    12.9898,
                    78.233,
                    37.719
                ))) * 43758.5453);
            }

            [maxvertexcount(24)]
            void geom(
                triangle v2g input[3],
                inout TriangleStream<g2f> output
            )
            {
                float layers = max(1.0, _FurLayers);

                for (int layer = 0; layer < 8; layer++)
                {
                    if (layer >= layers)
                        break;

                    float t = (layer + 1.0) / layers;

                    float3 p0 = input[0].vertex.xyz;
                    float3 p1 = input[1].vertex.xyz;
                    float3 p2 = input[2].vertex.xyz;

                    float3 n0 = normalize(input[0].normal);
                    float3 n1 = normalize(input[1].normal);
                    float3 n2 = normalize(input[2].normal);

                    float offset = _FurLength * t;

                    float3 v0 = p0 + n0 * offset;
                    float3 v1 = p1 + n1 * offset;
                    float3 v2 = p2 + n2 * offset;

                    g2f o;

                    o.vertex = UnityObjectToClipPos(float4(v0, 1));
                    o.uv = input[0].uv;

                    float r0 = Random(p0 * 100.0 + layer);

                    o.alpha =
                        _FurDensity *
                        (1.0 - t * _TipTransparency) *
                        (0.65 + r0 * 0.35);

                    output.Append(o);

                    o.vertex = UnityObjectToClipPos(float4(v1, 1));
                    o.uv = input[1].uv;

                    float r1 = Random(p1 * 100.0 + layer);

                    o.alpha =
                        _FurDensity *
                        (1.0 - t * _TipTransparency) *
                        (0.65 + r1 * 0.35);

                    output.Append(o);

                    o.vertex = UnityObjectToClipPos(float4(v2, 1));
                    o.uv = input[2].uv;

                    float r2 = Random(p2 * 100.0 + layer);

                    o.alpha =
                        _FurDensity *
                        (1.0 - t * _TipTransparency) *
                        (0.65 + r2 * 0.35);

                    output.Append(o);

                    output.RestartStrip();
                }
            }

            fixed4 frag(g2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float3 color = tex.rgb * _FurColor.rgb;

                return fixed4(color, i.alpha);
            }

            ENDCG
        }
    }
}