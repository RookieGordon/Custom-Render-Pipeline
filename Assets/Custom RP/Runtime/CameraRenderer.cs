using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private ScriptableRenderContext _context;

    private Camera _camera;

    private const string bufferName = "Render Camera";

    private CommandBuffer _buffer = new CommandBuffer() { name = bufferName };

    private CullingResults _cullingResults;

    private static ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    public void Render(ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing)
    {
        this._context = context;
        this._camera = camera;
        PrepareBuffer();
        PrepareForSceneWindow();
        if (!Cull())
        {
            return;
        }

        Setup();
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing);
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private void Setup()
    {
        this._context.SetupCameraProperties(this._camera);
        CameraClearFlags flags = this._camera.clearFlags;
        this._buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth,
#if  UNITY_2022_1_OR_NEWER
                                                flags <= CameraClearFlags.Color
#else
                                        flags == CameraClearFlags.Color, 
#endif
                                                flags == CameraClearFlags.Color ? this._camera.backgroundColor.linear : Color.clear);
        this._buffer.BeginSample(SampleName);
        ExecuteBuffer();
    }

    private void DrawVisibleGeometry(bool useDynamicBatching, bool useGPUInstancing)
    {
        var sortingSetting = new SortingSettings(this._camera) { criteria = SortingCriteria.CommonOpaque };
        var drawingSetting = new DrawingSettings(_unlitShaderTagId, sortingSetting)
        {
            enableInstancing = useGPUInstancing,
            enableDynamicBatching = useDynamicBatching,
        };
        var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);
        this._context.DrawRenderers(this._cullingResults, ref drawingSetting, ref filteringSetting);

        this._context.DrawSkybox(this._camera);

        sortingSetting.criteria = SortingCriteria.CommonTransparent;
        drawingSetting.sortingSettings = sortingSetting;
        filteringSetting.renderQueueRange = RenderQueueRange.transparent;
        this._context.DrawRenderers(this._cullingResults, ref drawingSetting, ref filteringSetting);
    }

    private void Submit()
    {
        this._buffer.EndSample(SampleName);
        ExecuteBuffer();
        this._context.Submit();
    }

    private void ExecuteBuffer()
    {
        this._context.ExecuteCommandBuffer(this._buffer);
        this._buffer.Clear();
    }

    private bool Cull()
    {
        if (this._camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            this._cullingResults = this._context.Cull(ref p);
            return true;
        }

        return false;
    }
}