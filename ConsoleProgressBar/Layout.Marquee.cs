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
        /// Definition for the Marquee
        /// The Marquee is a char that moves around the ProgressBar
        /// </summary>
        public class LayoutMarquee
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
            /// Marquee definition when it moves over 'Pending' section
            /// </summary>
            public Element<char> OverPending { get; } = new Element<char>();

            /// <summary>
            /// Marquee definition when it moves over 'Progress' section
            /// </summary>
            public Element<char> OverProgress { get; } = new Element<char>();

            /// <summary>
            /// Sets the Marqee definition when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public LayoutMarquee SetValue(char value) => SetValue(pb => value);

            /// <summary>
            /// Sets the Marqee definition when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="valueGetter"></param>
            /// <returns></returns>
            public LayoutMarquee SetValue(Func<ProgressBar, char> valueGetter)
            {
                OverPending.SetValue(valueGetter);
                OverProgress.SetValue(valueGetter);
                return this;
            }

            /// <summary>
            /// Sets the Marqee Foreground Color when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="foregroundColor"></param>
            /// <returns></returns>
            public LayoutMarquee SetForegroundColor(ConsoleColor foregroundColor) 
                => SetForegroundColor(pb => foregroundColor);

            /// <summary>
            /// Sets the Marqee Foreground Color when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="foregroundColorGetter"></param>
            /// <returns></returns>
            public LayoutMarquee SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
            {
                OverPending.SetForegroundColor(foregroundColorGetter);
                OverProgress.SetForegroundColor(foregroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the Marqee Background Color when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="backgroundColor"></param>
            /// <returns></returns>
            public LayoutMarquee SetBackgroundColor(ConsoleColor backgroundColor) 
                => SetBackgroundColor(pb => backgroundColor);

            /// <summary>
            /// Sets the Marqee Background Color when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="backgroundColorGetter"></param>
            /// <returns></returns>
            public LayoutMarquee SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
            {
                OverPending.SetBackgroundColor(backgroundColorGetter);
                OverProgress.SetBackgroundColor(backgroundColorGetter);
                return this;
            }

            /// <summary>
            /// Sets the Marqee Visibility when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="visible"></param>
            /// <returns></returns>
            public LayoutMarquee SetVisible(bool visible) 
                => SetVisible(pb => visible);

            /// <summary>
            /// Sets the Marqee Visibility when it moves over 'Pending' or 'Progress' section
            /// </summary>
            /// <param name="showGetter"></param>
            /// <returns></returns>
            public LayoutMarquee SetVisible(Func<ProgressBar, bool> showGetter)
            {
                OverPending.SetVisible(showGetter);
                OverProgress.SetVisible(showGetter);
                return this;
            }

            /// <summary>
            /// Ctor
            /// </summary>
            public LayoutMarquee()
            {
                OverPending.SetValue(pb => pb.HasProgress ? '+' : '■')
                           .SetForegroundColor(pb => pb.HasProgress ? ConsoleColor.Yellow : ConsoleColor.Green);

                OverProgress.SetValue('■')
                            .SetForegroundColor(ConsoleColor.Yellow);
            }
        }
    }
}