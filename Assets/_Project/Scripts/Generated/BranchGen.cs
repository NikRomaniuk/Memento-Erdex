using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="BranchManager"/> from the <see cref="BranchGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - BranchData: The ScriptableObject containing STATIC Data </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="BranchManager"/> into Scene </item>
/// </list>
/// </summary>
public class BranchGen
{
	// --- BranchData ---
	public BranchData BranchData { get; private set; }

	// --- Settings ---
	public Orientation Side { get; private set; }
	// Temporary compatibility mirror of Pos.y, kept for existing loaders
	public float Height { get; private set; }
	// Absolute branch origin in world-space coordinates
	public Vector2 Pos { get; private set; }
    // Generated shape chain that belongs to this branch
    public List<ShapeGen> Shapes { get; private set; }

	public BranchGen(BranchData branchData, Orientation side, Vector2 pos)
	{
		BranchData = branchData;
		Side = side;
        Pos = pos;
        Height = pos.y;
        Shapes = new List<ShapeGen>();
	}

    public BranchGen(BranchData branchData, Orientation side, float height)
		// Backward-compatible constructor while migration to Pos.y is in progress
        : this(branchData, side, new Vector2(0f, height))
    {
    }
}
