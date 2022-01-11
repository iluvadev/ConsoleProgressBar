// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

using iluvadev.ConsoleProgressBar.Extensions;
using System;
using System.Collections.Generic;

namespace iluvadev.ConsoleProgressBar
{
    /// <summary>
    /// Definition of a Layout for a ProgressBar representation
    /// </summary>
    public partial class Layout
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
        /// Layout definition for Margins
        /// </summary>
        public LayoutMargin Margins { get; } = new LayoutMargin();

        /// <summary>
        /// Layout definition for Marquee (character moving around the ProgressBar)
        /// </summary>
        public LayoutMarquee Marquee { get; } = new LayoutMarquee();

        /// <summary>
        /// Layout definition for Body
        /// </summary>
        public LayoutBody Body { get; } = new LayoutBody();


        /// <summary>
        /// Width of entire ProgressBar
        /// Default = 30
        /// </summary>
        public int ProgressBarWidth { get; set; } = 30;

        /// <summary>
        /// Gets the internal Width of the ProgressBar
        /// </summary>
        public int GetInnerWidth(ProgressBar progressBar)
            => Math.Max(ProgressBarWidth - Margins.GetLength(progressBar), 0);

        /// <summary>
        /// Returns the Actions to do in order to Render the ProgressBar with this Layout
        /// </summary>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        internal List<Action> GetRenderActions(ProgressBar progressBar)
        {
            //  [■■■■■■■■■■■■········] -> Without Marquee
            //  [■■■■■■■■■■■■····+···] -> Marquee over Pending space
            //  [■■■■■■■■#■■■········] -> Marquee over Progress space
            //  [·····+··············] -> Marquee withot progress
            var list = new List<Action>();

            int innerWidth = GetInnerWidth(progressBar);
            int progressLenght = progressBar.HasProgress ? Convert.ToInt32(progressBar.Percentage / (100f / innerWidth)) : 0;
            int pendingLenght = innerWidth - progressLenght;

            bool marqueeInProgress = progressBar.HasProgress &&
                progressBar.MarqueePosition >= 0 && progressBar.MarqueePosition < progressLenght &&
                Marquee.OverProgress.GetVisible(progressBar);
            bool marqueeInPending = progressBar.MarqueePosition >= progressLenght &&
                Marquee.OverPending.GetVisible(progressBar);

            int progressBeforeMarqueeLength = progressLenght;
            if (marqueeInProgress) progressBeforeMarqueeLength = progressBar.MarqueePosition;

            int progressAfterMarqueeLength = 0;
            if (marqueeInProgress) progressAfterMarqueeLength = progressLenght - progressBeforeMarqueeLength - 1;

            int pendingBeforeMarqueeLength = pendingLenght;
            if (marqueeInPending) pendingBeforeMarqueeLength = progressBar.MarqueePosition - progressLenght;

            int pendingAfterMarqueeLength = 0;
            if (marqueeInPending) pendingAfterMarqueeLength = pendingLenght - pendingBeforeMarqueeLength - 1;

            string innerText = "";
            if (Body.Text.GetVisible(progressBar))
                innerText = (Body.Text.GetValue(progressBar) ?? "").AdaptToMaxWidth(innerWidth);

            string textProgressBeforeMarquee = string.IsNullOrEmpty(innerText) ?
                                               new string(Body.Progress.GetValue(progressBar), progressBeforeMarqueeLength)
                                               : innerText.Substring(0, progressBeforeMarqueeLength);

            char? charProgressMarquee = null;
            if (marqueeInProgress) charProgressMarquee = string.IsNullOrEmpty(innerText) ?
                                                         Marquee.OverProgress.GetValue(progressBar)
                                                         : innerText[progressBar.MarqueePosition];

            string textProgressAfterMarquee = string.IsNullOrEmpty(innerText) ?
                                              new string(Body.Progress.GetValue(progressBar), progressAfterMarqueeLength)
                                              : innerText.Substring(progressBar.MarqueePosition + 1, progressAfterMarqueeLength);

            string textPendingBeforeMarquee = string.IsNullOrEmpty(innerText) ?
                                              new string(Body.Pending.GetValue(progressBar), pendingBeforeMarqueeLength)
                                              : innerText.Substring(progressLenght, pendingBeforeMarqueeLength);

            char? charPendingMarquee = null;
            if (marqueeInPending) charPendingMarquee = string.IsNullOrEmpty(innerText) ?
                                                       Marquee.OverPending.GetValue(progressBar)
                                                       : innerText[progressBar.MarqueePosition];

            string textPendingAfterMarquee = string.IsNullOrEmpty(innerText) ?
                                             new string(Body.Pending.GetValue(progressBar), pendingAfterMarqueeLength)
                                             : innerText.Substring(progressBar.MarqueePosition + 1, pendingAfterMarqueeLength);

            //Margin: Start
            list.AddRange(Margins.Start.GetRenderActions(progressBar));

            //Body: Progress before Marquee
            if (!string.IsNullOrEmpty(textProgressBeforeMarquee))
            {
                var elementProgressBeforeMarquee = new Element<string>(textProgressBeforeMarquee,
                    Body.Progress.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Body.Progress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementProgressBeforeMarquee.GetRenderActions(progressBar));
            }

            //Body: Marquee in progress
            if (charProgressMarquee.HasValue)
            {
                var elementProgressMarquee = new Element<char>(charProgressMarquee.Value,
                    Marquee.OverProgress.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Marquee.OverProgress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementProgressMarquee.GetRenderActions(progressBar));
            }

            //Body: Progress after Marquee
            if (!string.IsNullOrEmpty(textProgressAfterMarquee))
            {
                var elementProgressAfterMarquee = new Element<string>(textProgressAfterMarquee,
                    Body.Progress.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Body.Progress.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementProgressAfterMarquee.GetRenderActions(progressBar));
            }

            //Body: Pending before Marquee
            if (!string.IsNullOrEmpty(textPendingBeforeMarquee))
            {
                var elementPendingBeforeMarquee = new Element<string>(textPendingBeforeMarquee,
                    Body.Pending.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Body.Pending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementPendingBeforeMarquee.GetRenderActions(progressBar));
            }

            //Body: Marquee in Pending
            if (charPendingMarquee.HasValue)
            {
                var elementPendingMarquee = new Element<char>(charPendingMarquee.Value,
                    Marquee.OverPending.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Marquee.OverPending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementPendingMarquee.GetRenderActions(progressBar));
            }

            //Body: Pending after Marquee
            if (!string.IsNullOrEmpty(textPendingAfterMarquee))
            {
                var elementPendingAfterMarquee = new Element<string>(textPendingAfterMarquee,
                    Body.Pending.GetForegroundColor(progressBar) ?? Console.ForegroundColor,
                    Body.Pending.GetBackgroundColor(progressBar) ?? Console.BackgroundColor);
                list.AddRange(elementPendingAfterMarquee.GetRenderActions(progressBar));
            }

            //Margin: End
            list.AddRange(Margins.End.GetRenderActions(progressBar));

            return list;
        }
    }
}
