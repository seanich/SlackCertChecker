using System;

namespace CertChecker
{
    public static class SlackHelper
    {
        public static string FormatDate(DateTime dateTime)
        {
            var unixTime = ((DateTimeOffset) dateTime).ToUnixTimeSeconds();
            var fallbackString = dateTime.ToString();
            return $"<!date^{unixTime}^{{date_num}} {{time}}|{fallbackString}>";
        }
    }
}