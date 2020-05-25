using System.Linq;
using BubbleBot.WPF;

namespace BubbleBot.Utility.Extensions
{
    public static class StringsExtensions
    {
        // Fields
        private static readonly string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        public static string ToCamelCase(this string text)
        {
            return $"{char.ToLower(text[0])}{text.Substring(1)}";
        }

        public static string Capitalize(this string text)
        {
            return $"{char.ToUpper(text[0])}{text.Substring(1)}";
        }

        public static string PureCapitalize(this string text)
        {
            return $"{char.ToUpper(text[0])}{text.Substring(1).ToLower()}";
        }

        public static string ToRandomString(this int length)
        {
            return new string(Enumerable.Repeat(characters, length).Select(s => s[Randomize.GetRandomInt(0, s.Length)])
                .ToArray());
        }

        public static string Truncate(this string text, int maxLength)
        {
            return text.Length <= maxLength ? text : $"{text.Substring(0, maxLength)}...";
        }

        public static string SetCulture(this string text)
        {
            return text.ToString(App.Culture);
        }
    }
}