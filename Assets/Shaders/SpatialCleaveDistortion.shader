Shader "AshenRequiem/Spatial Cleave Distortion"
{
    Properties
    {
        _SliceAngle ("Slice Angle", Float) = 30
        _SliceCenter ("Slice Center", Vector) = (0.5, 0.5, 0, 0)
        _SliceOffset ("Slice Offset", Vector) = (0, 0, 0, 0)
        [HDR] _EdgeGlowColor ("Edge Glow", Color) = (0, 0, 0, 1)
        _EffectStrength ("Effect Strength", Range(0, 1)) = 0
        _FractureWidth ("Fracture Width", Range(0.001, 0.03)) = 0.005
        _ChromaticAberration ("Chromatic Aberration", Range(0, 2)) = 0.8
        _Jitter ("Jitter", Vector) = (0, 0, 0, 0)
        _SliceSeed ("Slice Seed", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "SpatialCleave"

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _SliceAngle;
            float4 _SliceCenter;
            float4 _SliceOffset;
            half4 _EdgeGlowColor;
            float _EffectStrength;
            float _FractureWidth;
            float _ChromaticAberration;
            float4 _Jitter;
            float _SliceSeed;

            float Hash11(float value)
            {
                return frac(sin(value * 127.1 + _SliceSeed * 311.7) * 43758.5453);
            }

            float JaggedFault(float alongLine)
            {
                float cell = floor(alongLine * 38.0);
                float local = frac(alongLine * 38.0);
                float a = Hash11(cell);
                float b = Hash11(cell + 1.0);
                float coarse = lerp(a, b, smoothstep(0.0, 1.0, local)) - 0.5;
                float fine = sin(alongLine * 173.0 + _SliceSeed * 9.0) * 0.22;
                return coarse + fine;
            }

            half4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 centered = uv - _SliceCenter.xy;
                float aspect = _BlitTexture_TexelSize.z / max(_BlitTexture_TexelSize.w, 1.0);
                centered.x *= aspect;

                float angleRadians = radians(_SliceAngle);
                float2 tangent = float2(cos(angleRadians), sin(angleRadians));
                float2 normal = float2(-tangent.y, tangent.x);
                float2 normalUV = normalize(float2(normal.x / max(aspect, 0.0001), normal.y));
                float alongLine = dot(centered, tangent);
                float signedDistance = dot(centered, normal);

                float jag = JaggedFault(alongLine);
                float faultDistance = signedDistance + jag * _FractureWidth * 0.72;
                float side = faultDistance >= 0.0 ? 1.0 : -1.0;
                float distanceToFault = abs(faultDistance);
                float strength = saturate(_EffectStrength);

                // The two halves of the frame slide in opposite directions.
                float2 screenOffset = float2(_SliceOffset.x / max(aspect, 0.0001), _SliceOffset.y);
                float2 halfShift = screenOffset * side;

                // A tight refraction field bends details into the tear instead of only moving the frame.
                float refractionFalloff = exp(-distanceToFault * 42.0);
                float refractionWave = sin(alongLine * 92.0 + _SliceSeed * 5.0) * 0.5 + 0.5;
                float2 refraction = normalUV * side * refractionFalloff * refractionWave * 0.010 * strength;

                // Thin parallel fault bands create displaced fragments around the main rupture.
                float bandA = 1.0 - smoothstep(0.0, _FractureWidth * 1.7,
                    abs(distanceToFault - _FractureWidth * 3.2));
                float bandB = 1.0 - smoothstep(0.0, _FractureWidth * 1.25,
                    abs(distanceToFault - _FractureWidth * 6.4));
                float segmented = step(0.36, Hash11(floor((alongLine + 2.0) * 17.0)));
                float2 fragmentShift = tangent * (bandA - bandB) * segmented * 0.012 * side * strength;

                float2 sampleUV = saturate(uv + halfShift + refraction + fragmentShift + _Jitter.xy * refractionFalloff);

                float edgeInfluence = exp(-distanceToFault * 70.0) * strength;
                float2 chromaOffset = normalUV * _ChromaticAberration * 0.004 * edgeInfluence;
                half4 centerSample = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, sampleUV);
                half red = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, saturate(sampleUV + chromaOffset)).r;
                half blue = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, saturate(sampleUV - chromaOffset)).b;
                half3 color = half3(red, centerSample.g, blue);

                float pixelWidth = max(fwidth(faultDistance), 1.0 / max(_BlitTexture_TexelSize.w, 1.0));
                float core = 1.0 - smoothstep(pixelWidth * 0.5, max(_FractureWidth * 0.72, pixelWidth * 1.5), distanceToFault);
                float rim = (1.0 - smoothstep(_FractureWidth * 0.7, _FractureWidth * 2.8, distanceToFault)) - core;
                float ghostRim = saturate(bandA * 0.45 + bandB * 0.22) * segmented;
                float bladeLine = 1.0 - smoothstep(pixelWidth * 0.2, max(_FractureWidth * 0.34, pixelWidth * 1.1), distanceToFault);
                float bladePulse = 0.82 + sin(alongLine * 150.0 + _SliceSeed * 4.0) * 0.18;

                half3 voidColor = half3(0.002, 0.006, 0.018);
                color = lerp(color, voidColor, saturate(core * strength * 1.15));
                color += _EdgeGlowColor.rgb * (rim + ghostRim) * strength;
                half3 bladeColor = lerp(_EdgeGlowColor.rgb, half3(8.0, 9.0, 12.0), 0.72);
                color += bladeColor * bladeLine * bladePulse * strength;

                return half4(color, centerSample.a);
            }
            ENDHLSL
        }
    }
}
