using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    // --- Settings ---
    private float _treeHeight = 10f;

    // --- References ---
    [SerializeField] private BlanksLibrary _blanksLibrary; // Must complete GenerateBlanks before tree can generate
    [SerializeField] private TreeConstants _treeConstants;
    [SerializeField] private TreeGenerationData _activeTreeGeneration;
    [SerializeField] private TreeGenerationData _defaultTreeGeneration;

    // --- Events ---
    [SerializeField] private GameEvent_Float _onTreeHeightDefined;

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
        // --- Setup ---
        // Seed
        _seed = Random.Range(0, 1000000000);

        if (!_activeTreeGeneration.GetUseRandomSeed())
        {
            _seed = _activeTreeGeneration.GetFixedSeed();
        }

        // Tree Height
        _treeHeight = _defaultTreeGeneration.GetTreeHeight();

        if (_activeTreeGeneration.GetTreeHeight() >= _treeConstants.GetMinTreeHeight() &&
            _activeTreeGeneration.GetTreeHeight() <= _treeConstants.GetMaxTreeHeight())
        {
            _treeHeight = _activeTreeGeneration.GetTreeHeight();
        }

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

        // --- Notify listeners that tree generation is fully complete ---
        _onTreeHeightDefined?.Invoke(_chunkGenerator.GeneratedTreeHeight);
    }
}
