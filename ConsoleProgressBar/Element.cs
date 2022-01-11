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
    /// <summary>
    /// An element of a ProgressBar
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Element<T>
    {
        private Func<ProgressBar, T> _ValueGetter;
        private Func<ProgressBar, ConsoleColor> _ForegroundColorGetter;
        private Func<ProgressBar, ConsoleColor> _BackgroundColorGetter;
        private Func<ProgressBar, bool> _VisibleGetter;

        /// <summary>
        /// Ctor
        /// </summary>
        public Element() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        public Element(T value, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            SetValue(value);
            SetForegroundColor(foregroundColor);
            SetBackgroundColor(backgroundColor);
        }

        /// <summary>
        /// Sets the ProgressBar element visible (or not)
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        public Element<T> SetVisible(bool show) 
            => SetVisible(pb => show);

        /// <summary>
        /// Sets the ProgressBar element visible (or not)
        /// </summary>
        /// <param name="showGetter"></param>
        /// <returns></returns>
        public Element<T> SetVisible(Func<ProgressBar, bool> showGetter)
        {
            _VisibleGetter = showGetter;
            return this;
        }
        /// <summary>
        /// Gets the ProgressBar element visibility
        /// </summary>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        public bool GetVisible(ProgressBar progressBar)
            => _VisibleGetter?.Invoke(progressBar) ?? true;

        /// <summary>
        /// Sets the Value of the ProgressBar element
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Element<T> SetValue(T value) => SetValue(pb => value);

        /// <summary>
        /// Sets the Value of the ProgressBar element
        /// </summary>
        /// <param name="valueGetter"></param>
        /// <returns></returns>
        public Element<T> SetValue(Func<ProgressBar, T> valueGetter)
        {
            _ValueGetter = valueGetter;
            return this;
        }

        /// <summary>
        /// Gets the Value of the ProgressBar element
        /// </summary>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        public virtual T GetValue(ProgressBar progressBar)
            => GetVisible(progressBar) && _ValueGetter != null ? _ValueGetter.Invoke(progressBar)
                                                               : default;

        /// <summary>
        /// Sets the ForegroundColor of the ProgressBar element
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <returns></returns>
        public Element<T> SetForegroundColor(ConsoleColor foregroundColor) 
            => SetForegroundColor(pb => foregroundColor);

        /// <summary>
        /// Sets the ForegroundColor of the ProgressBar element
        /// </summary>
        /// <param name="foregroundColorGetter"></param>
        /// <returns></returns>
        public Element<T> SetForegroundColor(Func<ProgressBar, ConsoleColor> foregroundColorGetter)
        {
            _ForegroundColorGetter = foregroundColorGetter;
            return this;
        }

        /// <summary>
        /// Gets the ForegroundColor of the ProgressBar element
        /// </summary>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        public virtual ConsoleColor? GetForegroundColor(ProgressBar progressBar)
            => (GetVisible(progressBar) && _ForegroundColorGetter != null) ? _ForegroundColorGetter.Invoke(progressBar) 
                                                                           : (ConsoleColor?)null;

        /// <summary>
        /// Sets the BackgroundColor of the ProgressBar element
        /// </summary>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        public Element<T> SetBackgroundColor(ConsoleColor backgroundColor) 
            => SetBackgroundColor(pb => backgroundColor);

        /// <summary>
        /// Sets the BackgroundColor of the ProgressBar element
        /// </summary>
        /// <param name="backgroundColorGetter"></param>
        /// <returns></returns>
        public Element<T> SetBackgroundColor(Func<ProgressBar, ConsoleColor> backgroundColorGetter)
        {
            _BackgroundColorGetter = backgroundColorGetter;
            return this;
        }

        /// <summary>
        /// Gets the BackgroundColor of the ProgressBar element
        /// </summary>
        /// <param name="progressBar"></param>
        /// <returns></returns>
        public virtual ConsoleColor? GetBackgroundColor(ProgressBar progressBar)
            => (GetVisible(progressBar) && _BackgroundColorGetter != null) ? _BackgroundColorGetter.Invoke(progressBar) 
                                                                           : (ConsoleColor?)null;
    }

}
