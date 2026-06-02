Shader "NeonRush/HologramURP"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.08, 0.75, 1, 0.58)
        _EmissionColor ("Emission Color", Color) = (0.16, 1.4, 2.8, 1)
        _Alpha ("Alpha", Range(0, 1)) = 0.42
        _LineDensity ("Line Density", Range(4, 140)) = 52
        _LineStrength ("Line Strength", Range(0, 2)) = 0.78
        _ScanSpeed ("Scan Speed", Range(-10, 10)) = 1.7
        _ScanBandSize ("Scan Band Size", Range(0.01, 0.45)) = 0.12
        _FresnelPower ("Fresnel Power", Range(0.4, 8)) = 2.1
        _FresnelStrength ("Fresnel Strength", Range(0, 5)) = 1.85
        _GlitchStrength ("Glitch Strength", Range(0, 1)) = 0.22
        _GlitchFrequency ("Glitch Frequency", Range(1, 80)) = 23
        _ProjectionFade ("Projection Fade", Range(0, 1)) = 0.2
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
            Name "Hologram"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

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
                float3 positionOS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissionColor;
                half _Alpha;
                half _LineDensity;
                half _LineStrength;
                half _ScanSpeed;
                half _ScanBandSize;
                half _FresnelPower;
                half _FresnelStrength;
                half _GlitchStrength;
                half _GlitchFrequency;
                half _ProjectionFade;
                half _Pulse;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 positionOS = input.positionOS.xyz;
                half time = _Time.y * _ScanSpeed;
                half slice = step(0.96, frac((positionOS.y + time * 0.25) * _GlitchFrequency));
                half offset = (frac(positionOS.y * 19.73 + time * 3.11) - 0.5) * _GlitchStrength * 0.06 * slice;
                positionOS.x += offset;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                output.positionHCS = positionInputs.positionCS;
                output.normalWS = normalize(normalInputs.normalWS);
                output.viewDirWS = normalize(GetWorldSpaceViewDir(positionInputs.positionWS));
                output.uv = input.uv;
                output.positionOS = positionOS;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half time = _Time.y * _ScanSpeed;
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);

                half horizontal = smoothstep(0.68, 1.0, frac((input.positionOS.y + time) * _LineDensity));
                half scanBand = 1.0 - smoothstep(0.0, _ScanBandSize, abs(frac(input.positionOS.y * 0.32 - time * 0.18) - 0.5));
                half glitchCut = step(0.94, frac((input.positionOS.y + time * 0.35) * _GlitchFrequency));
                half fresnel = pow(1.0 - saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS))), _FresnelPower);
                half verticalFade = lerp(1.0, saturate(input.uv.y + 0.18), _ProjectionFade);

                half lineEnergy = horizontal * _LineStrength + scanBand * 0.8;
                half3 baseColor = tex.rgb * _BaseColor.rgb * 0.28;
                half3 glow = _EmissionColor.rgb * (fresnel * _FresnelStrength + lineEnergy * 0.55 + 0.12 + _Pulse * 0.08);
                half alpha = _Alpha * tex.a * verticalFade;
                alpha *= saturate(0.42 + horizontal * 0.32 + scanBand * 0.55 + fresnel * 0.45);
                alpha *= lerp(1.0, 0.28, glitchCut * _GlitchStrength);

                return half4(baseColor + glow, saturate(alpha));
            }
            ENDHLSL
        }
    }
}
