#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
    UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
    float3 posOS: POSITION;
    float2 baseUV: TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varings
{
    float4 posCS: SV_POSITION;
    float2 baseUV: VAR_BASE_UV;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varings UnlitPassVertex (Attributes IN)
{
    Varings OUT = (Varings)0;
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    
    float3 posWS = TransformObjectToWorld(IN.posOS);
    OUT.posCS = TransformWorldToHClip(posWS);

    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
    OUT.baseUV = IN.baseUV * baseST.xy + baseST.zw;
    
    return OUT;
}

float4 UnlitPassFragment(Varings IN): SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN);
    
    float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.baseUV);
    float4 base = baseColor * baseMap;
    
    #ifdef _CLIPPING
        clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
    #endif
    
    return base;
}

#endif