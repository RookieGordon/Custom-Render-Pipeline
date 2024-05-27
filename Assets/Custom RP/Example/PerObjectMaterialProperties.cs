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
    [FormerlySerializedAs("cutoff")] [SerializeField, Range(0f, 1f)]
    public float cutOff = 0.5f;

    public void Awake()
    {
        OnValidate();
    }

    public void OnValidate () {
        if (_block == null) {
            _block = new MaterialPropertyBlock();
        }
        
        _block.SetColor(_baseColorId, BaseColor);
        _block.SetFloat(_cutOffId, cutOff);
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
