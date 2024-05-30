using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PerObjectMaterialProperties : MonoBehaviour
{

    private static MaterialPropertyBlock _block;

    private static int _baseColorId = Shader.PropertyToID("_BaseColor");
    [SerializeField] 
    public Color BaseColor = Color.white;
    
    private static int _cutOffId = Shader.PropertyToID("_Cutoff");
    [SerializeField, Range(0f, 1f)]
    public float CutOff = 0.5f;
    
    private static int _metallicId = Shader.PropertyToID("_Metallic");
    [SerializeField, Range(0f, 1f)]
    public float Metallic = 0.0f;
    private static int _smoothness = Shader.PropertyToID("_Smoothness");
    [SerializeField, Range(0f, 1f)]
    public float Smoothness = 0.5f;

    public void Awake()
    {
        OnValidate();
    }

    public void OnValidate () {
        if (_block == null) {
            _block = new MaterialPropertyBlock();
        }
        
        _block.SetColor(_baseColorId, BaseColor);
        _block.SetFloat(_cutOffId, CutOff);
        _block.SetFloat(_metallicId, Metallic);
        _block.SetFloat(_smoothness, Smoothness);
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
