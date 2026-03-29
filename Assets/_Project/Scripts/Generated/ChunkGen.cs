using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="ChunkManager"/> from the <see cref="ChunkGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - ChunkData: The ScriptableObject containing general STATIC Data </item>
///   <item> - Branches: Array of generated Branches </item>
///   <item> - Settings: Extra DYNAMIC Data needed for loading the <see cref="ChunkManager"/> into Scene </item>
/// </list>
/// </summary>
public class ChunkGen
{
    public ChunkData ChunkData { get; private set; }
    public float CurrentHeight { get; private set; }
    public List<TrunkGen> TrunkParts { get; private set; }
    public List<BranchGen> Branches { get; private set; }

    public ChunkGen(ChunkData chunkData, float currentHeight)
    {
        ChunkData = chunkData;
        CurrentHeight = currentHeight;
        TrunkParts = new List<TrunkGen>();
        Branches = new List<BranchGen>();
    }
}
