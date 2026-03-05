using System.Collections.Generic;
using UnityEngine;

public class ChunkGen
{
    public ChunkData ChunkData { get; private set; }
    public float CurrentHeight { get; private set; }
    public List<TrunkGen> TrunkParts { get; private set; }

    public ChunkGen(ChunkData chunkData, float currentHeight)
    {
        ChunkData = chunkData;
        CurrentHeight = currentHeight;
        TrunkParts = new List<TrunkGen>();
    }
}
