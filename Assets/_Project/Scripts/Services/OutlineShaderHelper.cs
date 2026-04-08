using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShaderHelper : MonoBehaviour
{
    private static readonly int _mainTexExplicitId = Shader.PropertyToID("_MainTexExplicit");
    private static readonly int _TexelSizeId = Shader.PropertyToID("_TexelSize");
    private static readonly int _customUVScaleId = Shader.PropertyToID("_CustomUVScale");
    private static readonly int _customWorldScaleId = Shader.PropertyToID("_CustomWorldScale");
    private static readonly int _spritePixelSizeId = Shader.PropertyToID("_SpritePixelSize");
    private static readonly int _outlineWidthId = Shader.PropertyToID("_Outline_Width");

    private const float _outlineWidthNumerator = 160f;
    private const float _outlineWidthPadding = 48f;

    private SpriteRenderer _sr;
    private MaterialPropertyBlock _block;

    private void OnEnable()
    {
        _sr = GetComponent<SpriteRenderer>();
        _block ??= new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (_sr == null || _sr.sprite == null) { return; }

        Sprite sprite = _sr.sprite;

        Texture2D texture = sprite.texture;
        float texWidth = texture != null ? texture.width : 1f;
        float texHeight = texture != null ? texture.height : 1f;
        Vector4 texelSize = new Vector4(1f / texWidth, 1f / texHeight, texWidth, texHeight);

        Vector4 uv = UnityEngine.Sprites.DataUtility.GetInnerUV(sprite);
        float scaleX = uv.z - uv.x;
        float scaleY = uv.w - uv.y;

        Vector3 worldScale = transform.lossyScale;

        float pixelWidth = sprite.rect.width;
        float pixelHeight = sprite.rect.height;

        // Keep the exact original outline-width behavior with named constants
        float finalWidth = _outlineWidthNumerator / (pixelWidth + _outlineWidthPadding);

        _sr.GetPropertyBlock(_block);

        // Bind texture and per-sprite parameters on a per-instance property block
        _block.SetTexture(_mainTexExplicitId, sprite.texture);
        _block.SetVector(_TexelSizeId, texelSize);
        _block.SetVector(_customUVScaleId, new Vector4(scaleX, scaleY, 0f, 0f));
        _block.SetVector(_customWorldScaleId, new Vector4(worldScale.x, worldScale.y, 1f, 1f));
        _block.SetVector(_spritePixelSizeId, new Vector2(pixelWidth, pixelHeight));
        _block.SetFloat(_outlineWidthId, finalWidth);

        _sr.SetPropertyBlock(_block);
    }
}
