using UnityEngine;

public class ClutterManager : MonoBehaviour, IBakeable, IBuildable
{
    // --- References ---
    public SpriteRenderer _spriteRenderer;
    public SpriteRenderer _outlineRenderer;

    // --- Properties ---
    [SerializeField] private Size _size = Size.Tiny;
    [SerializeField] private ClutterData _data;

    private SpriteView _spriteView;
    private OutlineView _outlineView;

    public SpriteView SpriteView => _spriteView;
    public OutlineView OutlineView => _outlineView;
    public ClutterData Data => _data;

    private void Awake()
    {
        EnsureViewsInitialized();
    }

    private void EnsureViewsInitialized()
    {
        _spriteView ??= new SpriteView(_spriteRenderer);
        _outlineView ??= new OutlineView(_outlineRenderer, Color.black);
    }

    // --- IBakeable ---
    public void GatherData(IData data)
    {
        if (data is not ClutterData clutterData) return;
        EnsureViewsInitialized();

        _data = clutterData;
        clutterData.sprite = _spriteRenderer.sprite;
        clutterData.size = _size;
    }

    // --- IBuildable ---
    public void SetData(IData data)
    {
        if (data is not ClutterData clutterData) return;
        SetData(clutterData, false, false);
    }

    public void SetData(ClutterData data, bool isXFlipped, bool isYFlipped)
    {
        EnsureViewsInitialized();

        _data = data;
        _spriteView.SetData(data, isXFlipped, isYFlipped);
        _outlineView.SetData(data, isXFlipped, isYFlipped);

        _size = data.size;
    }

    public void Clear()
    {
        EnsureViewsInitialized();

        _spriteView.Clear();
        _outlineView.Clear();

        _size = Size.Tiny;
        _data = null;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        gameObject.SetActive(false);
        Debug.Log("ClutterManager cleared to initial state");
    }
}