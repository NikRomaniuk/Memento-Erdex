using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShaderHelper : MonoBehaviour
{
    private static readonly int _mainTexExplicitId = Shader.PropertyToID("_MainTexExplicit");
    private static readonly int _TexelSizeId = Shader.PropertyToID("_TexelSize");

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

        _sr.GetPropertyBlock(_block);

        // Bind only the properties used by the shader custom function
        _block.SetTexture(_mainTexExplicitId, texture);
        _block.SetVector(_TexelSizeId, texelSize);

        _sr.SetPropertyBlock(_block);
    }
}
