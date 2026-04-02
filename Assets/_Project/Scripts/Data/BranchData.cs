using UnityEngine;

[System.Serializable]
public struct IslandSlot
{
    public bool isStatic;
    // Local X position of island slot center along branch.
    [Step(0.05f)] public float xPoint;

    // Used when isStatic == True
    public IslandData staticIslandData;

    // Used when isStatic == False
    public bool allowTiny;
    public bool allowSmall;
    public bool allowMedium;

    public IslandSlot(bool isStatic, float xPoint, IslandData staticIslandData, bool allowTiny, bool allowSmall, bool allowMedium)
    {
        this.isStatic = isStatic;
        this.xPoint = xPoint;
        this.staticIslandData = staticIslandData;
        this.allowTiny = allowTiny;
        this.allowSmall = allowSmall;
        this.allowMedium = allowMedium;
    }
}

[CreateAssetMenu(fileName = "NewBranchData", menuName = "Entries/BranchData")]
public class BranchData : ScriptableObject, IData
{
    string IData.id => id;

    public enum AvailableSide
    {
        Both,
        Left,
        Right,
        Middle
    }

    [Header("Main Configuration")]
    // Unique identifier
    public string id;
    public AvailableSide avaliableSide = AvailableSide.Both;
    public int length;

    [Header("Islands Configuration")]
    public IslandSlot[] islandSlots;

    private void OnValidate()
    {
        length = Mathf.Max(0, length);

        if (islandSlots == null) return;

        for (int i = 0; i < islandSlots.Length; i++)
        {
            IslandSlot slot = islandSlots[i];
            slot.xPoint = Mathf.Max(0f, slot.xPoint);
            islandSlots[i] = slot;
        }
    }


    //[Header("Gameplay Configuration")]
}