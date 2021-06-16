using System;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace LoadSite
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Введите адрес сайта, на котором вы хотите посчитать уникальные слова: ");
                string name = Console.ReadLine();
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] raw = wc.DownloadData(name);
                string webData = System.Text.Encoding.UTF8.GetString(raw);

                var pageDoc = new HtmlDocument();
                pageDoc.LoadHtml(webData);
                string pageText = pageDoc.DocumentNode.InnerText;
                pageText = pageText.Trim();

                List<char> word = new List<char>();//массив под слово
                List<string> words = new List<string>();//массив слов
                bool flag = false;//флаг во избежание записи пробелов, если их больше 2

                foreach (var ch in pageText)
                {
                    if (!Separators.IsSep(ch) && !char.IsDigit(ch))
                    {
                        word.Add(ch);//записываем букву в массив
                        flag = false;
                    }
                    else if (flag == true)
                    {
                        continue;
                    }
                    else
                    {
                        string tmp = new string(word.ToArray());
                        tmp = tmp.ToLower();
                        words.Add(tmp);//записываем слово в массив слов
                        word.Clear();
                        flag = true;
                    }
                }
                pageText = null;
                words.Sort();

                Console.WriteLine($"Всего слов: {words.Count}\nРезультат:");

                for (int i = 0; i < words.Count; i++)
                {
                    int cnt = 0;
                    for (int j = 0; j < words.Count; j++)
                    {
                        if (Equals(words[i], words[j]))
                        {
                            cnt++;//счетчик повторений слова
                        }
                    }
                    Console.WriteLine($"Слово: {words[i],25} | Количество на странице: {cnt}");
                    words.RemoveRange(i, cnt);
                }
                Console.WriteLine($"Уникальных слов: {words.Count}\n");
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("ОШИБКА 1!\nВеб-страницы с таким названием не существует или на устройстве проблемы с подключением к сети.\n");
            }
        }
    }
    class Separators
    {
        private static char[] separators = new char[26] { ' ', ',', '.', '\n', '\0', '!', '?', '"', ';', ':', '(', ')', '\r', '\t', '-', '<', '>', '&', '«', '»', '–', '—', '#', '%', '‹', '›' };

        public static bool IsSep(char c)
        {
            foreach (var ch in separators)
                if (c == ch) return true;
            return false;
        }
    }
}