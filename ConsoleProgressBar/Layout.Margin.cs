// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

using System;

namespace iluvadev.ConsoleProgressBar
{
    public partial class Layout
    {
        /// <summary>
        /// Definition of the Layout used to Render the Margins of the ProgressBar
        /// </summary>
        public class LayoutMargin
        {
            /*  Examples of ProgressBar:
            *      - Marquee is a Character moving around the ProgressBar
            *      
            *      With Progress available (Maximum defined):
            *          [■■■■■■■■■■■■········] -> Without Marquee
            *          [■■■■■■■■■■■■····+···] -> With Marquee (in pending space) 
            *          [■■■■■■■■#■■■········] -> With Marquee (in progress space)
            *          
            *      Without Progress available (don't have Maximum):
            *          [·······■············] -> Marquee is always displayed
            */

            /// <summary>
            /// Element to show at the Margin Left (Start of the ProgressBar) 
            /// </summary>
            public Element<string> Start { get; } = new Element<string>();

            /// <summary>
            /// Element to show at the Margin Right (End of the ProgressBar)
            /// </summary>
            public Element<string> End { get; } = new Element<string>();

            /// <summary>
            /// Sets the LayoutMargin value for Start and End elements
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public LayoutMargin SetValue(string value) 
                => SetValue(pb => value);
            
            /// <summary>
            /// Sets the LayoutMargin value for Start and End elements
            /// </summary>
            /// <param name="valueGetter"></param>
            /// <returns></returns>
            public LayoutMargin SetValue(Func<ProgressBar, string> valueGetter)
            {
                Start.SetValue(valueGetter);
                End.SetValue(valueGetter);
                return this;
            }

            /// <summary>
            /// Sets the ForegroundColor for Start and End elements
            /// </summary>
            /// <param name="foregroundColor"></param>
            /// <returns></returns>
            public LayoutMargin SetForegroundColor(ConsoleColor foregroundColor) 
                => SetForegroundColor(pb => foregroundColor);

            /// <summary>
            /// Sets the ForegroundColor for Start and End elements
            /// </summary>
            /// <param name="foregroundColorGetter"></param>
            /// <returns></returns>
            public LayoutMargin SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                Start.SetForegroundColor(foregroundColorGetter);
                End.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the BackgroundColor for Start and End elements
            /// </summary>
            /// <param name="backgroundColor"></param>
            /// <returns></returns>
            public LayoutMargin SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);
            
            /// <summary>
            /// Sets the BackgroundColor for Start and End elements
            /// </summary>
            /// <param name="backgroundColorGetter"></param>
            /// <returns></returns>
            public LayoutMargin SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                Start.SetBackgroundColor(backgroundColorGetter);
                End.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the visibility for Start and End elements
            /// </summary>
            /// <param name="visible"></param>
            /// <returns></returns>
            public LayoutMargin SetVisible(bool visible) 
                => SetVisible(pb => visible);

            /// <summary>
            /// Sets the visibility for Start and End elements
            /// </summary>
            /// <param name="showGetter"></param>
            /// <returns></returns>
            public LayoutMargin SetVisible(Func<ProgressBar, bool> showGetter)
            {
                Start.SetVisible(showGetter);
                End.SetVisible(showGetter);
                return this;
            }

            /// <summary>
            /// Return the length of Margins
            /// </summary>
            /// <param name="progressBar"></param>
            /// <returns></returns>
            public int GetLength(ProgressBar progressBar)
                => (Start.GetValue(progressBar)?.Length ?? 0) + (End.GetValue(progressBar)?.Length ?? 0);

            /// <summary>
            /// Ctor
            /// </summary>
            public LayoutMargin()
            {
                Start.SetValue("[").SetForegroundColor(ConsoleColor.DarkBlue);
                End.SetValue("]").SetForegroundColor(ConsoleColor.DarkBlue);
            }
        }

    }
}