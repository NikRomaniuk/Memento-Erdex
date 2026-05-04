using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    // --- Settings ---
    [SerializeField] private float _treeHeight = 10f;

    // --- References ---
    [SerializeField] private BlanksLibrary _blanksLibrary; // Must complete GenerateBlanks before tree can generate
    [SerializeField] private GameplayData _activeGameplay;

    // --- Data ---
    private int _seed = 0;

    // --- Getters ---
    public int Seed => _seed;
    public float TreeHeight => _treeHeight;

    // --- Components ---
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
        _seed = _activeGameplay.GetSeed();
        GenerateTree(new System.Random(_seed));
    }

    public void GenerateTree(System.Random random)
    {
        // --- Validations ---
        if (_chunkGenerator == null) { Debug.LogError("ChunkGenerator component not found!"); return; }
        if (_trunkGenerator == null) { Debug.LogError("TrunkGenerator component not found!"); return; }
        if (_blanksLibrary == null || !_blanksLibrary.IsReady)
        {
            Debug.LogError("BlanksLibrary is not ready! Cannot generate tree");
            return;
        }

        // --- Generate ---
        _treeGen = new TreeGen();
        _chunkGenerator.GenerateChunks(random, _treeGen, _treeHeight);
        _trunkGenerator.GenerateTrunk(random, _treeGen, _chunkGenerator.GeneratedTreeHeight);

        // --- Publish data (signals TreeManager that generation is complete) ---
        TreeLoader.GenData = _treeGen;
    }
}
