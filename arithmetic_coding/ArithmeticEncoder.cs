using System.Collections.Generic;
using System.Linq;

namespace arithmetic_coding
{
    public static class ArithmeticEncoder
    {
        private static Dictionary<char, KeyValuePair<double, double>> charIntervals =
            new Dictionary<char, KeyValuePair<double, double>>();

        private static void InitCharIntervals(string str)
        {
            double lowerBorder = 0;
            foreach (var character in str)
            {
                if (charIntervals.ContainsKey(character)) continue;
                
                Dictionary<char, double> probabilities = GetCharsProbability(str);
                double upperBorder = lowerBorder + probabilities[character];
                KeyValuePair<double, double> interval = new KeyValuePair<double, double>(lowerBorder, upperBorder);
                charIntervals.Add(character, interval);
                lowerBorder = upperBorder;
            }
            charIntervals = charIntervals.OrderBy(pair => pair.Value.Key)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        
        private static Dictionary<char, double> GetCharsProbability(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            foreach (var character in str)
            {
                if (char.IsWhiteSpace(character))
                {
                    if (!result.ContainsKey('&'))
                    {
                        result.Add('&', 1);
                    }
                    else
                    {
                        result['&']++;
                    }
                    continue;
                }

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
        public static string Encode(string inputStr)
        {
            string encode = "";

            InitCharIntervals(inputStr);
            
            return encode;
        }
    }
}