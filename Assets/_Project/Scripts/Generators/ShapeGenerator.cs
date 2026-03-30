using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    // Allowed overflow relative to BranchData.length
    private const float MAX_LENGTH_ALLOWANCE = 4f;
    // Number of chain assembly tries per branch
    private const int MAX_ATTEMPTS = 20;
    // Hard cap against accidental infinite chain growth in one attempt
    private const int MAX_SHAPES_PER_BRANCH = 32;

    [Header("Shape Pool")]
    [SerializeField] private ShapeData[] _shapeDataPool;

    [Header("Visual Settings")]
    [SerializeField] private Color _color = Color.white;

    public void GenerateShapes(BranchGen branchGen, System.Random random)
    {
        // --- Validations ---
        if (_shapeDataPool == null || _shapeDataPool.Length == 0)
        {
            Debug.LogError("ShapeData pool is empty! Cannot generate shapes");
            return;
        }

        if (!TryMapBranchOrientationToSide(branchGen.Side, out Side shapeSide))
        {
            Debug.LogWarning($"Branch side '{branchGen.Side}' is not supported for shape generation");
            return;
        }

        // Regenerate branch shapes from scratch each time
        branchGen.Shapes.Clear();

        // Split source pool into base and tip candidates once per branch generation
        List<ShapeData> basePool = new List<ShapeData>();
        List<ShapeData> tipPool = new List<ShapeData>();
        foreach (ShapeData data in _shapeDataPool)
        {
            if (data == null || data.Length <= 0f)
                continue;

            if (data.type == ShapeData.Type.Tip)
                tipPool.Add(data);
            else
                basePool.Add(data);
        }

        if (tipPool.Count == 0)
        {
            Debug.LogWarning($"No Tip shapes available for branch '{branchGen.BranchData.id}'. Cannot generate a valid chain");
            return;
        }

        float targetLength = Mathf.Max(0f, branchGen.BranchData.length);
        float maxLength = targetLength + MAX_LENGTH_ALLOWANCE;

        List<ShapeData> assembledShapeData = null;
        // Retry assembling a valid chain; on first success we keep that candidate
        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            if (TryAssembleShapeChain(random, targetLength, maxLength, basePool, tipPool, out assembledShapeData))
            {
                break;
            }
        }

        if (assembledShapeData == null)
        {
            Debug.LogWarning($"Failed to assemble shape chain for branch '{branchGen.BranchData.id}' in {MAX_ATTEMPTS} attempts");
            return;
        }

        // Position all generated shapes as a continuous chain from branch origin
        Vector2 currentPos = branchGen.Pos;
        for (int i = 0; i < assembledShapeData.Count; i++)
        {
            ShapeData selectedShapeData = assembledShapeData[i];
            bool isTip = i == assembledShapeData.Count - 1;

            bool isXFlipped = false;
            if (!isTip && selectedShapeData.canBeXFlipped)
            {
                isXFlipped = random.Next(0, 2) == 1;
            }

            ShapeGen shapeGen = new ShapeGen(selectedShapeData, shapeSide, isXFlipped, currentPos, 80, _color);
            branchGen.Shapes.Add(shapeGen);

            // Left branches extend towards negative X, right branches towards positive X
            float nextXDelta = selectedShapeData.Length;
            currentPos.x += shapeSide == Side.Left ? -nextXDelta : nextXDelta;
        }

        Debug.Log($"Generated {assembledShapeData.Count} shapes for branch '{branchGen.BranchData.id}' at height {branchGen.Pos.y}");
    }

    private bool TryAssembleShapeChain(
        System.Random random,
        float targetLength,
        float maxLength,
        List<ShapeData> basePool,
        List<ShapeData> tipPool,
        out List<ShapeData> assembledShapeData)
    {
        assembledShapeData = new List<ShapeData>();
        float currentLength = 0f;

        for (int index = 0; index < MAX_SHAPES_PER_BRANCH; index++)
        {
            // Last piece must always be Tip, so we test if branch can be closed now
            ShapeData randomTip = tipPool[random.Next(0, tipPool.Count)];
            float lengthWithTip = currentLength + randomTip.Length;

            if (lengthWithTip >= targetLength && lengthWithTip <= maxLength)
            {
                assembledShapeData.Add(randomTip);
                return true;
            }

            List<ShapeData> feasibleBasePool = GetFeasibleBasePool(basePool, currentLength, maxLength);
            if (feasibleBasePool.Count == 0)
            {
                assembledShapeData = null;
                return false;
            }

            // Branch is still too short; append one random feasible Base and continue
            ShapeData randomBase = feasibleBasePool[random.Next(0, feasibleBasePool.Count)];
            assembledShapeData.Add(randomBase);
            currentLength += randomBase.Length;
        }

        assembledShapeData = null;
        return false;
    }

    private List<ShapeData> GetFeasibleBasePool(List<ShapeData> basePool, float currentLength, float maxLength)
    {
        List<ShapeData> feasibleBasePool = new List<ShapeData>();

        for (int i = 0; i < basePool.Count; i++)
        {
            ShapeData candidate = basePool[i];
            if (currentLength + candidate.Length <= maxLength)
            {
                feasibleBasePool.Add(candidate);
            }
        }

        return feasibleBasePool;
    }

    private bool TryMapBranchOrientationToSide(BranchOrientation branchOrientation, out Side side)
    {
        // Only left/right branches can host shapes in current generation rules
        switch (branchOrientation)
        {
            case BranchOrientation.Left:
                side = Side.Left;
                return true;

            case BranchOrientation.Right:
                side = Side.Right;
                return true;

            case BranchOrientation.Middle:
            default:
                side = Side.Right;
                return false;
        }
    }
}
