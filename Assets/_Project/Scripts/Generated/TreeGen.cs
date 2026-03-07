using System.Collections.Generic;
using UnityEngine;

public class TreeGen
{
    public List<ChunkGen> Chunks { get; private set; }

    public TreeGen()
    {
        Chunks = new List<ChunkGen>();
    }
}