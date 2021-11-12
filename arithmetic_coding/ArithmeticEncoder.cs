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

        private static Dictionary<char, KeyValuePair<double, double>> _charIntervals;

        private static void InitCharIntervals(string str)
        {
            double lowerBorder = 0;
            _charIntervals = new Dictionary<char, KeyValuePair<double, double>>();
            _probabilities = GetCharsProbability(str);

            foreach (var item in _probabilities)
            {
                double upperBorder = lowerBorder + _probabilities[item.Key];
                KeyValuePair<double, double> interval = new KeyValuePair<double, double>(lowerBorder, upperBorder);
                _charIntervals.Add(item.Key, interval);
                lowerBorder = upperBorder;
            }
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
                result[key] = Math.Round(result[key], 5);
            }

            result = result.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            return result;
        }

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

            WriteResultToFile(inputStr, lowerBorder);

            return lowerBorder;
        }
    }
}