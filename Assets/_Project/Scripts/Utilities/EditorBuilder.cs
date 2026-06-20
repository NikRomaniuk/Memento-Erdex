using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditorBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Builder _builder;

    [Header("General Data")]
    [SerializeField] private BlanksLibrary.BlankType _blankType;
    [SerializeField] private ScriptableObject _dataToBuild;

    // --- Trunk Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Trunk)]
    [SerializeField] private Side _trunkSide = Side.Right;

    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Trunk)]
    [SerializeField] private bool _trunkIsYFlipped = false;

    [ShowIf("@_blankType == BlanksLibrary.BlankType.Trunk || _blankType == BlanksLibrary.BlankType.Shape")]
    [SerializeField] private ClutterList _clutterList;

    [ShowIf("@_blankType == BlanksLibrary.BlankType.Trunk || _blankType == BlanksLibrary.BlankType.Shape")]
    [SerializeField] private DataDatabase _clutterTemplateDB;

    [ShowIf("@_blankType == BlanksLibrary.BlankType.Trunk || _blankType == BlanksLibrary.BlankType.Shape")]
    [SerializeField] private BlanksLibrary _blanksLibrary;

    // --- Chunk Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Chunk)]
    [SerializeField] private float _chunkCurrentHeight = 0f;

    // --- Branch Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Branch)]
    [SerializeField] private Orientation _branchOrientation = Orientation.Right;

    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Branch)]
    [SerializeField] private float _branchCurrentHeight = 0f;

    // --- Shape Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Shape)]
    [SerializeField] private Side _shapeSide = Side.Right;

    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Shape)]
    [SerializeField] private bool _shapeIsXFlipped = false;

    // --- Island Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Island)]
    [SerializeField] private bool _islandIsXFlipped = false;

    // --- Clutter Data ---
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Clutter)]
    [SerializeField] private bool _clutterIsXFlipped = false;
    
    [ShowIf(nameof(_blankType), BlanksLibrary.BlankType.Clutter)]
    [SerializeField] private bool _clutterIsYFlipped = false;

    [Button(ButtonSizes.Medium)]
    public void BuildFromEditor()
    {
#if UNITY_EDITOR
        if (_builder == null)
        {
            Debug.LogError("Builder is not assigned");
            return;
        }

        if (_dataToBuild == null)
        {
            Debug.LogError("No data assigned! Please assign a ScriptableObject to build from");
            return;
        }

        switch (_blankType)
        {
            case BlanksLibrary.BlankType.Trunk:
            {
                var trunkData = _dataToBuild as TrunkData;
                if (trunkData == null)
                {
                    Debug.LogError("_dataToBuild is not a TrunkData! Please assign a TrunkData ScriptableObject");
                    return;
                }
                // Initialize TrunkSegment with data & extra settings
                _builder.Initialize(trunkData, _trunkSide, _trunkIsYFlipped);
                Debug.Log($"TrunkPart successfully built from <b>{trunkData.name}</b> (ID: {trunkData.id})");

                if (_clutterList == null) { Debug.LogError("ClutterList is not assigned"); return; }
                if (_clutterTemplateDB == null) { Debug.LogError("ClutterTemplateDB is not assigned"); return; }
                if (_blanksLibrary == null) { Debug.LogError("BlanksLibrary is not assigned"); return; }

                var clutterPrefab = _blanksLibrary.ClutterManagerPrefab;
                if (clutterPrefab == null) { Debug.LogError("BlanksLibrary Prefab is not assigned"); return; }

                ClearClutterListChildren();

                var slots = trunkData.clutterSlots;
                if (slots == null || slots.Length == 0)
                {
                    Debug.LogWarning("TrunkData has no clutter slots to build");
                    return;
                }

                int sideFlipX = _trunkSide == Side.Left ? -1 : 1;
                bool shouldFlipX = _trunkSide == Side.Left;
                bool shouldFlipY = _trunkIsYFlipped;
                float trunkHeight = trunkData.Height;

                var templateData = _clutterTemplateDB.GetData<ClutterData>();
                for (int i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    ClutterData data = slot.isStatic
                        ? slot.staticClutterData
                        : templateData.FirstOrDefault(item => item.size == slot.size);

                    if (data == null)
                    {
                        Debug.LogWarning($"ClutterSlot {i} has no matching ClutterData. Skipping");
                        continue;
                    }

                    Vector2 slotPos = slot.pos;
                    slotPos.x *= sideFlipX;
                    if (shouldFlipY)
                    {
                        slotPos.y = trunkHeight - slotPos.y;
                    }

                    var clutter = Instantiate(clutterPrefab, _clutterList.transform);
                    clutter.transform.localPosition = new Vector3(slotPos.x, slotPos.y, 0f);
                    clutter.SetData(data, shouldFlipX, shouldFlipY);

                    // Deactivate SpriteRenderer for non-static Clutter
                    if (!slot.isStatic && clutter._spriteRenderer != null)
                    {
                        clutter._spriteRenderer.enabled = false;
                    }

                    UnityEditor.EditorUtility.SetDirty(clutter);
                    UnityEditor.EditorUtility.SetDirty(clutter.gameObject);
                }
                break;
            }

            case BlanksLibrary.BlankType.Chunk:
            {
                var chunkData = _dataToBuild as ChunkData;
                if (chunkData == null)
                {
                    Debug.LogError("_dataToBuild is not a ChunkData! Please assign a ChunkData ScriptableObject");
                    return;
                }
                // Initialize ChunkManager with data
                _builder.Initialize(chunkData, _chunkCurrentHeight);
                Debug.Log($"Chunk successfully built from <b>{chunkData.name}</b>");
                break;
            }

            case BlanksLibrary.BlankType.Branch:
            {
                var branchData = _dataToBuild as BranchData;
                if (branchData == null)
                {
                    Debug.LogError("_dataToBuild is not a BranchData! Please assign a BranchData ScriptableObject");
                    return;
                }
                // Initialize BranchManager with data & extra settings
                _builder.Initialize(branchData, _branchOrientation, _branchCurrentHeight);
                Debug.Log($"Branch successfully built from <b>{branchData.name}</b> (ID: {branchData.id})");
                break;
            }

            case BlanksLibrary.BlankType.Shape:
            {
                var shapeData = _dataToBuild as ShapeData;
                if (shapeData == null)
                {
                    Debug.LogError("_dataToBuild is not a ShapeData! Please assign a ShapeData ScriptableObject");
                    return;
                }
                // Initialize ShapeManager with data & extra settings
                _builder.Initialize(shapeData, _shapeSide, _shapeIsXFlipped);
                Debug.Log($"Shape successfully built from <b>{shapeData.name}</b> (ID: {shapeData.id})");

                if (_clutterList == null) { Debug.LogError("ClutterList is not assigned"); return; }
                if (_clutterTemplateDB == null) { Debug.LogError("ClutterTemplateDB is not assigned"); return; }
                if (_blanksLibrary == null) { Debug.LogError("BlanksLibrary is not assigned"); return; }

                var shapeClutterPrefab = _blanksLibrary.ClutterManagerPrefab;
                if (shapeClutterPrefab == null) { Debug.LogError("BlanksLibrary Prefab is not assigned"); return; }

                ClearClutterListChildren();

                var shapeSlots = shapeData.clutterSlots;
                if (shapeSlots == null || shapeSlots.Length == 0)
                {
                    Debug.LogWarning("ShapeData has no clutter slots to build");
                    return;
                }

                int sideFlipX = _shapeSide == Side.Left ? -1 : 1;
                bool shouldFlipX = (_shapeSide == Side.Left) ^ _shapeIsXFlipped;
                bool shouldFlipY = false;
                float shapeLength = shapeData.Length;

                var shapeTemplateData = _clutterTemplateDB.GetData<ClutterData>();
                for (int i = 0; i < shapeSlots.Length; i++)
                {
                    var slot = shapeSlots[i];
                    ClutterData data = slot.isStatic
                        ? slot.staticClutterData
                        : shapeTemplateData.FirstOrDefault(item => item.size == slot.size);

                    if (data == null)
                    {
                        Debug.LogWarning($"ClutterSlot {i} has no matching ClutterData. Skipping");
                        continue;
                    }

                    Vector2 slotPos = slot.pos;
                    if (_shapeIsXFlipped)
                    {
                        slotPos.x = shapeLength - slotPos.x;
                    }
                    slotPos.x *= sideFlipX;

                    var clutter = Instantiate(shapeClutterPrefab, _clutterList.transform);
                    clutter.transform.localPosition = new Vector3(slotPos.x, slotPos.y, 0f);
                    clutter.SetData(data, shouldFlipX, shouldFlipY);
                    if (!slot.isStatic && clutter._spriteRenderer != null)
                    {
                        clutter._spriteRenderer.enabled = false;
                    }

                    UnityEditor.EditorUtility.SetDirty(clutter);
                    UnityEditor.EditorUtility.SetDirty(clutter.gameObject);
                }
                break;
            }

            case BlanksLibrary.BlankType.Island:
            {
                var islandData = _dataToBuild as IslandData;
                if (islandData == null)
                {
                    Debug.LogError("_dataToBuild is not an IslandData! Please assign an IslandData ScriptableObject");
                    return;
                }
                // Initialize IslandManager with data & extra settings
                _builder.Initialize(islandData, _islandIsXFlipped);
                Debug.Log($"Island successfully built from <b>{islandData.name}</b> (ID: {islandData.id})");
                break;
            }

            case BlanksLibrary.BlankType.Clutter:
            {
                var clutterData = _dataToBuild as ClutterData;
                if (clutterData == null)
                {
                    Debug.LogError("_dataToBuild is not a ClutterData! Please assign a ClutterData ScriptableObject");
                    return;
                }
                // Initialize ClutterManager with data & extra settings
                _builder.Initialize(clutterData, _clutterIsXFlipped, _clutterIsYFlipped);
                Debug.Log($"Clutter successfully built from <b>{clutterData.name}</b> (ID: {clutterData.id})");
                break;
            }

            default:
                Debug.LogError($"Unknown BlankType: {_blankType}. Aborting BuildFromEditor");
                break;
        }
#else
        Debug.LogWarning("BuildFromEditor is available only in the Unity Editor");
#endif
    }

    private void ClearClutterListChildren()
    {
#if UNITY_EDITOR
        var parent = _clutterList.transform;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
#endif
    }
}
