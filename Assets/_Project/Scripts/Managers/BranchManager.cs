using UnityEngine;

public class BranchManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("Properties")]
    [SerializeField] private IslandSlot[] _islandSlots;

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not BranchData branchData) return;

        branchData.islandSlots = _islandSlots != null ? (IslandSlot[])_islandSlots.Clone() : new IslandSlot[0];
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not BranchData branchData) return;
        SetData(branchData);
    }

    public void SetData(BranchData data)
    {
        _islandSlots = data.islandSlots != null ? (IslandSlot[])data.islandSlots.Clone() : new IslandSlot[0];
    }

    public void Clear()
    {
        _islandSlots = new IslandSlot[0];

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("BranchManager cleared to initial state");
    }

    public IslandSlot[] IslandSlots => _islandSlots;
}
