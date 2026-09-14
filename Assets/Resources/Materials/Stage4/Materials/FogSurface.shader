Shader "Custom/FogSurface"
{
    Properties
    {
        _Color ("Fog Color", Color) = (0.75, 0.78, 0.8, 1)
        _Opacity ("Opacity", Range(0,1)) = 0.85
        _NoiseTex ("Noise", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 2
        _NoiseSpeed ("Noise Speed", Vector) = (0.01, 0.01, 0, 0)
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            fixed4 _Color;
            float _Opacity;
            float _NoiseScale;
            float4 _NoiseSpeed;
            float _NoiseStrength;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * _NoiseScale;
                uv += _Time.y * _NoiseSpeed.xy;

                float noise = tex2D(_NoiseTex, uv).r;

                float alpha = _Opacity;
                alpha *= lerp(1.0, noise, _NoiseStrength);

                return fixed4(_Color.rgb, alpha);
            }

            ENDCG
        }
    }
}