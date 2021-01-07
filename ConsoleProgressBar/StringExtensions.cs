namespace ConsoleProgressBar
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxChars, string append = "...")
            => value.Length <= maxChars ? value : value.Substring(0, maxChars - append.Length) + append;

    }
}
