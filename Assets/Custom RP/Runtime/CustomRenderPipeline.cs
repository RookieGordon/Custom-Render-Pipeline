using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer _renderer = new CameraRenderer();

    bool useDynamicBatching, useGPUInstancing;
    public CustomRenderPipeline(bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher)
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
    }
    
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            _renderer.Render(context, camera,this.useDynamicBatching, this.useGPUInstancing);
        }
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        base.Render(context, cameras);
    }
}
