// Description: ProgressBar for Console Applications, with advanced features.
// Project site: https://github.com/iluvadev/ConsoleProgressBar
// Issues: https://github.com/iluvadev/ConsoleProgressBar/issues
// License (MIT): https://github.com/iluvadev/ConsoleProgressBar/blob/main/LICENSE
//
// Copyright (c) 2021, iluvadev, and released under MIT License.
//

namespace iluvadev.ConsoleProgressBar
{
    /// <summary>
    /// Definitions for Texts in a ProgressBar
    /// </summary>
    public partial class Text
    {
        /// <summary>
        /// Definition of the text in the same line as ProgressBar (Body)
        /// </summary>
        public TextBody Body { get; } = new TextBody();

        /// <summary>
        /// Definition of the texts in the lines below a ProgressBar (Description)
        /// </summary>
        public TextDescription Description { get; } = new TextDescription();
    }
}
