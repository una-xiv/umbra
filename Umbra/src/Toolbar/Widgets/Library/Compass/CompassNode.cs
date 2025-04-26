using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

internal sealed class CompassNode : Node
{
    /// <summary>
    /// Whether to use the camera angle for the compass.
    /// If false, the compass will use the player character heading.
    /// </summary>
    public bool UseCameraAngle { get; set; } = true;

    /// <summary>
    /// Defines how wide the compass is in degrees.
    /// </summary>
    public float AngleRangeDegrees { get; set; } = 225.0f;

    /// <summary>
    /// The power of the fish-eye distortion effect.
    ///
    /// A value of 1.0f means no distortion.
    /// A value more than 1.0f means more elements at the center.
    /// A value less than 1.0f means more elements at the edges.
    /// </summary>
    public float FishEyePower { get; set; } = 0.75f;

    /// <summary>
    /// How many pixels are required between 15 degree ticks.
    /// </summary>
    public float MinPixelsPer15DegTick { get; set; } = 4.0f;

    /// <summary>
    /// How many pixels are required between cardinal ticks.
    /// </summary>
    public float MinPixelsBetweenCardinals { get; set; } = 15.0f;

    /// <summary>
    /// Defines when the edge fade effect starts.
    /// </summary>
    public float FadeOutEdgeThresholdPixels { get; set; } = 30.0f;

    public uint LineColor       { get; set; } = 0xFFACFFAA;
    public uint TextColor       { get; set; } = 0xFFFFFFFF;
    public uint CenterLineColor { get; set; } = 0xFF0000FF;

    private readonly IPlayer     _player = Framework.Service<IPlayer>();
    private readonly IGameCamera _camera = Framework.Service<IGameCamera>();

    private static readonly Dictionary<int, string> CardinalData = new()
        { { 0, "N" }, { 90, "W" }, { 180, "S" }, { 270, "E" } };

    private const float Pi      = MathF.PI;
    private const float Rad2Deg = 180.0f / Pi;

    private static float GetScreenXForAngle(
        float targetAngleDegrees,
        float currentHeadingDegrees,
        float widgetCenterX,
        float widgetWidth,
        float viewAngleDegrees,
        float distortionPower)
    {
        if (widgetWidth <= 0) return widgetCenterX;

        float deltaDegrees                         = targetAngleDegrees - currentHeadingDegrees;
        while (deltaDegrees > 180f) deltaDegrees   -= 360f;
        while (deltaDegrees <= -180f) deltaDegrees += 360f;

        if (MathF.Abs(viewAngleDegrees) < 0.001f) viewAngleDegrees = 1.0f;

        float normalizedOffset = deltaDegrees / viewAngleDegrees;
        float distortedOffset  = MathF.Sign(normalizedOffset) * MathF.Pow(MathF.Abs(normalizedOffset), distortionPower);

        float halfPow                             = MathF.Pow(0.5f, distortionPower);
        if (MathF.Abs(halfPow) < 0.0001f) halfPow = 0.0001f;

        float scale       = (widgetWidth / 2.0f) / halfPow;
        float pixelOffset = distortedOffset * scale;

        return widgetCenterX - pixelOffset;
    }

    /// <summary>
    /// Modifies the alpha channel of a color based on a multiplier.
    /// </summary>
    /// <param name="color">The base color in AABBGGRR format.</param>
    /// <param name="alphaMultiplier">The alpha multiplier (0.0 = transparent, 1.0 = original alpha).</param>
    /// <returns>The new color with adjusted alpha, or the original if multiplier is >= 1.0.</returns>
    private static uint ApplyAlpha(uint color, float alphaMultiplier)
    {
        alphaMultiplier = Math.Clamp(alphaMultiplier, 0.0f, 1.0f);

        if (alphaMultiplier >= 0.999f) return color;
        if (alphaMultiplier <= 0.001f) return color & 0x00FFFFFF;

        return (color & 0x00FFFFFF) | ((uint)(((color >> 24) & 0xFF) * alphaMultiplier) << 24);
    }

    /// <summary>
    /// Calculates the alpha multiplier based on horizontal position for edge fade effect.
    /// </summary>
    /// <param name="screenX">The horizontal position of the element.</param>
    /// <param name="widgetX">The left edge of the widget.</param>
    /// <param name="widgetWidth">The width of the widget.</param>
    /// <param name="threshold">The distance from the edge where fade starts.</param>
    /// <returns>Alpha multiplier (0.0 to 1.0).</returns>
    private static float CalculateEdgeFadeAlpha(float screenX, float widgetX, float widgetWidth, float threshold)
    {
        if (threshold <= 0.01f) return 1.0f;

        float distanceFromLeftEdge  = screenX - widgetX;
        float distanceFromRightEdge = (widgetX + widgetWidth) - screenX;

        float distanceFromNearestEdge = MathF.Min(distanceFromLeftEdge, distanceFromRightEdge);

        if (distanceFromNearestEdge >= threshold) return 1.0f;
        if (distanceFromNearestEdge < 0) return 0.0f;

        return Math.Clamp(distanceFromNearestEdge / threshold, 0.0f, 1.0f);
    }


    protected override void OnDraw(ImDrawListPtr drawList)
    {
        var contentRect = Bounds.ContentRect;
        if (contentRect.Width <= 1 || contentRect.Height <= 1) return;

        float widgetX       = contentRect.TopLeft.X;
        float widgetY       = contentRect.TopLeft.Y;
        float widgetWidth   = contentRect.Width;
        float widgetHeight  = contentRect.Height;
        float widgetCenterX = widgetX + widgetWidth / 2.0f;
        float widgetCenterY = widgetY + widgetHeight / 2.0f;

        float playerAngleRad;
        float currentHeadingDegrees;

        if (UseCameraAngle) {
            playerAngleRad        = _camera.GetCameraAngle();
            currentHeadingDegrees = -(playerAngleRad * Rad2Deg + 90f - 360f) % 360f;
        } else {
            playerAngleRad        = _player.Rotation;
            currentHeadingDegrees = (playerAngleRad * Rad2Deg + 180f + 360f) % 360f;
        }

        float centerX0  = GetScreenXForAngle(currentHeadingDegrees, currentHeadingDegrees, widgetCenterX, widgetWidth, AngleRangeDegrees, FishEyePower);
        float centerX15 = GetScreenXForAngle(currentHeadingDegrees + 15f, currentHeadingDegrees, widgetCenterX, widgetWidth, AngleRangeDegrees, FishEyePower);

        float center15DegPixelSpan = MathF.Abs(centerX15 - centerX0);
        bool  show15DegTicks       = center15DegPixelSpan >= MinPixelsPer15DegTick;

        float minCardinalPixelSpan = float.MaxValue;
        float lastCardinalX        = -float.MaxValue;
        bool  firstCardinalFound   = false;

        for (int angleOffset = -180; angleOffset <= 180; angleOffset += 90) {
            float testAngle = (currentHeadingDegrees + angleOffset + 360f) % 360f;
            testAngle = MathF.Round(testAngle / 90f) * 90f;

            if (!CardinalData.ContainsKey((int)testAngle)) continue;
            
            float cardinalX = GetScreenXForAngle(testAngle, currentHeadingDegrees, widgetCenterX, widgetWidth, AngleRangeDegrees, FishEyePower);
            
            if (cardinalX >= widgetX - widgetWidth && cardinalX <= widgetX + widgetWidth * 2) {
                if (firstCardinalFound) minCardinalPixelSpan = MathF.Min(minCardinalPixelSpan, MathF.Abs(cardinalX - lastCardinalX));
                
                lastCardinalX      = cardinalX;
                firstCardinalFound = true;
            }
        }

        bool showCardinalText = minCardinalPixelSpan >= MinPixelsBetweenCardinals;

        drawList.PushClipRect(contentRect.TopLeft, contentRect.BottomRight, true);

        float centerAlphaMultiplier = CalculateEdgeFadeAlpha(widgetCenterX, widgetX, widgetWidth, FadeOutEdgeThresholdPixels);
        
        if (centerAlphaMultiplier > 0.001f) {
            uint  currentCenterColor = ApplyAlpha(CenterLineColor, centerAlphaMultiplier);
            float centerMarkHeight   = widgetHeight * 0.8f;
            float centerY1           = widgetCenterY - centerMarkHeight / 2f;
            float centerY2           = centerY1 + centerMarkHeight;
            drawList.AddLine(new(widgetCenterX, centerY1), new(widgetCenterX, centerY2),
                currentCenterColor, 1.5f);
        }

        try {
            for (int i = 0; i < 360; i += 15) {
                float markAngleDegrees   = i;
                bool  isCardinal         = CardinalData.ContainsKey(i);
                bool  isIntermediateTick = (i % 30 == 0) && !isCardinal;

                if (!isCardinal && !isIntermediateTick && !show15DegTicks) continue;
                if (isIntermediateTick && !show15DegTicks) continue;

                float screenX = GetScreenXForAngle(markAngleDegrees, currentHeadingDegrees, widgetCenterX, widgetWidth,
                    AngleRangeDegrees, FishEyePower);

                float alphaMultiplier = CalculateEdgeFadeAlpha(screenX, widgetX, widgetWidth, FadeOutEdgeThresholdPixels);
                if (alphaMultiplier <= 0.001f) continue;

                float tickHeight, tickWidth;
                if (isCardinal) {
                    tickHeight = widgetHeight * 0.8f;
                    tickWidth  = 0f;
                } else if (isIntermediateTick) {
                    tickHeight = widgetHeight * 0.45f;
                    tickWidth  = 1.0f;
                } else {
                    tickHeight = widgetHeight * 0.3f;
                    tickWidth  = 1.0f;
                }

                float tickY1 = widgetCenterY - (tickHeight / 2.0f);
                float tickY2 = tickY1 + tickHeight;

                if (tickWidth > 0) {
                    uint currentLineColor = ApplyAlpha(LineColor, alphaMultiplier);
                    drawList.AddLine(new(screenX, tickY1), new(screenX, tickY2), currentLineColor, tickWidth);
                }

                if (isCardinal && showCardinalText && CardinalData.TryGetValue(i, out string? letter)) {
                    Vector2 textSize = ImGui.CalcTextSize(letter);
                    float   textX    = screenX - (textSize.X / 2.0f);
                    float   textY    = widgetY + 4;
                    textX = Math.Clamp(textX, widgetX, widgetX + widgetWidth - textSize.X);

                    uint currentTextColor = ApplyAlpha(TextColor, alphaMultiplier);
                    drawList.AddText(new(textX, textY), currentTextColor, letter);
                }
            }
        } finally {
            drawList.PopClipRect();
        }
    }
}
