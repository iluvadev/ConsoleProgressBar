using System;

namespace ConsoleProgressBar
{
    public static class TimeSpanExtensions
    {
        public static string ToStringWithAllHours(this TimeSpan value, bool includeMilliseconds = false)
            => $"{value.TotalHours:F0}{value:\\:mm\\:ss}{(includeMilliseconds ? value.ToString("\\.fff") : null)}";
    }
}
