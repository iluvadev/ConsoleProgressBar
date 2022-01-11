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
        /// Definition for the Description lines in a ProgressBar
        /// </summary>
        public class TextDescription
        {
            /// <summary>
            /// Description lines definition when ProgressBar is "Processing"
            /// </summary>
            public ElementList<string> Processing { get; } = new ElementList<string>();

            /// <summary>
            /// Description lines definition when ProgressBar is "Paused"
            /// </summary>
            public ElementList<string> Paused { get; } = new ElementList<string>();

            /// <summary>
            /// Description lines definition when ProgressBar is "Done"
            /// </summary>
            public ElementList<string> Done { get; } = new ElementList<string>();

            /// <summary>
            /// Indentation for Description lines
            /// </summary>
            public Element<string> Indentation { get; }
                 = new Element<string>();

            /// <summary>
            /// Ctor
            /// </summary>
            public TextDescription()
            {
                Processing.AddNew().SetValue(pb => pb.ElementName)
                                   .SetVisible(pb => !string.IsNullOrEmpty(pb.ElementName))
                                   .SetForegroundColor(ConsoleColor.DarkYellow);

                Paused.AddNew().SetValue("[Paused]")
                               .SetForegroundColor(ConsoleColor.DarkCyan);

                Done.AddNew().SetValue(pb => $"{pb.Value} in {pb.TimeProcessing.ToStringWithAllHours()} ({pb.TimePerElement.ToStringWithAllHours()} each one)")
                             .SetForegroundColor(ConsoleColor.DarkGray);

                Indentation.SetValue("  -> ").SetForegroundColor(ConsoleColor.DarkBlue);
            }

            /// <summary>
            /// Clears Description Lines
            /// </summary>
            /// <returns></returns>
            public TextDescription Clear()
            {
                Processing.Clear();
                Paused.Clear();
                Done.Clear();
                return this;
            }

            /// <summary>
            /// Gets the current Description Lines definition by the ProgressBar context ("Processing", "Paused" or "Done")
            /// </summary>
            /// <param name="progressBar"></param>
            /// <returns></returns>
            public ElementList<string> GetCurrentDefinitionList(ProgressBar progressBar)
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
