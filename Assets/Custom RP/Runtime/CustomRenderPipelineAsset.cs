using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    public bool UseDynamicBatching = true, UseGPUInstancing = true, UseSRPBatcher = true;
    
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(UseDynamicBatching, UseGPUInstancing, UseSRPBatcher);
    }
}
