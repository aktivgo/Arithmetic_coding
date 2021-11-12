using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace arithmetic_coding
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                PrintMenu();
                Console.Write("Выберите пункт меню: ");
                var ch = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                switch (ch)
                {
                    case 0:
                        return;
                    case 1:
                    {
                        Console.Write("Введите название файла: ");
                        string fileName = Console.ReadLine();
                        if (!File.Exists("encode/" + fileName))
                        {
                            Console.WriteLine("Попробуйте ещё раз\n");
                            break;
                        }

                        Console.WriteLine();

                        GetEncode(fileName);
                    }
                        break;
                    case 2:
                    {
                        Console.Write("Введите название файла: ");
                        string fileName = Console.ReadLine();
                        if (!File.Exists("decode/" + fileName))
                        {
                            Console.WriteLine("Попробуйте ещё раз\n");
                            break;
                        }

                        Console.WriteLine();

                        GetDecode(fileName);
                    }
                        break;
                    default:
                        Console.WriteLine("Попробуйте ещё раз\n");
                        break;
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("1. Закодировать сообщение");
            Console.WriteLine("2. Раскодировать сообщение");
            Console.WriteLine("0. Выход\n");
        }

        private static void GetEncode(string fileName)
        {
            FileStream stream = new FileStream("encode/" + fileName, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string textFromFile = reader.ReadToEnd();
            stream.Close();

            Console.WriteLine("Входные данные:\n" + textFromFile + "\n");
            Console.WriteLine("Результат:");
            Console.WriteLine(ArithmeticEncoder.Encode(textFromFile));
            Console.WriteLine();
        }

        private static void GetDecode(string fileName)
        {
            FileStream stream = new FileStream("decode/" + fileName, FileMode.Open);
            StreamReader reader = new StreamReader(stream, Encoding.Default);
            string textFromFile = reader.ReadToEnd();
            stream.Close();
            Console.WriteLine("Входные данные:\n" + textFromFile + "\n");
            char[] separators = { '\n', '\r' };
            string[] textAr = textFromFile.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<char, KeyValuePair<double, double>> table =
                new Dictionary<char, KeyValuePair<double, double>>();
            for (int i = 2; i < textAr.Length; i++)
            {
                string[] row = textAr[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                char character;
                KeyValuePair<double, double> interval;
                if (row.Length == 2)
                {
                    character = ' ';
                    interval = new KeyValuePair<double, double>(double.Parse(row[0]), double.Parse(row[1]));
                }
                else
                {
                    character = char.Parse(row[0]);
                    interval = new KeyValuePair<double, double>(double.Parse(row[1]), double.Parse(row[2]));
                }

                table.Add(character, interval);
            }

            Console.WriteLine("Результат:");
            Console.WriteLine(ArithmeticDecoder.Decode(double.Parse(textAr[0].Replace(".", ",")), int.Parse(textAr[1]), table));
            Console.WriteLine();
        }
    }
}