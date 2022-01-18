namespace ProjectNameApi.Extensions
{
    public static class StringArrayExt
    {
        public static string ToStringWithLineBreaks(this string[] array)
        {
            var output = "";
            var hasText = false;

            for (var i = array.Length - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(array[i]))
                {
                    if (hasText) output = "\n" + output;

                    continue;
                }

                if (hasText)
                    output = array[i] + "\n" + output;
                else output = array[i] + output;

                hasText = true;
            }

            return output;
        }
    }
}