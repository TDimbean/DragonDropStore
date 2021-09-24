using System;

namespace DragonDrop.Integration.Helpers
{
    public static class StringHelpers
    {
        public static string GetUntilOrEmpty(this string text, string stopAt = ".")
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal) + 1;

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return string.Empty;
        }

    }
}
