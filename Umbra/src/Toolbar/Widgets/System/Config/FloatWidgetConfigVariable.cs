namespace Umbra.Widgets;

public class FloatWidgetConfigVariable(
    string  id,
    string  name,
    string? description,
    float   defaultValue,
    float   minValue = float.MinValue,
    float   maxValue = float.MaxValue
)
    : WidgetConfigVariable<float>(id, name, description, defaultValue)
{
    public float MinValue { get; set; } = minValue;
    public float MaxValue { get; set; } = maxValue;

    /// <inheritdoc/>
    protected override float Sanitize(object? value)
    {
        try {
            float res = value switch {
                null       => 0,
                string str => !float.TryParse(str, out float result) ? 0 : result,
                _          => Convert.ToSingle(value)
            };

            return Math.Clamp(res, MinValue, MaxValue);
        } catch {
            return 0;
        }
    }
}
