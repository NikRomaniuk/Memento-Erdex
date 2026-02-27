using UnityEngine;

[ExecuteInEditMode]
public class OutlineShaderHelper : MonoBehaviour
{
    // Don't need this one anymore since we're calculating the width based on equasion
    //[SerializeField] private float _outlineWidth = 1f;
    
    private SpriteRenderer _sr;
    private MaterialPropertyBlock _block;

    void OnEnable()
    {
        _sr = GetComponent<SpriteRenderer>();
        _block = new MaterialPropertyBlock();
    }

    void LateUpdate()
    {
        if (_sr == null || _sr.sprite == null) return;

        Vector4 uv = UnityEngine.Sprites.DataUtility.GetInnerUV(_sr.sprite);
        float scaleX = uv.z - uv.x;
        float scaleY = uv.w - uv.y;

        Vector3 worldScale = transform.lossyScale;

        float pixelWidth = _sr.sprite.rect.width;
        float pixelHeight = _sr.sprite.rect.height;

        float finalWidth = 160f / (pixelWidth + 48f);

        _sr.GetPropertyBlock(_block);
        _block.SetVector("_CustomUVScale", new Vector4(scaleX, scaleY, 0, 0));
        _block.SetVector("_CustomWorldScale", new Vector4(worldScale.x, worldScale.y, 1, 1));
        _block.SetVector("_SpritePixelSize", new Vector2(pixelWidth, pixelHeight));
        _block.SetFloat("_Outline_Width", finalWidth);
        _sr.SetPropertyBlock(_block);

        //Debug.Log($"UV Scale: {scaleX}/{scaleY}, Pixels: {pixelWidth}/{pixelHeight}");
    }
}
