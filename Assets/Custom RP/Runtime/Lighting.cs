using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string bufferName = "Lighting";
    
    private CommandBuffer _buffer = new CommandBuffer() { name = bufferName };

    private const int MaxDirLightCounts = 4;
    private static int
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");
    private static Vector4[]
        dirLightColors = new Vector4[MaxDirLightCounts],
        dirLightDirections = new Vector4[MaxDirLightCounts];

    private CullingResults _cullingResults;
    
    public void Steup(ScriptableRenderContext context, CullingResults results)
    {
        this._cullingResults = results;
        this._buffer.BeginSample(bufferName);
        SetupLights();
        this._buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(this._buffer);
        this._buffer.Clear();
    }

    private void SetupDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }

    private void SetupLights()
    {
        var visibleLights = this._cullingResults.visibleLights;
        int lightCount = 0;
        for (int i = 0; i < visibleLights.Length; i++)
        {
            var visibleLight = visibleLights[i];
            if (visibleLight.lightType != LightType.Directional)
            {
                continue;
            }
            
            SetupDirectionalLight(lightCount++, ref visibleLight);
            if (lightCount >= MaxDirLightCounts)
            {
                break;
            }

        }
        
        this._buffer.SetGlobalInt(dirLightCountId, visibleLights.Length);
        this._buffer.SetGlobalVectorArray(dirLightColorsId, dirLightColors);
        this._buffer.SetGlobalVectorArray(dirLightDirectionsId, dirLightDirections);
    }
}
