using System;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Components;

public class SimpleColorThemeEditor : UdtNode
{
    private SimpleColorPickerNode WindowsPicker => QuerySelector<SimpleColorPickerNode>("#windows")!;
    private SimpleColorPickerNode WidgetsPicker => QuerySelector<SimpleColorPickerNode>("#widgets")!;
    private SimpleColorPickerNode InputsPicker  => QuerySelector<SimpleColorPickerNode>("#inputs")!;
    private SimpleColorPickerNode AccentPicker  => QuerySelector<SimpleColorPickerNode>("#accent")!;

    public SimpleColorThemeEditor() : base("umbra.windows.settings.components.color_editor_simple.xml")
    {
        UmbraColors.OnColorProfileChanged += OnColorProfileChanged;
        OnColorProfileChanged(string.Empty);

        WindowsPicker.OnValueChanged += OnWindowColorChanged;
        WidgetsPicker.OnValueChanged += OnWidgetsColorChanged;
        InputsPicker.OnValueChanged  += OnInputsColorChanged;
        AccentPicker.OnValueChanged += OnAccentColorChanged;
    }

    protected override void OnDisposed()
    {
        UmbraColors.OnColorProfileChanged -= OnColorProfileChanged;
    }

    private void OnColorProfileChanged(string _)
    {
        WindowsPicker.Value = Color.GetNamedColor("Window.Background");
        WidgetsPicker.Value = Color.GetNamedColor("Widget.Background");
        InputsPicker.Value  = Color.GetNamedColor("Input.Background");
        AccentPicker.Value  = Color.GetNamedColor("Window.AccentColor");
    }

    private void OnAccentColorChanged(uint color)
    {
        Color.AssignByName("Window.AccentColor", color);
        UmbraColors.UpdateCurrentProfile();
    }
    
    private void OnWindowColorChanged(uint color)
    {
        Logger.Info($"Window Color Changed: {color}");

        uint textColor    = GetContrastingTextColor(color);
        uint outlineColor = SetAlpha(GetContrastingTextColor(textColor), (byte)(IsBright(textColor) ? 150 : 1));

        Color.AssignByName("Window.Background", color);
        Color.AssignByName("Window.BackgroundLight", ShadeTowards(color, textColor, 0.10D));
        Color.AssignByName("Window.Border", ShadeTowards(color, textColor, 0.30D));
        Color.AssignByName("Window.TitlebarBackground", color);
        Color.AssignByName("Window.TitlebarBorder", ShadeTowards(color, textColor, 0.25D));
        Color.AssignByName("Window.TitlebarGradient1", ShadeTowards(color, textColor, 0.1D));
        Color.AssignByName("Window.TitlebarGradient2", color);
        Color.AssignByName("Window.TitlebarText", textColor);
        Color.AssignByName("Window.TitlebarTextOutline", outlineColor);
        Color.AssignByName("Window.TitlebarCloseButton", color);
        Color.AssignByName("Window.TitlebarCloseButtonBorder", ShadeTowards(color, textColor, 0.45D));
        Color.AssignByName("Window.TitlebarCloseButtonHover", OffsetColor(ShadeTowards(color, textColor, 0.45D), 25, 0, 0));
        Color.AssignByName("Window.TitlebarCloseButtonX", textColor);
        Color.AssignByName("Window.TitlebarCloseButtonXHover", Lighter(textColor, 1.25D));
        Color.AssignByName("Window.TitlebarCloseButtonXOutline", outlineColor);
        Color.AssignByName("Window.ScrollbarTrack", ShadeTowards(color, textColor, 0.10D));
        Color.AssignByName("Window.ScrollbarThumb", ShadeTowards(color, textColor, 0.45D));
        Color.AssignByName("Window.ScrollbarThumbHover", ShadeTowards(color, textColor, 0.70D));
        Color.AssignByName("Window.ScrollbarThumbActive", ShadeTowards(color, textColor, 0.90D));
        Color.AssignByName("Window.Text", textColor);
        Color.AssignByName("Window.TextLight", Lighter(textColor, 1.25D));
        Color.AssignByName("Window.TextMuted", ShadeTowards(color, textColor, 0.90D));
        Color.AssignByName("Window.TextOutline", outlineColor);
        Color.AssignByName("Window.TextDisabled", ShadeTowards(color, textColor, 0.6D));
        
        UmbraColors.UpdateCurrentProfile();
    }

    private void OnWidgetsColorChanged(uint color)
    {
        uint textColor     = GetContrastingTextColor(color);
        uint outlineColor  = SetAlpha(GetContrastingTextColor(textColor), (byte)(IsBright(textColor) ? 150 : 1));
        uint accentBg      = Color.GetNamedColor("Window.AccentColor");
        uint accentText    = GetContrastingTextColor(accentBg);
        uint accentOutline = SetAlpha(GetContrastingTextColor(accentText), (byte)(IsBright(accentText) ? 150 : 1));

        uint bg1    = color;
        uint bg2    = Darker(color);
        uint border = ShadeTowards(color, textColor, 0.30D);

        // Separate;
        // Color.AssignByName("Widget.ProgressBar",                  0xA0accef9);
        // Color.AssignByName("Widget.ProgressBarOverflow",          0xC0accef9);

        Color.AssignByName("Toolbar.InactiveBackground1", SetAlpha(bg1, 0xC0));
        Color.AssignByName("Toolbar.InactiveBackground2", SetAlpha(bg2, 0xC0));
        Color.AssignByName("Toolbar.Background1", bg1);
        Color.AssignByName("Toolbar.Background2", bg2);
        Color.AssignByName("Toolbar.InactiveBorder", SetAlpha(border, 0xA0));
        Color.AssignByName("Toolbar.Border", border);
        Color.AssignByName("Widget.Background", bg2);
        Color.AssignByName("Widget.BackgroundDisabled", ShadeTowards(color, textColor, 0.30D));
        Color.AssignByName("Widget.BackgroundHover", ShadeTowards(color, textColor, 0.30D));
        Color.AssignByName("Widget.Border", border);
        Color.AssignByName("Widget.BorderDisabled", ShadeTowards(color, border, 0.30D));
        Color.AssignByName("Widget.BorderHover", ShadeTowards(border, textColor, 0.30D));
        Color.AssignByName("Widget.Text", textColor);
        Color.AssignByName("Widget.TextDisabled", ShadeTowards(textColor, color, 0.4D));
        Color.AssignByName("Widget.TextHover", Lighter(textColor));
        Color.AssignByName("Widget.TextMuted", ShadeTowards(textColor, color, 0.25D));
        Color.AssignByName("Widget.TextOutline", outlineColor);
        Color.AssignByName("Widget.PopupBackground", bg2);
        Color.AssignByName("Widget.PopupBackground.Gradient1", bg1);
        Color.AssignByName("Widget.PopupBackground.Gradient2", bg2);
        Color.AssignByName("Widget.PopupBorder", border);
        Color.AssignByName("Widget.PopupMenuText", textColor);
        Color.AssignByName("Widget.PopupMenuTextMuted", ShadeTowards(textColor, color, 0.25D));
        Color.AssignByName("Widget.PopupMenuTextDisabled", ShadeTowards(textColor, color, 0.40D));
        Color.AssignByName("Widget.PopupMenuTextHover", accentText);
        Color.AssignByName("Widget.PopupMenuBackgroundHover", accentBg);
        Color.AssignByName("Widget.PopupMenuTextOutline", accentOutline);
        Color.AssignByName("Widget.PopupMenuTextOutlineHover", accentOutline);
        Color.AssignByName("Widget.PopupMenuTextOutlineDisabled", 0);
        
        UmbraColors.UpdateCurrentProfile();
    }

    private void OnInputsColorChanged(uint color)
    {
        uint textColor    = GetContrastingTextColor(color);
        uint outlineColor = SetAlpha(GetContrastingTextColor(textColor), (byte)(IsBright(textColor) ? 150 : 1));

        Color.AssignByName("Input.Background", color);
        Color.AssignByName("Input.Border", ShadeTowards(color, textColor, 0.30D));
        Color.AssignByName("Input.Text", textColor);
        Color.AssignByName("Input.TextMuted", ShadeTowards(color, textColor, 0.80D));
        Color.AssignByName("Input.TextOutline", outlineColor);
        Color.AssignByName("Input.BackgroundHover", ShadeTowards(color, textColor, 0.15D));
        Color.AssignByName("Input.BorderHover", ShadeTowards(color, textColor, 0.60D));
        Color.AssignByName("Input.TextHover", Lighter(textColor, 1.25D));
        Color.AssignByName("Input.TextOutlineHover", outlineColor);
        Color.AssignByName("Input.BackgroundDisabled", ShadeTowards(color, textColor, 0.20D));
        Color.AssignByName("Input.BorderDisabled", ShadeTowards(color, textColor, 0.30D));
        Color.AssignByName("Input.TextDisabled", ShadeTowards(color, textColor, 0.50D));
        Color.AssignByName("Input.TextOutlineDisabled", outlineColor);
        
        UmbraColors.UpdateCurrentProfile();
    }

    // --- Constants for Black and White in AABBGGRR ---
    public const uint Black = 0xFF000000; // Solid Black
    public const uint White = 0xFFFFFFFF; // Solid White

    // --- Component Extraction and Creation ---

    /// <summary>
    /// Extracts the Alpha, Red, Green, and Blue components from an AABBGGRR uint color.
    /// </summary>
    /// <param name="color">The color in AABBGGRR format.</param>
    /// <param name="a">Outputs the Alpha component (0-255).</param>
    /// <param name="r">Outputs the Red component (0-255).</param>
    /// <param name="g">Outputs the Green component (0-255).</param>
    /// <param name="b">Outputs the Blue component (0-255).</param>
    public static void GetComponents(uint color, out byte a, out byte r, out byte g, out byte b)
    {
        a = (byte)((color >> 24) & 0xFF);
        b = (byte)((color >> 16) & 0xFF); // Note: B is before G in AABBGGRR
        g = (byte)((color >> 8) & 0xFF);
        r = (byte)(color & 0xFF);
    }

    /// <summary>
    /// Creates an AABBGGRR uint color from Alpha, Red, Green, and Blue components.
    /// </summary>
    /// <param name="a">Alpha component (0-255).</param>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <returns>The color in AABBGGRR format.</returns>
    public static uint FromComponents(byte a, byte r, byte g, byte b)
    {
        return ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r;
    }

    /// <summary>
    /// Calculates the perceived luminance of a color.
    /// Uses the formula Y = (R*299 + G*587 + B*114) / 1000.
    /// </summary>
    /// <param name="r">Red component (0-255).</param>
    /// <param name="g">Green component (0-255).</param>
    /// <param name="b">Blue component (0-255).</param>
    /// <returns>Luminance value (approx. 0-255).</returns>
    private static double CalculateLuminance(byte r, byte g, byte b)
    {
        // Formula for perceived brightness (YIQ)
        return (r * 299 + g * 587 + b * 114) / 1000.0;
    }

    /// <summary>
    /// Determines if a color is perceived as bright based on its luminance.
    /// </summary>
    /// <param name="color">The color to check in AABBGGRR format.</param>
    /// <param name="luminanceThreshold">The luminance threshold (0-255) to distinguish bright from dark. Colors at or above this value are considered bright. Defaults to 149.0 for consistency with GetContrastingTextColor.</param>
    /// <returns>True if the color's luminance is greater than or equal to the threshold (considered bright), false otherwise (considered dark).</returns>
    public static bool IsBright(uint color, double luminanceThreshold = 149.0)
    {
        // Extract RGB components (Alpha is not needed for luminance)
        GetComponents(color, out _, out byte r, out byte g, out byte b);

        // Calculate luminance using the same internal helper
        double luminance = CalculateLuminance(r, g, b);

        // Compare with the threshold
        return luminance >= luminanceThreshold;
    }

    /// <summary>
    /// Determines the best contrasting text color (solid black or solid white) for a given background color.
    /// </summary>
    /// <param name="backgroundColor">The background color in AABBGGRR format.</param>
    /// <param name="luminanceThreshold">The luminance threshold (0-255) to switch between black and white text. Default is 149.</param>
    /// <returns>Either ColorUtils.Black (0xFF000000) or ColorUtils.White (0xFFFFFFFF).</returns>
    public static uint GetContrastingTextColor(uint backgroundColor, double luminanceThreshold = 149.0)
    {
        GetComponents(backgroundColor, out _, out byte r, out byte g, out byte b);
        double luminance = CalculateLuminance(r, g, b);

        // If the background is bright (luminance >= threshold), use black text.
        // Otherwise (background is dark), use white text.
        return luminance >= luminanceThreshold ? Black : White;
    }

    /// <summary>
    /// Adjusts the brightness of a color by a multiplicative factor.
    /// Preserves the original alpha channel.
    /// </summary>
    /// <param name="color">The original color in AABBGGRR format.</param>
    /// <param name="factor">Brightness adjustment factor. > 1.0 makes it lighter, < 1.0 makes it darker. 1.0 leaves it unchanged.</param>
    /// <returns>The adjusted color in AABBGGRR format.</returns>
    public static uint AdjustBrightness(uint color, double factor)
    {
        if (factor < 0) factor = 0; // Factor cannot be negative

        GetComponents(color, out byte a, out byte r, out byte g, out byte b);

        // Adjust each color channel, clamping the result between 0 and 255
        byte newR = (byte)Math.Max(0, Math.Min(255, Math.Round(r * factor)));
        byte newG = (byte)Math.Max(0, Math.Min(255, Math.Round(g * factor)));
        byte newB = (byte)Math.Max(0, Math.Min(255, Math.Round(b * factor)));

        return FromComponents(a, newR, newG, newB);
    }

    /// <summary>
    /// Generates a lighter shade of the given color.
    /// </summary>
    /// <param name="color">The base color in AABBGGRR format.</param>
    /// <param name="amount">How much lighter to make it (e.g., 1.2 for 20% lighter). Must be >= 1.0.</param>
    /// <returns>The lighter color shade.</returns>
    public static uint Lighter(uint color, double amount = 1.2)
    {
        if (amount < 1.0) amount = 1.0; // Ensure it makes it lighter or same
        return AdjustBrightness(color, amount);
    }

    /// <summary>
    /// Generates a darker shade of the given color.
    /// </summary>
    /// <param name="color">The base color in AABBGGRR format.</param>
    /// <param name="amount">How much darker to make it (e.g., 0.8 for 20% darker). Must be <= 1.0.</param>
    /// <returns>The darker color shade.</returns>
    public static uint Darker(uint color, double amount = 0.8)
    {
        if (amount > 1.0) amount = 1.0; // Ensure it makes it darker or same
        return AdjustBrightness(color, amount);
    }

    /// <summary>
    /// Offsets the R, G, B components of a color by specific integer amounts.
    /// Preserves the original alpha channel. Clamps results between 0 and 255.
    /// </summary>
    /// <param name="color">The original color in AABBGGRR format.</param>
    /// <param name="offsetR">Amount to add to the Red component (-255 to 255).</param>
    /// <param name="offsetG">Amount to add to the Green component (-255 to 255).</param>
    /// <param name="offsetB">Amount to add to the Blue component (-255 to 255).</param>
    /// <returns>The offset color in AABBGGRR format.</returns>
    public static uint OffsetColor(uint color, int offsetR, int offsetG, int offsetB)
    {
        GetComponents(color, out byte a, out byte r, out byte g, out byte b);

        // Calculate new values and clamp them within the 0-255 range
        int newRInt = r + offsetR;
        int newGInt = g + offsetG;
        int newBInt = b + offsetB;

        byte newR = (byte)Math.Max(0, Math.Min(255, newRInt));
        byte newG = (byte)Math.Max(0, Math.Min(255, newGInt));
        byte newB = (byte)Math.Max(0, Math.Min(255, newBInt));

        return FromComponents(a, newR, newG, newB);
    }

    /// <summary>
    /// Overrides the alpha component of the given color.
    /// </summary>
    /// <param name="color">The original color in AABBGGRR format.</param>
    /// <param name="alpha">The alpha component (0 to 255)</param>
    /// <returns>The updated color in AABBGGRR format.</returns>
    public static uint SetAlpha(uint color, byte alpha)
    {
        GetComponents(color, out _, out byte r, out byte g, out byte b);
        return FromComponents(alpha, r, g, b);
    }

    /// <summary>
    /// Shades a color towards a target color using linear interpolation (lerp).
    /// This is useful for creating "muted" or "disabled" versions of a text color
    /// relative to its background. Preserves the alpha channel of the original colorToShade.
    /// </summary>
    /// <param name="colorToShade">The starting color (e.g., text color) in AABBGGRR format.</param>
    /// <param name="targetColor">The color to shade towards (e.g., background color) in AABBGGRR format.</param>
    /// <param name="factor">Interpolation factor (range 0.0 to 1.0).
    /// 0.0 returns colorToShade unchanged.
    /// 1.0 returns targetColor (RGB components), keeping original alpha.
    /// Values between 0 and 1 move colorToShade towards targetColor.
    /// Typical values for shading: 0.2 (Subtle), 0.4 (Muted), 0.6 (Disabled). </param>
    /// <returns>The shaded color in AABBGGRR format.</returns>
    public static uint ShadeTowards(uint colorToShade, uint targetColor, double factor)
    {
        // Clamp factor to the valid range [0, 1] to prevent unexpected results
        factor = Math.Max(0.0, Math.Min(1.0, factor));

        GetComponents(colorToShade, out byte startA, out byte startR, out byte startG, out byte startB);
        // We only need the RGB of the target color for interpolation
        GetComponents(targetColor, out _, out byte targetR, out byte targetG, out byte targetB);

        // Linearly interpolate each color channel
        // Formula: start + (end - start) * factor
        byte newR = (byte)Math.Round(startR + (targetR - startR) * factor);
        byte newG = (byte)Math.Round(startG + (targetG - startG) * factor);
        byte newB = (byte)Math.Round(startB + (targetB - startB) * factor);

        // Note: Clamping RGB results within 0-255 after interpolation isn't strictly
        // necessary if the factor is [0,1] and inputs are valid bytes,
        // but Math.Round could theoretically push edge cases. For safety:
        newR = Math.Max((byte)0, Math.Min((byte)255, newR));
        newG = Math.Max((byte)0, Math.Min((byte)255, newG));
        newB = Math.Max((byte)0, Math.Min((byte)255, newB));


        // Combine using the original alpha from colorToShade and the new RGB values
        return FromComponents(startA, newR, newG, newB);
    }
}
