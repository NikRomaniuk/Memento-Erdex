using System.Collections.Generic;
using UnityEngine;

// Enums
public enum Orientation
{
    Left,
    Right,
    Middle
}

public enum Side
{
    Left,
    Right
}

public enum Size
{
    Tiny,
    Small,
    Medium
}


//public enum GenerationType { Static, Dynamic }

public static class Constants
{
    public const float UNIT_SIZE = 0.8f;
    public const float BRANCH_SLOT_X_OFFSET = UNIT_SIZE;

    public static readonly Dictionary<Size, float> SIZE_VALUES = new Dictionary<Size, float>
    {
        { Size.Tiny, UNIT_SIZE },
        { Size.Small, UNIT_SIZE * 2f },
        { Size.Medium, UNIT_SIZE * 3f }
    };
}
