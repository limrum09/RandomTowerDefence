Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;

            float4 _OutlineColor;
            float _OutlineSize;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                float alpha = color.a;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float outlineAlpha = 0;

                outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(offset.x, 0)).a;
                outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-offset.x, 0)).a;
                outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, offset.y)).a;
                outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -offset.y)).a;

                outlineAlpha = saturate(outlineAlpha);

                if (alpha <= 0 && outlineAlpha > 0)
                {
                    return _OutlineColor;
                }

                return color;
            }

            ENDHLSL
        }
    }
}