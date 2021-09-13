using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
public class KotlinParsingView : MonoBehaviour, IInputOutputProvider
{
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private TMP_InputField _output;

    [FormerlySerializedAs("_printAstButton")] 
    [SerializeField] private Button _printObjectButton;
    
    [FormerlySerializedAs("_printCsButton")] 
    [SerializeField] private Button _printProxyButton;

    private KotlinParsingController _controller;

    public TMP_InputField InputField => _input;
    public TMP_InputField OutputField => _output;

    private void Awake()
    {
        _controller = new KotlinParsingController(this);
        _printObjectButton.onClick.AddListener(_controller.PrintObject);
        _printProxyButton.onClick.AddListener(_controller.PrintProxy);
    }

    private void OnDestroy()
    {
        _printProxyButton.onClick.RemoveAllListeners();
        _printObjectButton.onClick.RemoveAllListeners();
    }
}
