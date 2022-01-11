// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

using System;

namespace iluvadev.ConsoleProgressBar.Extensions
{
    /// <summary>
    /// TimeSpan extensions
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Gets a textual Sumarized for remaining time: X days, or Y hours, or Z minutes, etc.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ToStringAsSumarizedRemainingText(this TimeSpan? ts)
            => ts.HasValue ? ToStringAsSumarizedRemainingText(ts.Value) : "unknown";

        /// <summary>
        /// Gets a textual Sumarized for remaining time: X days, or Y hours, or Z minutes, etc.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ToStringAsSumarizedRemainingText(this TimeSpan ts)
        {
            int units;
            if ((units = Convert.ToInt32(Math.Round(ts.TotalDays))) > 1) return $"{units} days";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalDays))) == 1) return $"a day";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalHours))) > 1) return $"{units} hours";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalHours))) == 1) return $"an hour";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalMinutes))) > 1) return $"{units} minutes";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalMinutes))) == 1) return $"a minute";
            else if ((units = Convert.ToInt32(Math.Round(ts.TotalSeconds))) > 1) return $"{units} seconds";
            else if ((Convert.ToInt32(Math.Floor(ts.TotalSeconds))) == 1) return $"a second";
            else return "a moment";
        }

        /// <summary>
        /// Converts a TimeSpan to String, showing all hours
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="includeMilliseconds"></param>
        /// <returns></returns>
        public static string ToStringWithAllHours(this TimeSpan? ts, bool includeMilliseconds = true)
            => ts.HasValue ? ToStringWithAllHours(ts.Value, includeMilliseconds) 
                           : "unknown";

        /// <summary>
        /// Converts a TimeSpan to String, showing all hours
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="includeMilliseconds"></param>
        /// <returns></returns>
        public static string ToStringWithAllHours(this TimeSpan ts, bool includeMilliseconds = true)
            => includeMilliseconds ? $"{ts.TotalHours:F0}{ts:\\:mm\\:ss\\.fff}"
                                   : $"{ts.TotalHours:F0}{ts:\\:mm\\:ss}";
    }
}
