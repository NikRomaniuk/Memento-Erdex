using UnityEngine;
using Evo.UI;

public class CustomQuantitySelector : MonoBehaviour
{
    // --- References ---
    [SerializeField] private QuantitySelector _quantitySelector;

    // --- Configuration ---
    [SerializeField] private Reference_Int _minQuantity;
    [SerializeField] private Reference_Int _maxQuantity;
    [SerializeField] private Reference_Int _startQuantity;
    [SerializeField] private Observable_Int _quantityObservable;

    // --- Public Accessors ---
    public QuantitySelector QuantitySelector => _quantitySelector;

    void Start()
    {
        // Configure range
        _quantitySelector.minQuantity = _minQuantity;
        _quantitySelector.maxQuantity = _maxQuantity;
        _quantitySelector.SetQuantity(_startQuantity);

        _quantitySelector.onValueChanged.AddListener(quantity =>
        {
            _quantityObservable.Value = quantity;
        });
    }
}
