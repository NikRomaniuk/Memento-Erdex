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
    [SerializeField] private Color _defaultOutlineColor = Color.black;

    // --- Runtime ---
    [HideInInspector] public List<PropManager> LoadedProps = new List<PropManager>(); // Active PropManagers loaded for this Шsland

    private SpriteView _spriteView;
    private OutlineView _outlineView;
    private EntityCollider _entityCollider;

    public SpriteView SpriteView => _spriteView;
    public OutlineView OutlineView => _outlineView;
    public EntityCollider EntityCollider => _entityCollider;

    private void Awake()
    {
        EnsureViewsInitialized();
    }

    private void EnsureViewsInitialized()
    {
        _spriteView ??= new SpriteView(_spriteRenderer);
        _outlineView ??= new OutlineView(_outlineRenderer, _defaultOutlineColor);
        _entityCollider ??= new EntityCollider(_boxCollider);
    }

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not IslandData islandData) return;
        EnsureViewsInitialized();

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

        // --- Bake misc data ---
        islandData.defaultOutlineColor = _outlineView.DefaultColor;
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not IslandData islandData) return;
        SetData(islandData, false);
    }

    public void SetData(IslandData data, bool isXFlipped)
    {
        EnsureViewsInitialized();

        bool shouldFlipX = isXFlipped && data.canBeXFlipped; // Flip only if allowed to!

        // --- Apply Visual ---
        _spriteView.SetData(data, shouldFlipX);
        _outlineView.SetData(data, shouldFlipX);

        // --- Apply Collider ---
        _entityCollider.SetData(data);

        // --- Apply General ---
        _size = data.size;

        // --- Apply Prop Data ---
        _staticPropSlots = data.staticPropSlots != null ? (PropSlot[])data.staticPropSlots.Clone() : new PropSlot[0];
        _propCapacity = Mathf.Max(0, data.propCapacity);

        // --- Apply Misc ---
        _defaultOutlineColor = _outlineView.DefaultColor;
    }

    public void Clear()
    {
        EnsureViewsInitialized();

        // --- Reset Visual ---
        _spriteView.Clear();
        _outlineView.Clear();

        // --- Reset Collider ---
        _entityCollider.Clear();

        // --- Reset General ---
        _size = Size.Tiny;

        // --- Reset Prop Data ---
        _staticPropSlots = new PropSlot[0];
        _propCapacity = 0;
        LoadedProps.Clear();

        // --- Reset Misc ---
        _defaultOutlineColor = _outlineView.DefaultColor;

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
