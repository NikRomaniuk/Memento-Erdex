#ifndef SHARP_BILINEAR_INCLUDED
#define SHARP_BILINEAR_INCLUDED

void RetroAA_float(
    UnityTexture2D tex,
    float2 uv,
    float4 texelSize,
    out float4 color)
{
    float2 texelCoord = uv * texelSize.zw;
    float2 hfw = 0.5 * fwidth(texelCoord);
    float2 fl = floor(texelCoord - 0.5) + 0.5;
    float2 uvaa = (fl + smoothstep(0.5 - hfw, 0.5 + hfw, texelCoord - fl)) * texelSize.xy;

    color = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate, uvaa);
}

#endif