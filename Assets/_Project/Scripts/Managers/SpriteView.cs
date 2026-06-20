using UnityEngine;

public class SpriteView : IBuildable
{
    private readonly SpriteRenderer _spriteRenderer;

    public SpriteView(SpriteRenderer spriteRenderer)
    {
        _spriteRenderer = spriteRenderer;
    }

    public void SetData(IData data)
    {
        switch (data)
        {
            case TrunkData trunkData:
                SetData(trunkData, Side.Right, false, true);
                break;
            case ShapeData shapeData:
                SetData(shapeData, Side.Right, false, true);
                break;
            case IslandData islandData:
                SetData(islandData, false);
                break;
            case ClutterData clutterData:
                SetData(clutterData, false, false);
                break;
        }
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped, bool isShape)
    {
        if (_spriteRenderer == null || data == null) { return; }

        int flipX = side == Side.Left ? -1 : 1;

        _spriteRenderer.sprite = isShape ? data.shapeSprite : data.borderSprite;
        _spriteRenderer.flipX = side != Side.Right;
        _spriteRenderer.flipY = isYFlipped;
        _spriteRenderer.transform.localPosition = new Vector3(data.spritesOffset.x * flipX, data.spritesOffset.y, 0f);
    }

    public void SetData(ShapeData data, Side side, bool shouldFlipX, bool isShape)
    {
        if (_spriteRenderer == null || data == null) { return; }

        bool shouldFlipBySide = side == Side.Left;
        bool finalFlipX = shouldFlipBySide ^ shouldFlipX;
        int offsetFlipX = shouldFlipBySide ? -1 : 1;

        _spriteRenderer.sprite = isShape ? data.shapeSprite : data.borderSprite;
        _spriteRenderer.flipX = finalFlipX;
        _spriteRenderer.flipY = false;
        _spriteRenderer.transform.localPosition = new Vector3(data.spritesOffset.x * offsetFlipX, data.spritesOffset.y, 0f);
    }

    public void SetData(IslandData data, bool shouldFlipX)
    {
        if (_spriteRenderer == null || data == null) { return; }

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = shouldFlipX;
        _spriteRenderer.flipY = false;
        _spriteRenderer.transform.localPosition = new Vector3(data.spriteOffset.x * (shouldFlipX ? -1 : 1), data.spriteOffset.y, 0f);
    }

    public void SetData(ClutterData data, bool shouldFlipX, bool shouldFlipY)
    {
        if (_spriteRenderer == null || data == null) { return; }

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = shouldFlipX;
        _spriteRenderer.flipY = shouldFlipY;
        _spriteRenderer.transform.localPosition = Vector3.zero;
    }

    public void Clear()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = null;
            _spriteRenderer.flipX = false;
            _spriteRenderer.flipY = false;
            _spriteRenderer.transform.localPosition = Vector3.zero;
        }
    }
}
