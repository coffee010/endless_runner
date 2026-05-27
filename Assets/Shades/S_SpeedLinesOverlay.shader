Shader "NeonRush/SpeedLinesOverlay"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.35, 0.8, 1, 1)
        _Intensity ("Intensity", Range(0, 1)) = 0
        _LineDensity ("Line Density", Range(8, 80)) = 34
        _FlowSpeed ("Flow Speed", Range(0, 20)) = 8
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent+100"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "SpeedLinesOverlay"
            Blend SrcAlpha One
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

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

            CBUFFER_START(UnityPerMaterial)
                half4 _TintColor;
                half _Intensity;
                half _LineDensity;
                half _FlowSpeed;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half2 centered = input.uv - 0.5;
                half edge = smoothstep(0.32, 0.68, abs(centered.x)) + smoothstep(0.34, 0.58, abs(centered.y));
                edge = saturate(edge);

                half angle = atan2(centered.y, centered.x) * 0.15915494;
                half radial = length(centered);
                half movingLines = smoothstep(0.84, 1.0, frac(angle * _LineDensity + radial * 12.0 - _Time.y * _FlowSpeed));
                half streak = smoothstep(0.36, 0.78, radial) * movingLines * edge;

                half alpha = streak * _Intensity * 0.45;
                return half4(_TintColor.rgb * 0.45, alpha);
            }
            ENDHLSL
        }
    }
}
