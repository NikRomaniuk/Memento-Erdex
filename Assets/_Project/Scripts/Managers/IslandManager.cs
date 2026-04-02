using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour, IBakeable, IBuildable
{
    [Header("References")]
    // --- References ---
    public SpriteRenderer _spriteRenderer;
    public SpriteRenderer _outlineRenderer;
    public BoxCollider2D _boxCollider;

    [Header("Properties")]
    [SerializeField] private Size _size = Size.Tiny;

    [Header("Prop Data")]
    [SerializeField] private PropSlot[] _staticPropSlots;
    [SerializeField] private int _propCapacity;

    // --- Runtime ---
    [HideInInspector] public List<PropManager> LoadedProps = new List<PropManager>(); // Active PropManagers loaded for this Шsland

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not IslandData islandData) return;

        // --- Bake visual data ---
        islandData.sprite = _spriteRenderer.sprite;
        islandData.spriteOffset = _spriteRenderer.transform.localPosition;

        // --- Bake collider data ---
        islandData.colliderSize = _boxCollider.size;
        islandData.colliderOffset = _boxCollider.offset;

        // --- Bake general data ---
        islandData.size = _size;

        // --- Bake prop data ---
        islandData.staticPropSlots = _staticPropSlots != null ? (PropSlot[])_staticPropSlots.Clone() : new PropSlot[0];
        islandData.propCapacity = Mathf.Max(0, _propCapacity);
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not IslandData islandData) return;
        SetData(islandData, false);
    }

    public void SetData(IslandData data, bool isXFlipped)
    {
        bool shouldFlipX = isXFlipped && data.canBeXFlipped; // Flip only if allowed to!

        // --- Apply Visual ---
        // - Sprite -
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = shouldFlipX;
        // - Outline -
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = shouldFlipX;

        // --- Apply Collider ---
        _boxCollider.size = data.colliderSize;
        _boxCollider.offset = data.colliderOffset;

        // --- Apply General ---
        _size = data.size;

        // --- Apply Prop Data ---
        _staticPropSlots = data.staticPropSlots != null ? (PropSlot[])data.staticPropSlots.Clone() : new PropSlot[0];
        _propCapacity = Mathf.Max(0, data.propCapacity);
    }

    public void Clear()
    {
        // --- Reset Visual ---
        // - Sprite -
        _spriteRenderer.sprite = null;
        _spriteRenderer.flipX = false;
        // - Outline -
        _outlineRenderer.sprite = null;
        _outlineRenderer.flipX = false;

        // --- Reset Collider ---
        _boxCollider.size = Vector2.zero;
        _boxCollider.offset = Vector2.zero;

        // --- Reset General ---
        _size = Size.Tiny;

        // --- Reset Prop Data ---
        _staticPropSlots = new PropSlot[0];
        _propCapacity = 0;
        LoadedProps.Clear();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("IslandManager cleared to initial state");
    }

    private void OnValidate()
    {
        _propCapacity = Mathf.Max(0, _propCapacity);
    }

    // --- Public accessors ---
    public Size Size => _size;
    public PropSlot[] StaticPropSlots => _staticPropSlots;
    public int PropCapacity => _propCapacity;
}
