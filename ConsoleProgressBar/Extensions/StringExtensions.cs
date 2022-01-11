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
    /// Extensions for String
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string that occupy all console line/s
        /// </summary>
        /// <param name="value">The string to write in console</param>
        /// <param name="allowMultipleLines">To allow print the string in muliple lines or only in one:
        ///     True: The text can be represented in more than one Console line (fill spaces to the end of last line)
        ///     False: The text must be represented in only ONE line (truncate to fit or fill spaces to the end of line)
        /// </param>
        /// <returns></returns>
        public static string AdaptToConsole(this string value, bool allowMultipleLines = true)
        {
            int maxWidth = Console.BufferWidth;

            if (allowMultipleLines)
                maxWidth *= Math.DivRem(value.Length, maxWidth, out _) + 1;

            return AdaptToMaxWidth(value, maxWidth);
        }

        /// <summary>
        /// Returns a string with exactly maxChars: Truncates string value or fill with spaces to fits exact length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxWidth"></param>
        /// <param name="append">Text appended when it is truncated. Default: "..."</param>
        /// <returns></returns>
        public static string AdaptToMaxWidth(this string value, int maxWidth, string append = "...")
        {
            value ??= "";
            int len = value.Length;

            if (maxWidth <= 0) return "";
            else if (len == maxWidth) return value;
            else if (len < maxWidth) return value.PadRight(maxWidth);
            else return value[..(maxWidth - append.Length)] + append;
        }
    }
}
