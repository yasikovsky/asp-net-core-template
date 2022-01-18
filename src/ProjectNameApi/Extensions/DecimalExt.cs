namespace ProjectNameApi.Extensions
{
    public static class DecimalExt
    {
        public static string ToDecimalString(this decimal input, int decimalDigits = 2)
        {
            var format = "0";

            if (decimalDigits > 0)
                format += ".";

            for (var i = 0; i < decimalDigits; i++) format += "0";

            return input.ToString(format);
        }

        public static string ToDecimalString(this decimal? input, int decimalDigits = 2)
        {
            return input == null ? "" : input.Value.ToDecimalString(decimalDigits);
        }

        public static string ToMoneyString(this decimal? input, string currency = "")
        {
            return input == null ? "" : input.Value.ToMoneyString(currency);
        }

        public static string ToMoneyString(this decimal input, string currency = "")
        {
            if (!string.IsNullOrEmpty(currency))
                return $"{input:0.00} {currency}";

            return $"{input:0.00}";
        }
    }
}