using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTreeConstants", menuName = "Constants/Tree")]
public class TreeConstants : ScriptableObject
{
    [SerializeField, InfoBox("@MaxTreeHeight.ToString()"),
    ValidateInput(nameof(ValidateMaxTreeHeight), "Max must be >= Min")]
    private Observable_Int MaxTreeHeight;

    [SerializeField, InfoBox("@MinTreeHeight.ToString()"),
    ValidateInput(nameof(ValidateMinTreeHeight), "Min must be <= Max")]
    private Observable_Int MinTreeHeight;

    // ================
    // Getters & Setters
    // ================

    // --- Tree Height ---
    // Max
    public int GetMaxTreeHeight() { return MaxTreeHeight.Value; }
    // Min
    public int GetMinTreeHeight() { return MinTreeHeight.Value; }

    // ==============
    // Odin Validation
    // ==============
    private bool ValidateMaxTreeHeight(Observable_Int obs) => obs.Value >= MinTreeHeight.Value;
    private bool ValidateMinTreeHeight(Observable_Int obs) => obs.Value <= MaxTreeHeight.Value;
}
