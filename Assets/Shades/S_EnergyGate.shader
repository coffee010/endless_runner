Shader "NeonRush/EnergyGate"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1, 0.45, 1, 1)
        _EmissionColor ("Emission Color", Color) = (0.1, 0.45, 1, 1)
        _Alpha ("Alpha", Range(0, 1)) = 0.26
        _LineDensity ("Line Density", Range(4, 60)) = 22
        _ScanSpeed ("Scan Speed", Range(-8, 8)) = 1.8
        _FresnelPower ("Edge Power", Range(0.5, 8)) = 2.2
        _Pulse ("Pulse", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "EnergyGate"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissionColor;
                half _Alpha;
                half _LineDensity;
                half _ScanSpeed;
                half _FresnelPower;
                half _Pulse;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = positionInputs.positionCS;
                output.normalWS = normalize(normalInputs.normalWS);
                output.viewDirWS = normalize(GetWorldSpaceViewDir(positionInputs.positionWS));
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half time = _Time.y * _ScanSpeed;
                half horizontalLines = smoothstep(0.86, 1.0, frac((input.uv.y + time) * _LineDensity));
                half diagonalLines = smoothstep(0.9, 1.0, frac((input.uv.x + input.uv.y * 0.65 - time * 0.35) * 12.0));
                half scanBand = 1.0 - smoothstep(0.0, 0.11, abs(frac(input.uv.y - time * 0.22) - 0.5));
                half edge = pow(1.0 - saturate(abs(input.uv.x - 0.5) * 2.0), 0.35);
                half fresnel = pow(1.0 - saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS))), _FresnelPower);

                half lineMask = saturate(horizontalLines * 0.45 + diagonalLines * 0.28 + scanBand * 0.55);
                half energy = saturate(0.08 + lineMask + fresnel * 0.45 + (1.0 - edge) * 0.32);
                half3 color = _BaseColor.rgb * 0.05 + _EmissionColor.rgb * energy * (0.45 + _Pulse * 0.12);
                half alpha = saturate(_Alpha * (0.22 + lineMask * 0.42 + fresnel * 0.22 + (1.0 - edge) * 0.18));

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
