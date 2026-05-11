void SpriteOutline_float(
    UnityTexture2D MainTex,
    float2 UV,
    float4 TexelSize,
    float4 OutlineColor,
    float OutlineSize,
    out float4 OutColor)
{
    float4 baseColor = SAMPLE_TEXTURE2D(MainTex.tex, MainTex.samplerstate, UV);

    float2 offset = TexelSize.xy * OutlineSize;

    float outlineAlpha = 0;

    outlineAlpha += SAMPLE_TEXTURE2D(MainTex.tex, MainTex.samplerstate, UV + float2(offset.x, 0)).a;
    outlineAlpha += SAMPLE_TEXTURE2D(MainTex.tex, MainTex.samplerstate, UV + float2(-offset.x, 0)).a;
    outlineAlpha += SAMPLE_TEXTURE2D(MainTex.tex, MainTex.samplerstate, UV + float2(0, offset.y)).a;
    outlineAlpha += SAMPLE_TEXTURE2D(MainTex.tex, MainTex.samplerstate, UV + float2(0, -offset.y)).a;

    outlineAlpha = saturate(outlineAlpha);

    float outlineMask = step(baseColor.a, 0.001) * step(0.001, outlineAlpha);

    float3 finalRgb = lerp(baseColor.rgb, OutlineColor.rgb, outlineMask);
    float finalAlpha = max(baseColor.a, outlineMask * OutlineColor.a);

    OutColor = float4(finalRgb, finalAlpha);
}