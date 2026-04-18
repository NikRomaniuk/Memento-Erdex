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
                SetData(trunkData, Side.Right, false);
                break;
            case ShapeData shapeData:
                SetData(shapeData, false, shapeData.spriteOffset);
                break;
            case IslandData islandData:
                SetData(islandData, false);
                break;
        }
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped)
    {
        if (_spriteRenderer == null || data == null) { return; }

        int flipX = side == Side.Left ? -1 : 1;

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = side != Side.Right;
        _spriteRenderer.flipY = isYFlipped;
        _spriteRenderer.transform.localPosition = new Vector3(data.spriteOffset.x * flipX, data.spriteOffset.y, 0f);
    }

    public void SetData(ShapeData data, bool shouldFlipX, Vector3 spriteLocalPosition)
    {
        if (_spriteRenderer == null || data == null) { return; }

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = shouldFlipX;
        _spriteRenderer.flipY = false;
        _spriteRenderer.transform.localPosition = new Vector3(spriteLocalPosition.x, spriteLocalPosition.y, 0f);
    }

    public void SetData(IslandData data, bool shouldFlipX)
    {
        if (_spriteRenderer == null || data == null) { return; }

        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.flipX = shouldFlipX;
        _spriteRenderer.flipY = false;
        _spriteRenderer.transform.localPosition = new Vector3(data.spriteOffset.x * (shouldFlipX ? -1 : 1), data.spriteOffset.y, 0f);
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
