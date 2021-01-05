using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleProgressBar
{
    public class ProgressBarLayout
    {
        //[■■■■■···············] -> ShowProgress
        //[·······■············] -> ShowMarquee
        //[■■■■■··+············] -> ShowProgress + ShowMarquee
        //[■■■■■■■■#■■■········] -> ShowProgress + ShowMarquee (overlapped)

        public string Start { get; set; } = "[";
        public string End { get; set; } = "]";
        public char Pending { get; set; } = '·';
        public char Progress { get; set; } = '■';
        public char MarqueeAlone { get; set; } = '■';
        public char MarqueeInProgressPending { get; set; } = '+';
        public char MarqueeInProgress { get; set; } = '#';
    }
}
