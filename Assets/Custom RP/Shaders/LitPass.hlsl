#ifndef CUSTOM_UNLIT_PASS_INCLUDED
#define CUSTOM_UNLIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Surface.hlsl"
#include "../ShaderLibrary/Light.hlsl"
#include "../ShaderLibrary/BRDF.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)

    UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)

    UNITY_DEFINE_INSTANCED_PROP(float, _Metallic)
    UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct Attributes
{
    float3 posOS: POSITION;
    float3 normalOS: NORMAL;
    float2 baseUV: TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varings
{
    float4 posCS: SV_POSITION;
    float3 posWS: VAR_POSITION;
    float3 normalWS: VAR_NORMAL;
    float2 baseUV: VAR_BASE_UV;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

Varings LitPassVertex (Attributes IN)
{
    Varings OUT = (Varings)0;
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    
    float3 posWS = TransformObjectToWorld(IN.posOS);
    OUT.posWS = posWS;
    OUT.posCS = TransformWorldToHClip(posWS);

    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
    OUT.baseUV = IN.baseUV * baseST.xy + baseST.zw;
    
    return OUT;
}

float4 LitPassFragment(Varings IN): SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(IN);
    
    float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.baseUV);
    float4 base = baseColor * baseMap;
    
    #ifdef _CLIPPING
        clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
    #endif

    Surface surface = (Surface)0;
    surface.normal = SafeNormalize(IN.normalWS);
    surface.viewDirection = SafeNormalize(_WorldSpaceCameraPos - IN.posWS);
    surface.color = base.rgb;
    surface.alpha = base.a;
    surface.metallic = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Metallic);
    surface.smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness);

    #ifdef _PREMULTIPLY_ALPHA
        BRDF brdf = GetBRDF(surface, true);
    #else
        BRDF brdf = GetBRDF(surface);
    #endif
    float3 color = GetLighting(surface, brdf);
    return float4(color, surface.alpha);
}

#endif