using System;

namespace DragonDrop.Infrastructure.Helpers
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

        public static bool IsDigitsOnly(this string inp)
        {
            foreach (var c in inp) if (c < '0' || c > '9') return false;
            return true;
        }

        public static DateTime? ToDate(this string inp)
        {
            var date = new DateTime(1600, 1, 1);
            if (DateTime.TryParse(inp, out date)) return date;
            else return null;
        }

        public static decimal? ToDecimal(this string inp)
        {
            var dec = 0m;
            if (decimal.TryParse(inp, out dec)) return dec;
            else return null;
        }

        public static string RawStringToUpcBarcode(this string inp)
         => inp.Substring(0, 1) + " " + inp.Substring(1, 5) + " " + inp.Substring(6, 5) + " " + inp.Substring(11, 1);

        public static string UpcBarcodeToCondensedString(string inp) => inp.Replace(" ", string.Empty);
    }
}
