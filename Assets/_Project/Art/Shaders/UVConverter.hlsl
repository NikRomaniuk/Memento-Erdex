#ifndef UV_CONVERTER_INCLUDED
#define UV_CONVERTER_INCLUDED

// Shader Graph friendly UV conversion helpers for sprite-atlas workflows
// Expected source properties from SpriteShaderHelper:
// _CustomUVScale  -> sprite rect size in atlas UV space (xy)
// _CustomUVOffset -> sprite rect min corner in atlas UV space (xy)

float2 UVConverter_SafeScale(float2 uvScale)
{
    return max(abs(uvScale), float2(1e-6, 1e-6));
}

void LocalSpriteUVToSpriteSheetUV_float(
    float2 LocalSpriteUV,
    float2 SpriteUVScale,
    float2 SpriteUVOffset,
    out float2 SpriteSheetUV)
{
    SpriteSheetUV = LocalSpriteUV * SpriteUVScale + SpriteUVOffset;
}

void SpriteSheetUVToLocalSpriteUV_float(
    float2 SpriteSheetUV,
    float2 SpriteUVScale,
    float2 SpriteUVOffset,
    out float2 LocalSpriteUV)
{
    float2 safeScale = UVConverter_SafeScale(SpriteUVScale);
    LocalSpriteUV = (SpriteSheetUV - SpriteUVOffset) / safeScale;
}

// Variant that accepts Vector4 properties directly from SpriteShaderHelper
void LocalSpriteUVToSpriteSheetUV_FromVec4_float(
    float2 LocalSpriteUV,
    float4 CustomUVScale,
    float4 CustomUVOffset,
    out float2 SpriteSheetUV)
{
    LocalSpriteUVToSpriteSheetUV_float(LocalSpriteUV, CustomUVScale.xy, CustomUVOffset.xy, SpriteSheetUV);
}

// Variant that accepts Vector4 properties directly from SpriteShaderHelper
void SpriteSheetUVToLocalSpriteUV_FromVec4_float(
    float2 SpriteSheetUV,
    float4 CustomUVScale,
    float4 CustomUVOffset,
    out float2 LocalSpriteUV)
{
    SpriteSheetUVToLocalSpriteUV_float(SpriteSheetUV, CustomUVScale.xy, CustomUVOffset.xy, LocalSpriteUV);
}

void LocalSpriteUVToSpriteSheetUV_half(
    half2 LocalSpriteUV,
    half2 SpriteUVScale,
    half2 SpriteUVOffset,
    out half2 SpriteSheetUV)
{
    SpriteSheetUV = LocalSpriteUV * SpriteUVScale + SpriteUVOffset;
}

void SpriteSheetUVToLocalSpriteUV_half(
    half2 SpriteSheetUV,
    half2 SpriteUVScale,
    half2 SpriteUVOffset,
    out half2 LocalSpriteUV)
{
    half2 safeScale = max(abs(SpriteUVScale), half2(1e-6h, 1e-6h));
    LocalSpriteUV = (SpriteSheetUV - SpriteUVOffset) / safeScale;
}

void LocalSpriteUVToSpriteSheetUV_FromVec4_half(
    half2 LocalSpriteUV,
    half4 CustomUVScale,
    half4 CustomUVOffset,
    out half2 SpriteSheetUV)
{
    LocalSpriteUVToSpriteSheetUV_half(LocalSpriteUV, CustomUVScale.xy, CustomUVOffset.xy, SpriteSheetUV);
}

void SpriteSheetUVToLocalSpriteUV_FromVec4_half(
    half2 SpriteSheetUV,
    half4 CustomUVScale,
    half4 CustomUVOffset,
    out half2 LocalSpriteUV)
{
    SpriteSheetUVToLocalSpriteUV_half(SpriteSheetUV, CustomUVScale.xy, CustomUVOffset.xy, LocalSpriteUV);
}

#endif