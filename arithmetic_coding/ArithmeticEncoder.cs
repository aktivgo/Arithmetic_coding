using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace arithmetic_coding
{
    public static class ArithmeticEncoder
    {
        private static Dictionary<char, double> _probabilities;                         // Словарь символ-вероятность
        private static Dictionary<char, KeyValuePair<double, double>> _charIntervals;   // Словарь символ-интервал

        /// <summary>
        /// Получает словарь символ-интервал
        /// </summary>
        /// <param name="str"></param>
        private static void InitCharIntervals(string str)
        {
            double lowerBorder = 0;
            _charIntervals = new Dictionary<char, KeyValuePair<double, double>>();
            // Словарь символ-вероятность
            _probabilities = GetCharsProbability(str);

            // Проходим по каждомук символу в словаре
            foreach (var item in _probabilities)
            {
                // Считаем верхнюю границу для данного символа и записываем нижнюю и верхнюю границу в словарь с интервалами
                double upperBorder = lowerBorder + _probabilities[item.Key];
                KeyValuePair<double, double> interval = new KeyValuePair<double, double>(lowerBorder, upperBorder);
                _charIntervals.Add(item.Key, interval);
                // Новая нижняя граница равна верхней
                lowerBorder = upperBorder;
            }
        }

        /// <summary>
        /// Получает словарь символ-вероятность
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Dictionary<char, double> GetCharsProbability(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            foreach (var character in str)
            {
                if (result.ContainsKey(character))
                {
                    result[character]++;
                }
                else
                {
                    result.Add(character, 1);
                }
            }

            foreach (char key in result.Keys.ToArray())
            {
                result[key] /= str.Length;
                result[key] = Math.Round(result[key], 5);
            }

            // Сортируем по убыванию вероятностей
            result = result.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return result;
        }

        /// <summary>
        /// Записывает результат в файл
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
        private static void WriteResultToFile(string str, double encode)
        {
            string path = "decode/";
            string fileName = "test_" + str.Substring(0, 6) + ".txt";

            using (FileStream fstream = new FileStream(path + fileName, FileMode.Create))
            {
                byte[] array = Encoding.Default.GetBytes(encode.ToString(CultureInfo.InvariantCulture) + "1" + "\r\n");
                fstream.Write(array, 0, array.Length);
                array = Encoding.Default.GetBytes(str.Length + "\r\n");
                fstream.Write(array, 0, array.Length);

                string table = "";
                foreach (var item in _charIntervals)
                {
                    table += item.Key + " " + item.Value.Key + " " + item.Value.Value + "\r\n";
                }

                table = table.Remove(table.Length - 2);
                array = Encoding.Default.GetBytes(table);
                fstream.Write(array, 0, array.Length);
            }

            Console.WriteLine("Результат сохранен в файл " + fileName);
        }

        /// <summary>
        /// Кодирует входную строку
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static double Encode(string inputStr)
        {
            // Инициализируем словарь с интервалами
            InitCharIntervals(inputStr);

            // Начальные значения нижней и верхней границ
            double lowerBorder = 0;
            double upperBorder = 1;

            // Проходим по каждому символу строки
            foreach (var character in inputStr)
            {
                // Считаем длину интервала
                double interval = upperBorder - lowerBorder;
                // Считаем верхнюю и нижнюю границу
                upperBorder = lowerBorder + interval * _charIntervals[character].Value;
                lowerBorder = lowerBorder + interval * _charIntervals[character].Key;
            }

            // Результатом кодирования будет любое число, входящее в последний интервал
            // Записываем результат в файл
            WriteResultToFile(inputStr, lowerBorder);

            return lowerBorder;
        }
    }
}