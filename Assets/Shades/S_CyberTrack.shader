Shader "NeonRush/CyberTrack"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.015, 0.018, 0.025, 1)
        _EmissionColor ("Emission Color", Color) = (0.08, 0.42, 0.9, 1)
        _GridDensity ("Grid Density", Range(2, 80)) = 18
        _LaneWidth ("Lane Width", Range(0.001, 0.25)) = 0.018
        _FlowSpeed ("Flow Speed", Range(-10, 10)) = 2.2
        _EmissionPower ("Emission Power", Range(0, 8)) = 0.9
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "CyberTrack"
            ZWrite On
            Cull Back

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
                half4 _BaseColor;
                half4 _EmissionColor;
                half _GridDensity;
                half _LaneWidth;
                half _FlowSpeed;
                half _EmissionPower;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half LineMask(half value, half width)
            {
                half distanceToLine = abs(frac(value) - 0.5);
                return 1.0 - smoothstep(width, width + 0.015, distanceToLine);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half time = _Time.y * _FlowSpeed;
                half2 uv = input.uv;

                half laneA = 1.0 - smoothstep(0.012, 0.026, abs(uv.x - 0.22));
                half laneB = 1.0 - smoothstep(0.012, 0.026, abs(uv.x - 0.5));
                half laneC = 1.0 - smoothstep(0.012, 0.026, abs(uv.x - 0.78));
                half grid = LineMask((uv.y + time) * _GridDensity, 0.012);
                half flow = smoothstep(0.94, 1.0, frac((uv.y + time * 0.65) * 7.0));
                half edgeGlow = smoothstep(0.45, 0.5, abs(uv.x - 0.5));
                half panelShade = smoothstep(0.08, 0.35, abs(uv.x - 0.5));

                half laneMask = saturate(laneA + laneB * 0.6 + laneC);
                half neonMask = saturate(laneMask * 0.9 + grid * 0.16 + flow * 0.45 + edgeGlow * 0.35);
                half3 baseColor = _BaseColor.rgb * (0.65 + panelShade * 0.28);
                half3 emission = _EmissionColor.rgb * neonMask * _EmissionPower;

                return half4(baseColor + emission, 1);
            }
            ENDHLSL
        }
    }
}
