using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace TextRankCalc
{
    class TextRankCalc
    {
        static HashSet<char> VowelsEn = new HashSet<char> {'a', 'e', 'i', 'u', 'o'};
        static HashSet<char> VowelsRu = new HashSet<char> {'у', 'е', 'а', 'о', 'э', 'я', 'и', 'ю', 'ы'};
        static HashSet<char> ConsonantsEn = new HashSet<char> {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z'};
        static HashSet<char> ConsonantRu = new HashSet<char> {'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ'};

        public static decimal Calc(string text)
        {
            decimal consonants = 0;
            decimal vowels = 0;
            foreach (var letter in text)
            {                
                if(ConsonantsEn.Contains(letter) || ConsonantRu.Contains(letter))
                {
                    consonants++;
                }
                if(VowelsEn.Contains(letter) || VowelsRu.Contains(letter))
                {
                    vowels++;
                }
            }   
            decimal resualt = vowels / consonants; 
            return Math.Truncate(100 * resualt) / 100;
        }
    }
}
