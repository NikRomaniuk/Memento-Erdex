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
	public BranchOrientation Side { get; private set; }
	public float Height { get; private set; }

	public BranchGen(BranchData branchData, BranchOrientation side, float height)
	{
		BranchData = branchData;
		Side = side;
		Height = height;
	}
}
