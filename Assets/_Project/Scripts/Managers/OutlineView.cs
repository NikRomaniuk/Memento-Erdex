using UnityEngine;

public class OutlineView : IBuildable
{
    private readonly SpriteRenderer _outlineRenderer;
    private Color _defaultColor = Color.black;

    private OutlineShaderHelper _outlineShaderHelper;

    public Color DefaultColor => _defaultColor;

    public OutlineView(SpriteRenderer outlineRenderer, Color defaultColor)
    {
        _outlineRenderer = outlineRenderer;
        _defaultColor = defaultColor;
    }

    public void SetData(IData data)
    {
        switch (data)
        {
            case TrunkData trunkData:
                SetData(trunkData, Side.Right, false);
                break;
            case ShapeData shapeData:
                SetData(shapeData, false);
                break;
            case IslandData islandData:
                SetData(islandData, false);
                break;
        }
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped)
    {
        if (_outlineRenderer == null || data == null) { return; }

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = side != Side.Right;
        _outlineRenderer.flipY = isYFlipped;
    }

    public void SetData(ShapeData data, bool shouldFlipX)
    {
        if (_outlineRenderer == null || data == null) { return; }

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = shouldFlipX;
        _outlineRenderer.flipY = false;
    }

    public void SetData(IslandData data, bool shouldFlipX)
    {
        if (_outlineRenderer == null || data == null) { return; }

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = shouldFlipX;
        _outlineRenderer.flipY = false;
    }

    public void Clear()
    {
        if (_outlineRenderer != null)
        {
            _outlineRenderer.sprite = null;
            _outlineRenderer.flipX = false;
            _outlineRenderer.flipY = false;
        }

        SetDefaultColor(Color.black);
    }

    public void SetDefaultColor(Color defaultColor)
    {
        _defaultColor = defaultColor;
    }

    public void ApplyColor(Color color)
    {
        OutlineShaderHelper outlineShaderHelper = GetOutlineShaderHelper();
        if (outlineShaderHelper == null) { return; }

        outlineShaderHelper.SetOutlineColor(color);
    }

    public void ResetColor()
    {
        ApplyColor(_defaultColor);
    }

    private OutlineShaderHelper GetOutlineShaderHelper()
    {
        if (_outlineShaderHelper != null) { return _outlineShaderHelper; }
        if (_outlineRenderer == null) { return null; }

        _outlineShaderHelper = _outlineRenderer.GetComponent<OutlineShaderHelper>();
        return _outlineShaderHelper;
    }
}
