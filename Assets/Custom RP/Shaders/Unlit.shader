Shader "Custom RP/Unlit"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1.0,1.0,1.0,1.0)
        
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Source Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("destination Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
        
        [Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            
            #include "UnlitPass.hlsl"
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment

            #pragma multi_compile_instancing

            #pragma shader_feature _CLIPPING
            
            ENDHLSL
        }
    }
}
