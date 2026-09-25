Shader "Custom/HoleMaterialShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Radius("Radius", float) = 0.5
        _InRadius("Inner Radius", float) = 0.2
        _OutRadius("Outer Radius", float) = 0.2
        _Color("Color", Color) = (1, 1, 1, 1)
        _CenterColor("Center Color", Color) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
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
                float4 positionOS : POSITION_OS;
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _Radius;
                float _InRadius;
                float _OutRadius;
                half4 _Color;
                half4 _CenterColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionOS = IN.positionOS;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                half4 color = _Color;
                float radialDistance = distance(TransformObjectToWorld(IN.positionOS), TransformObjectToWorld(float4(0,0,0,0)));
                if (radialDistance > _Radius + _OutRadius) {
                    color[3] = 0;
                } else if (radialDistance >= _Radius) {
                    color[3] = clamp(lerp(color[3], 0, (radialDistance - _Radius) / _OutRadius), 0, 1);
                } else {
                    color[3] = clamp(lerp(color[3], 0, (_Radius - radialDistance) / _InRadius), 0, 1);
                    color[0] = _CenterColor[0] * _CenterColor[3] * (1 - color[3]) + color[0] * color[3];
                    color[1] = _CenterColor[1] * _CenterColor[3] * (1 - color[3]) + color[1] * color[3];
                    color[2] = _CenterColor[2] * _CenterColor[3] * (1 - color[3]) + color[2] * color[3];
                    color[3] = max(color[3], _CenterColor[3]);
                    // color = lerp(_CenterColor, color, radialDistance / _Radius);
                }
                return color;
            }
            ENDHLSL
        }
    }
}
