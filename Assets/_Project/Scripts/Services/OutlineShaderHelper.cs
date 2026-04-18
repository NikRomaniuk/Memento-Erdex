using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class OutlineShaderHelper : MonoBehaviour
{
    private static readonly int _mainTexExplicitId = Shader.PropertyToID("_MainTexExplicit");
    private static readonly int _TexelSizeId = Shader.PropertyToID("_TexelSize");
    private static readonly int _customUVScaleId = Shader.PropertyToID("_CustomUVScale");
    private static readonly int _customUVOffsetId = Shader.PropertyToID("_CustomUVOffset");
    private static readonly int _customWorldScaleId = Shader.PropertyToID("_CustomWorldScale");
    private static readonly int _spritePixelSizeId = Shader.PropertyToID("_SpritePixelSize");
    private static readonly int _outlineWidthId = Shader.PropertyToID("_Outline_Width");
    private static readonly int _outlineColorId = Shader.PropertyToID("_Outline_Color");

    private const float _horizontalBorderSumPixels = 16f;

    [Header("Properties")]
    [SerializeField] private float _outlineWidth = 1f;
    [SerializeField] private Color _outlineColor = Color.white;
    [SerializeField] private Vector3 _worldScale;

    private SpriteRenderer _sr;
    private MaterialPropertyBlock _block;

    private void OnEnable()
    {
        _sr = GetComponent<SpriteRenderer>();
        _block ??= new MaterialPropertyBlock();
    }

    public void SetOutlineColor(Color outlineColor)
    {
        _outlineColor = outlineColor;
        ApplyOutlineProperties();
    }

    private void LateUpdate()
    {
        ApplyOutlineProperties();
    }

    private void ApplyOutlineProperties()
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
        float offsetX = uv.x;
        float offsetY = uv.y;

        Vector3 worldScale = transform.lossyScale;
        _worldScale = worldScale;

        float pixelWidth = sprite.textureRect.width - _horizontalBorderSumPixels;
        float pixelHeight = sprite.textureRect.height - _horizontalBorderSumPixels;

        _sr.GetPropertyBlock(_block);

        _block.SetTexture(_mainTexExplicitId, sprite.texture);
        _block.SetVector(_TexelSizeId, texelSize);
        _block.SetVector(_customUVScaleId, new Vector4(scaleX, scaleY, 0f, 0f));
        _block.SetVector(_customUVOffsetId, new Vector4(offsetX, offsetY, 0f, 0f));
        _block.SetVector(_customWorldScaleId, new Vector4(worldScale.x, worldScale.y, 1f, 1f));
        _block.SetVector(_spritePixelSizeId, new Vector2(pixelWidth, pixelHeight));
        _block.SetFloat(_outlineWidthId, _outlineWidth);
        _block.SetColor(_outlineColorId, _outlineColor);

        _sr.SetPropertyBlock(_block);
    }
}
