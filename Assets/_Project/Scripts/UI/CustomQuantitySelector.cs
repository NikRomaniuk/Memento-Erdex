using UnityEngine;
using Evo.UI;
using TMPro;

public class CustomQuantitySelector : MonoBehaviour
{
    // --- References ---
    [SerializeField] private QuantitySelector _quantitySelector;
    [SerializeField] private TMP_InputField _quantityInputField;

    // --- Configuration ---
    [SerializeField] private Reference_Int _minQuantity;
    [SerializeField] private Reference_Int _maxQuantity;
    [SerializeField] private Reference_Int _startQuantity;
    [SerializeField] private Observable_Int _quantityObservable;

    // --- Public Accessors ---
    public QuantitySelector QuantitySelector => _quantitySelector;

    // --- Debug ---
    [SerializeField] private bool _debug = false;
    private string _lastDebug;

    void Start()
    {
        // Configure range
        _quantitySelector.minQuantity = _minQuantity;
        _quantitySelector.maxQuantity = _maxQuantity;
        _quantitySelector.SetQuantity(_startQuantity);

        _quantitySelector.onValueChanged.AddListener(quantity =>
        {
            _quantityObservable.Value = quantity;
            D($"Quantity changed to {quantity}");
        });

        _quantityInputField.onEndEdit.AddListener(quantityStr =>
        {
            if (int.TryParse(quantityStr, out int quantity))
            {
                if (quantity < _minQuantity) quantity = _minQuantity;
                if (quantity > _maxQuantity) quantity = _maxQuantity;
                _quantityObservable.Value = quantity;
                D($"Quantity changed to {quantity}");
            }
        });
    }

    // ====
    // Debug
    // ====

    private void D(string message)
    {
        if (!_debug) { return; }
        if (_lastDebug == message) { return; }

        _lastDebug = message;
        Debug.Log($"[Quantity Selector] {message}", this);
    }
}
