using System.Globalization;

namespace Umbra.Common;

public static partial class I18N
{
        public static string FormatTimeLeft(TimeSpan timeSpan, TimeFormat format = TimeFormat.Long)
    {
        switch (format) {
            case TimeFormat.Long:
                return FormatTimeLeftLong(timeSpan);
            default:
                return FormatTime(timeSpan, format);
        }
    }

    public static string FormatTime(TimeSpan timeSpan, TimeFormat format = TimeFormat.Long)
    {
        switch (format) {
            case TimeFormat.Long:
                return FormatTimeAgoLong(timeSpan);
            case TimeFormat.Short:
                return FormatTimeShort(timeSpan);
            case TimeFormat.LocalTime12H:
                return FormatTimeLocalTime12H(timeSpan);
            case TimeFormat.LocalTime24H:
                return FormatTimeLocalTime24H(timeSpan);
        }

        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    private static string FormatTimeLocalTime12H(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("Time.Now");
        }

        DateTime targetTime = DateTime.Now.Add(timeSpan);
        return targetTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
    }

    private static string FormatTimeLocalTime24H(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("Time.Now");
        }

        DateTime targetTime = DateTime.Now.Add(timeSpan);
        return targetTime.ToString("HH:mm");
    }

    private static string FormatTimeShort(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    private static string FormatTimeAgoLong(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("Time.Now");
        }

        timeSpan = timeSpan.Duration();

        if (timeSpan.TotalMinutes < 1) return Translate("Time.In") + " " + Translate("Time.AMinute");
        if (timeSpan.TotalHours < 1) return Translate("Time.In") + " " + Translate("Time.XMinutes", timeSpan.Minutes);

        switch (timeSpan)
        {
            case { TotalHours: >= 1, Minutes: 0 }:
                return Translate("Time.In") + " " + Translate("Time.AnHour");
            case { TotalHours: >= 1, Minutes: > 0 }:
            {
                string minutesPart = Translate("Time.XMinutes", timeSpan.Minutes);
                string hoursPart = timeSpan.Hours == 1
                    ? Translate("Time.AnHour")
                    : Translate("Time.XHours", timeSpan.Hours);

                return Translate("Time.In") + " " + hoursPart + " " + Translate("Time.And") + " " + minutesPart;
            }
            default:
                return Translate("Time.Always");
        }
    }

    private static string FormatTimeLeftLong(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("Time.Now");
        }

        timeSpan = timeSpan.Duration();

        if (timeSpan.TotalMinutes < 1) return Translate("Time.XLeft", Translate("Time.AMinute"));
        if (timeSpan.TotalHours < 1) return Translate("Time.XLeft", Translate("Time.XMinutes", timeSpan.Minutes));

        switch (timeSpan)
        {
            case { TotalHours: >= 1, Minutes: 0 }:
                return Translate("Time.XLeft", Translate("Time.AnHour"));
            case { TotalHours: >= 1, Minutes: > 0 }:
            {
                string minutesPart = Translate("Time.XMinutes", timeSpan.Minutes);
                string hoursPart = timeSpan.Hours == 1
                    ? Translate("Time.AnHour")
                    : Translate("Time.XHours", timeSpan.Hours);

                return Translate("Time.XLeft", hoursPart + " " + Translate("Time.And") + " " + minutesPart);
            }
            default:
                return Translate("Time.Always");
        }
    }

    public enum TimeFormat
    {
        [TranslationKey("Time.Format.Long")]
        Long,

        [TranslationKey("Time.Format.Short")]
        Short,

        [TranslationKey("Time.Format.LocalTime12h")]
        LocalTime12H,

        [TranslationKey("Time.Format.LocalTime24h")]
        LocalTime24H,
    }
}
