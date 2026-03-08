using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> A class that holds generated data for a <see cref="TreeManager"/> from the <see cref="TreeGenerator"/> </para>
/// <para> Contains: </para>
/// <list>
///   <item> - Chunks: List of generated Chunks (<see cref="ChunkGen"/>) for the Tree </item>
/// </list>
/// </summary>
public class TreeGen
{
    // List of generated Chunks for this tree
    public List<ChunkGen> Chunks { get; private set; }

    public TreeGen()
    {
        Chunks = new List<ChunkGen>();
    }
}