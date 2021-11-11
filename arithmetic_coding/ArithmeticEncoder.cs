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
        private static Dictionary<char, double> _probabilities;

        private static Dictionary<char, KeyValuePair<double, double>> _charIntervals =
            new Dictionary<char, KeyValuePair<double, double>>();

        private static void InitCharIntervals(string str)
        {
            double lowerBorder = 0;

            _probabilities = GetCharsProbability(str);

            foreach (var character in str)
            {
                if (_charIntervals.ContainsKey(character)) continue;

                double upperBorder = lowerBorder + _probabilities[character];
                KeyValuePair<double, double> interval = new KeyValuePair<double, double>(lowerBorder, upperBorder);
                _charIntervals.Add(character, interval);
                lowerBorder = upperBorder;
            }

            _charIntervals = _charIntervals.OrderBy(pair => pair.Value.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

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
            }

            return result;
        }

        private static void WriteResultToFile(string str, double encode)
        {
            string path = "decode/";
            string fileName = "test_" + str.Substring(0, 6) + ".txt";

            using (FileStream fstream = new FileStream(path + fileName, FileMode.OpenOrCreate))
            {
                byte[] array = Encoding.Default.GetBytes(encode.ToString(CultureInfo.InvariantCulture) + "\r\n");
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

        public static double Encode(string inputStr)
        {
            InitCharIntervals(inputStr);

            double lowerBorder = 0;
            double upperBorder = 1;

            foreach (var character in inputStr)
            {
                double interval = upperBorder - lowerBorder;
                upperBorder = lowerBorder + interval * _charIntervals[character].Value;
                lowerBorder = lowerBorder + interval * _charIntervals[character].Key;
            }

            WriteResultToFile(inputStr, Math.Round(lowerBorder, inputStr.Length));

            return lowerBorder;
        }
    }
}