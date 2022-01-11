// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

using iluvadev.ConsoleProgressBar.Extensions;
using System;

namespace iluvadev.ConsoleProgressBar
{
    public partial class Text
    {
        /// <summary>
        /// Definition for the Texts in a ProgressBar
        /// </summary>
        public class TextBody
        {
            /// <summary>
            /// Text in Body definition when ProgressBar is "Processing"
            /// </summary>
            public Element<string> Processing { get; } = new Element<string>();

            /// <summary>
            /// Text in Body definition when ProgressBar is "Paused"
            /// </summary>
            public Element<string> Paused { get; } = new Element<string>();

            /// <summary>
            /// Text in Body definition when ProgressBar is "Done"
            /// </summary>
            public Element<string> Done { get; } = new Element<string>();

            /// <summary>
            /// Sets the Body Text visibility
            /// </summary>
            /// <param name="show"></param>
            /// <returns></returns>
            public TextBody SetVisible(bool show) 
                => SetVisible(pb => show);

            /// <summary>
            /// Sets the Body Text visibility
            /// </summary>
            /// <param name="showGetter"></param>
            /// <returns></returns>
            public TextBody SetVisible(Func<ProgressBar, bool> showGetter)
            {
                Processing.SetVisible(showGetter);
                Paused.SetVisible(showGetter);
                Done.SetVisible(showGetter);
                return this;
            }

            /// <summary>
            /// Sets the Body Text definition in all ProgressBar states ("Processing", "Paused", "Done")
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public TextBody SetValue(string value) 
                => SetValue(pb => value);

            /// <summary>
            /// Sets the Body Text definition in all ProgressBar states ("Processing", "Paused", "Done")
            /// </summary>
            /// <param name="valueGetter"></param>
            /// <returns></returns>
            public TextBody SetValue(Func<ProgressBar, string> valueGetter)
            {
                Processing.SetValue(valueGetter);
                Paused.SetValue(valueGetter);
                Done.SetValue(valueGetter);
                return this;
            }

            /// <summary>
            /// Sets the Body Text Foreground Color
            /// </summary>
            /// <param name="foregroundColor"></param>
            /// <returns></returns>
            public TextBody SetForegroundColor(ConsoleColor foregroundColor) 
                => SetForegroundColor(pb => foregroundColor);

            /// <summary>
            /// Sets the Body Text Foreground Color
            /// </summary>
            /// <param name="foregroundColorGetter"></param>
            /// <returns></returns>
            public TextBody SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                Processing.SetForegroundColor(foregroundColorGetter);
                Paused.SetForegroundColor(foregroundColorGetter);
                Done.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the Body Text Background Color
            /// </summary>
            /// <param name="backgroundColor"></param>
            /// <returns></returns>
            public TextBody SetBackgroundColor(ConsoleColor backgroundColor) 
                => SetBackgroundColor(pb => backgroundColor);

            /// <summary>
            /// Sets the Body Text Background Color
            /// </summary>
            /// <param name="backgroundColorGetter"></param>
            /// <returns></returns>
            public TextBody SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                Processing.SetBackgroundColor(backgroundColorGetter);
                Paused.SetBackgroundColor(backgroundColorGetter);
                Done.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            /// <summary>
            /// Ctor
            /// </summary>
            public TextBody()
            {
                Processing.SetValue(pb => pb.HasProgress ?
                        $"{pb.Value} of {pb.Maximum} in {pb.TimeProcessing.ToStringWithAllHours()}, remaining: {pb.TimeRemaining.ToStringAsSumarizedRemainingText()}"
                        : $"Processing... ({pb.Value} in {pb.TimeProcessing.ToStringWithAllHours()})")
                    .SetForegroundColor(ConsoleColor.Cyan);

                Paused.SetValue(pb => pb.HasProgress ?
                        $"Paused... Running time: {pb.TimeProcessing.ToStringWithAllHours()}"
                        : $"{pb.Value} of {pb.Maximum} in {pb.TimeProcessing.ToStringWithAllHours()} (paused)")
                    .SetForegroundColor(ConsoleColor.DarkCyan);

                Done.SetValue("Done!")
                    .SetForegroundColor(ConsoleColor.DarkYellow);
            }

            /// <summary>
            /// Gets the current Text Body definition by the ProgressBar context ("Processing", "Paused" or "Done")
            /// </summary>
            /// <param name="progressBar"></param>
            /// <returns></returns>
            public Element<string> GetCurrentText(ProgressBar progressBar)
            {
                if (progressBar == null) return null;
                else if (progressBar.IsPaused) return Paused;
                else if (progressBar.IsDone) return Done;
                else if (progressBar.IsStarted) return Processing;
                else return null;
            }
        }

    }
}
