using System.Collections.Generic;
using System.Globalization;

namespace arithmetic_coding
{
    public static class ArithmeticDecoder
    {
        /// <summary>
        /// Получает символ из словаря по заданному числу
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="intervals"></param>
        /// <returns></returns>
        private static char GetSymbolByInterval(double encode,
            Dictionary<char, KeyValuePair<double, double>> intervals)
        {
            char symbol = '\0';
            // Проходим по всем интервалам
            foreach (var interval in intervals)
            {
                // Если наше число попадает в интервал, то возвращаем символ, находящийся в этом интервале
                if (encode < interval.Value.Key || encode >= interval.Value.Value) continue;
                symbol = interval.Key;
                return symbol;
            }

            return symbol;
        }

        /// <summary>
        /// Декодирует входное число
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="length"></param>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static string Decode(double encode, int length, Dictionary<char, KeyValuePair<double, double>> intervals)
        {
            string decode = "";
            
            while (length != 0)
            {
                // Получаем символ, соответствующий текущему значению числа, и записываем его в результат
                char symbol = GetSymbolByInterval(encode, intervals);
                decode += symbol;

                // Считаем расстояние интервала этого символа
                double interval = intervals[symbol].Value - intervals[symbol].Key;
                // Получаем новое число
                encode = (encode - intervals[symbol].Key) / interval;
                length--;
            }

            return decode;
        }
    }
}