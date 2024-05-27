using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    private partial void DrawUnsupportedShaders();
    private partial void DrawGizmos();
    private partial void PrepareForSceneWindow();
    private partial void PrepareBuffer();
    
#if UNITY_EDITOR
    
    private static ShaderTagId[] _legacyShaderTagIds =
    {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };

    private static Material _errorMat;

    public string SampleName { get; set; }

    private partial void DrawUnsupportedShaders()
    {
        if (_errorMat == null)
        {
            _errorMat = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }

        var drawingSetting = new DrawingSettings(_legacyShaderTagIds[0], new SortingSettings(this._camera))
            { overrideMaterial = _errorMat };
        for (int i = 1; i < _legacyShaderTagIds.Length; i++)
        {
            drawingSetting.SetShaderPassName(i, _legacyShaderTagIds[i]);
        }

        var filteringSetting = FilteringSettings.defaultValue;
        this._context.DrawRenderers(this._cullingResults, ref drawingSetting, ref filteringSetting);
    }

    private partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos()) {
            this._context.DrawGizmos(this._camera, GizmoSubset.PreImageEffects);
            this._context.DrawGizmos(this._camera, GizmoSubset.PostImageEffects);
        }
    }

    private partial void PrepareForSceneWindow()
    {
        if (this._camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(this._camera);
        }
    }

    private partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        this._buffer.name = SampleName = this._camera.name;
        Profiler.EndSample();
    }
    
#else
    private const string SampleName = bufferName;
#endif
}
