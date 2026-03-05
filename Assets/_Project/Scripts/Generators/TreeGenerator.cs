using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private int _seed = 0;
    [SerializeField] private float _treeHeight = 10f;
    public int Seed => _seed; // Seed getter for external access
    private TrunkGenerator _trunkGenerator;
    private ChunkGenerator _chunkGenerator;
    private TreeGen _treeGen;

    private void Awake()
    {
        _trunkGenerator = GetComponent<TrunkGenerator>();
        _chunkGenerator = GetComponent<ChunkGenerator>();
    }

    private void Start()
    {
        GenerateTree(new System.Random(_seed));
    }

    public void GenerateTree(System.Random random)
    {
        if (_chunkGenerator == null) { Debug.LogError("ChunkGenerator component not found!"); return;}
        if (_trunkGenerator == null) { Debug.LogError("TrunkGenerator component not found!"); return;}
        
        _treeGen = new TreeGen();
        _chunkGenerator.GenerateChunks(random, _treeGen, _treeHeight);
        _trunkGenerator.GenerateTrunk(random, _treeGen, _treeHeight);
        
        
    }
}
