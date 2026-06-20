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
                SetData(shapeData, Side.Right, false);
                break;
            case IslandData islandData:
                SetData(islandData, false);
                break;
            case ClutterData clutterData:
                SetData(clutterData, false, false);
                break;
        }
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped)
    {
        if (_outlineRenderer == null || data == null) { return; }

        int flipX = side == Side.Left ? -1 : 1;

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.borderSprite;
        _outlineRenderer.flipX = side != Side.Right;
        _outlineRenderer.flipY = isYFlipped;
        _outlineRenderer.transform.localPosition = new Vector3(data.spritesOffset.x * flipX, data.spritesOffset.y, 0f);
    }

    public void SetData(ShapeData data, Side side, bool shouldFlipX)
    {
        if (_outlineRenderer == null || data == null) { return; }

        bool shouldFlipBySide = side == Side.Left;
        bool finalFlipX = shouldFlipBySide ^ shouldFlipX;
        int offsetFlipX = shouldFlipBySide ? -1 : 1;

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.borderSprite;
        _outlineRenderer.flipX = finalFlipX;
        _outlineRenderer.flipY = false;
        _outlineRenderer.transform.localPosition = new Vector3(data.spritesOffset.x * offsetFlipX, data.spritesOffset.y, 0f);
    }

    public void SetData(IslandData data, bool shouldFlipX)
    {
        if (_outlineRenderer == null || data == null) { return; }

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = shouldFlipX;
        _outlineRenderer.flipY = false;
        _outlineRenderer.transform.localPosition = new Vector3(data.spriteOffset.x * (shouldFlipX ? -1 : 1), data.spriteOffset.y, 0f);
    }

    public void SetData(ClutterData data, bool shouldFlipX, bool shouldFlipY)
    {
        if (_outlineRenderer == null || data == null) { return; }

        SetDefaultColor(data.defaultOutlineColor);
        _outlineRenderer.sprite = data.sprite;
        _outlineRenderer.flipX = shouldFlipX;
        _outlineRenderer.flipY = shouldFlipY;
        _outlineRenderer.transform.localPosition = Vector3.zero;
    }

    public void Clear()
    {
        if (_outlineRenderer != null)
        {
            _outlineRenderer.sprite = null;
            _outlineRenderer.flipX = false;
            _outlineRenderer.flipY = false;
            _outlineRenderer.transform.localPosition = Vector3.zero;
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
