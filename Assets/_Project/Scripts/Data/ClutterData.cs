using UnityEngine;

[CreateAssetMenu(fileName = "NewClutterData", menuName = "Entries/ClutterData")]
public class ClutterData : ScriptableObject, IData
{
    string IData.id => id;

    // ===
    // MAIN
    // ===
    public string id; // Unique ID

    // ======
    // VISUALS
    // ======
    public Sprite sprite;
    public Color defaultOutlineColor = Color.black; 
    // Can be flipped...
    public bool canBeYFlipped = true; // Vertically
    public bool canBeXFlipped = true; // Horizontally

    // ======
    // GENERAL
    // ======
    public Size size = Size.Tiny;
}
