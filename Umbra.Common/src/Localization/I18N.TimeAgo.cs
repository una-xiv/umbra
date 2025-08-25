using System.Globalization;

namespace Umbra.Common;

public static partial class I18N
{
    public static string FormatTimeAgo(TimeSpan timeSpan, TimeAgoFormat format = TimeAgoFormat.Long)
    {
        switch (format) {
            case TimeAgoFormat.Long:
                return FormatTimeAgoLong(timeSpan);
            case TimeAgoFormat.Short:
                return FormatTimeAgoShort(timeSpan);
            case TimeAgoFormat.LocalTime12H:
                return FormatTimeAgoLocalTime12H(timeSpan);
            case TimeAgoFormat.LocalTime24H:
                return FormatTimeAgoLocalTime24H(timeSpan);
        }
        
        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    private static string FormatTimeAgoLocalTime12H(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("TimeAgo.Now");
        }

        DateTime targetTime = DateTime.Now.Add(timeSpan);
        return targetTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
    }
    
    private static string FormatTimeAgoLocalTime24H(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("TimeAgo.Now");
        }

        DateTime targetTime = DateTime.Now.Add(timeSpan);
        return targetTime.ToString("t"); // Short time pattern
    }
    
    private static string FormatTimeAgoShort(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
    
    private static string FormatTimeAgoLong(TimeSpan timeSpan)
    {
        if (timeSpan.TotalSeconds < 0) {
            return Translate("TimeAgo.Now");
        }

        timeSpan = timeSpan.Duration();

        if (timeSpan.TotalMinutes < 1) return Translate("TimeAgo.In") + " " + Translate("TimeAgo.AMinute");
        if (timeSpan.TotalHours < 1) return Translate("TimeAgo.In") + " " + Translate("TimeAgo.XMinutes", timeSpan.Minutes);
        
        switch (timeSpan)
        {
            case { TotalHours: >= 1, Minutes: 0 }:
                return Translate("TimeAgo.In") + " " + Translate("TimeAgo.AnHour");
            case { TotalHours: >= 1, Minutes: > 0 }:
            {
                string minutesPart = Translate("TimeAgo.XMinutes", timeSpan.Minutes);
                string hoursPart = timeSpan.Hours == 1
                    ? Translate("TimeAgo.AnHour")
                    : Translate("TimeAgo.XHours", timeSpan.Hours);

                return Translate("TimeAgo.In") + " " + hoursPart + " " + Translate("TimeAgo.And") + " " + minutesPart;
            }
            default:
                return Translate("TimeAgo.Always");
        }
    }
    
    public enum TimeAgoFormat
    {
        [TranslationKey("TimeAgo.Format.Long")]
        Long,
        
        [TranslationKey("TimeAgo.Format.Short")]
        Short,
        
        [TranslationKey("TimeAgo.Format.LocalTime12h")]
        LocalTime12H,
        
        [TranslationKey("TimeAgo.Format.LocalTime24h")]
        LocalTime24H,
    }
}
