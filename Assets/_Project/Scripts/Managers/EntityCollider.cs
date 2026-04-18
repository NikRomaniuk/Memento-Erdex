using UnityEngine;

public class EntityCollider : IBuildable
{
    private readonly BoxCollider2D _boxCollider;

    public EntityCollider(BoxCollider2D boxCollider)
    {
        _boxCollider = boxCollider;
    }

    public void SetData(IData data)
    {
        switch (data)
        {
            case TrunkData trunkData:
                SetData(trunkData, Side.Right, false);
                break;
            case IslandData islandData:
                SetData(islandData);
                break;
        }
    }

    public void SetData(TrunkData data, Side side, bool isYFlipped)
    {
        if (_boxCollider == null || data == null) { return; }

        int flipX = side == Side.Left ? -1 : 1;
        _boxCollider.size = data.colliderSize;
        _boxCollider.offset = new Vector2(data.colliderOffset.x * flipX, data.colliderOffset.y);
    }

    public void SetData(IslandData data)
    {
        if (_boxCollider == null || data == null) { return; }

        _boxCollider.size = data.colliderSize;
        _boxCollider.offset = data.colliderOffset;
    }

    public void Clear()
    {
        if (_boxCollider == null) { return; }

        _boxCollider.size = Vector2.zero;
        _boxCollider.offset = Vector2.zero;
    }
}
