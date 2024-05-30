using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor _matEditor;
    private Object[] _mats;
    private MaterialProperty[] _matProperties;
    
    private bool _showPresets;
    public bool Clipping {
        set => SetProperty("_Clipping", "_CLIPPING", value);
    }

    public bool PremultiplyAlpha {
        set => SetProperty("_PreMultipyAlphaTog", "_PREMULTIPLY_ALPHA", value);
    }

    public BlendMode SrcBlend {
        set => SetProperty("_SrcBlend", (float)value);
    }

    public BlendMode DstBlend {
        set => SetProperty("_DstBlend", (float)value);
    }

    public bool ZWrite {
        set => SetProperty("_ZWrite", value ? 1f : 0f);
    }
    
    public RenderQueue RenderQueue {
        set {
            foreach (Material m in this._mats) {
                m.renderQueue = (int)value;
            }
        }
    }
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);

        this._matEditor = materialEditor;
        this._mats = materialEditor.targets;
        this._matProperties = properties;
        
        EditorGUILayout.Space();
        this._showPresets = EditorGUILayout.Foldout(_showPresets, "Presets", true);
        if (this._showPresets)
        {
            OpaquePreset();
            ClipPreset();
            FadePreset();
            TransparentPreset();
        }
    }
    
    private bool HasProperty (string name) =>
        FindProperty(name, this._matProperties, false) != null;

    private bool SetProperty(string name, float value)
    {
        MaterialProperty property = FindProperty(name, this._matProperties, false);
        if (property != null)
        {
            property.floatValue = value;
            return true;
        }

        return false;
    }
    
    private void SetProperty(string name, string keyWord, bool value)
    {
        if (SetProperty(name, value ? 1f : 0f))
        {
            SetKeyword(keyWord, value);
        }
    }

    private void SetKeyword(string keyWord, bool enable)
    {
        foreach (Material mat in this._mats)
        {
            if (enable)
            {
                mat.EnableKeyword(keyWord);
            }
            else
            {
                mat.DisableKeyword(keyWord);
            }
        }
    }
    
    private bool PresetButton(string name) {
        if (GUILayout.Button(name)) {
            this._matEditor.RegisterPropertyChangeUndo(name);
            return true;
        }
        return false;
    }
    
    
    
    void OpaquePreset() {
        if (PresetButton("Opaque")) {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.Geometry;
        }
    }
    
    void ClipPreset () {
        if (PresetButton("Clip")) {
            Clipping = true;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.AlphaTest;
        }
    }
    
    void FadePreset () {
        if (PresetButton("Fade")) {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.SrcAlpha;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
    
    private bool HasPremultiplyAlpha => HasProperty("_PreMultipyAlphaTog");
    void TransparentPreset () {
        if (HasPremultiplyAlpha && PresetButton("Transparent")) {
            Clipping = false;
            PremultiplyAlpha = true;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
}
