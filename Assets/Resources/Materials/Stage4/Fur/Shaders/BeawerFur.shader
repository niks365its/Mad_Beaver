Shader "Beaver/Fur Simple"
{
    Properties
    {
        _MainTex ("Beaver Texture", 2D) = "white" {}
        _FurColor ("Fur Color", Color) = (0.55, 0.32, 0.15, 1)
        _FurAmount ("Fur Amount", Range(0, 1)) = 0.35
        _FurDark ("Fur Shadow", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        LOD 200

        // =========================
        // ОСНОВНЕ ВІДОБРАЖЕННЯ
        // =========================

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _FurColor;
            float _FurAmount;
            float _FurDark;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float light = saturate(dot(normal, lightDir));

                float fur = lerp(1.0, light, _FurAmount);

                float3 finalColor =
                    tex.rgb *
                    lerp(
                        1.0,
                        _FurColor.rgb,
                        _FurDark * _FurAmount
                    );

                finalColor *= lerp(0.75, 1.15, fur);

                return fixed4(finalColor, tex.a);
            }

            ENDCG
        }

        // =========================
        // SHADOW CASTER
        // =========================

        Pass
        {
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            CGPROGRAM

            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vertShadow(appdata v)
            {
                v2f o;

                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

                return o;
            }

            float4 fragShadow(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }

    FallBack "Diffuse"
}