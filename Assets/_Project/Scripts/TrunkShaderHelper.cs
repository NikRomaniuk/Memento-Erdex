using UnityEngine;

[ExecuteInEditMode]
public class TrunkShaderHelper : MonoBehaviour
{
    public Texture2D uniqueBark;
    public Texture2D normalMap;
    [Range(0, 1)] public float blendFactor = 1f;

    private static readonly int BarkTexID = Shader.PropertyToID("_OverlayTex");
    private static readonly int NormalID = Shader.PropertyToID("_NormalMap");
    private static readonly int BlendID = Shader.PropertyToID("_BlendAmount");

    private MaterialPropertyBlock _propBlock;
    private Renderer _renderer;

    void OnValidate() => UpdateProperties();

    public void UpdateProperties()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        // Get the current property block
        _renderer.GetPropertyBlock(_propBlock);
        
        if(uniqueBark != null) _propBlock.SetTexture(BarkTexID, uniqueBark);
        if(normalMap != null) _propBlock.SetTexture(NormalID, normalMap);
        //float autoBlend = Mathf.Clamp01(transform.localPosition.y * 0.1f); 
        _propBlock.SetFloat(BlendID, blendFactor);
        
        // Apply the updated property block to the shader
        _renderer.SetPropertyBlock(_propBlock);
    }
}
