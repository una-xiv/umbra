namespace Umbra.Common;

/// <summary>
/// Denotes a field in an enum that should be translated.
/// </summary>
/// <param name="key">The translation key or English translation if this enum does not need i18n translations.</param>
[AttributeUsage(AttributeTargets.Field)]
public class TranslationKeyAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}
