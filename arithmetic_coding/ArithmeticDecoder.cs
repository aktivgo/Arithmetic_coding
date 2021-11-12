using System.Collections.Generic;
using System.Globalization;

namespace arithmetic_coding
{
    public static class ArithmeticDecoder
    {
        private static char GetSymbolByInterval(double encode,
            Dictionary<char, KeyValuePair<double, double>> intervals)
        {
            char symbol = '\0';
            foreach (var interval in intervals)
            {
                if (encode < interval.Value.Key || encode >= interval.Value.Value) continue;
                symbol = interval.Key;
                return symbol;
            }

            return symbol;
        }

        public static string Decode(double encode, int length, Dictionary<char, KeyValuePair<double, double>> intervals)
        {
            string decode = "";

            while (length != 0)
            {
                char symbol = GetSymbolByInterval(encode, intervals);
                decode += symbol;

                double interval = intervals[symbol].Value - intervals[symbol].Key;
                encode = (encode - intervals[symbol].Key) / interval;
                length--;
            }

            return decode;
        }
    }
}