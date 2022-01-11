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
        /// Definition of the Layout used to Render the Body of the ProgressBar
        /// </summary>
        public class LayoutBody
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
            /// Element To show in Pending Section
            /// </summary>
            public Element<char> Pending { get; } = new Element<char>();

            /// <summary>
            /// Element to show in Progress Section
            /// </summary>
            public Element<char> Progress { get; } = new Element<char>();

            /// <summary>
            /// Layout for the Text 
            /// </summary>
            public Element<string> Text { get; } = new Element<string>();

            /// <summary>
            /// Sets the LayoutBody value for Pending and Progress elements
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public LayoutBody SetValue(char value) 
                => SetValue(pb => value);
            /// <summary>
            /// Sets the LayoutBody value for Pending and Progress elements
            /// </summary>
            /// <param name="valueGetter"></param>
            /// <returns></returns>
            public LayoutBody SetValue(Func<ProgressBar, char> valueGetter)
            {
                Pending.SetValue(valueGetter);
                Progress.SetValue(valueGetter);
                return this;
            }

            /// <summary>
            /// Sets the ForegroundColor for Pending and Progress elements
            /// </summary>
            /// <param name="foregroundColor"></param>
            /// <returns></returns>
            public LayoutBody SetForegroundColor(ConsoleColor foregroundColor) 
                => SetForegroundColor(pb => foregroundColor);

            /// <summary>
            /// Sets the ForegroundColor for Pending and Progress elements
            /// </summary>
            /// <param name="foregroundColorGetter"></param>
            /// <returns></returns>
            public LayoutBody SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                Pending.SetForegroundColor(foregroundColorGetter);
                Progress.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the BackgroundColor for Pending and Progress elements
            /// </summary>
            /// <param name="backgroundColor"></param>
            /// <returns></returns>
            public LayoutBody SetBackgroundColor(ConsoleColor backgroundColor)
                => SetBackgroundColor(pb => backgroundColor);
            /// <summary>
            /// Sets the BackgroundColor for Pending and Progress elements
            /// </summary>
            /// <param name="backgroundColorGetter"></param>
            /// <returns></returns>
            public LayoutBody SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                Pending.SetBackgroundColor(backgroundColorGetter);
                Progress.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            /// <summary>
            /// Ctor
            /// </summary>
            public LayoutBody()
            {
                Pending.SetValue('·').SetForegroundColor(ConsoleColor.DarkGray);
                Progress.SetValue('■').SetForegroundColor(ConsoleColor.DarkGreen);
                Text.SetVisible(false);
            }
        }

    }
}