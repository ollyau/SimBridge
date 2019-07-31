using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    class NNumber
    {
        private static Random r = new Random();

        public static string Generate()
        {
            var validChars = "ABCDEFGHJKLMNPQRSTUVWXYZ1234567890";
            var outChars = new char[5];

            for (int i = 0; i < outChars.Length; i++)
            {
                if (i == 0)
                {
                    outChars[i] = validChars[r.Next(24, validChars.Length - 1)];
                }
                else if (i < 3)
                {
                    outChars[i] = validChars[r.Next(24, validChars.Length)];
                }
                else if (!char.IsNumber(outChars[i-1]))
                {
                    outChars[i] = validChars[r.Next(24)];
                }
                else
                {
                    outChars[i] = validChars[r.Next(validChars.Length)];
                }
            }

            return new string(outChars);
        }

        public static string Phonetic(char input)
        {
            switch (char.ToUpperInvariant(input))
            {
                case 'A': return "Alpha";
                case 'B': return "Bravo";
                case 'C': return "Charlie";
                case 'D': return "Delta";
                case 'E': return "Echo";
                case 'F': return "Foxtrot";
                case 'G': return "Golf";
                case 'H': return "Hotel";
                case 'I': return "India";
                case 'J': return "Juliet";
                case 'K': return "Kilo";
                case 'L': return "Lima";
                case 'M': return "Mike";
                case 'N': return "November";
                case 'O': return "Oscar";
                case 'P': return "Papa";
                case 'Q': return "Quebec";
                case 'R': return "Romeo";
                case 'S': return "Sierra";
                case 'T': return "Tango";
                case 'U': return "Uniform";
                case 'V': return "Victor";
                case 'W': return "Whiskey";
                case 'X': return "X-ray";
                case 'Y': return "Yankee";
                case 'Z': return "Zulu";
                case '0': return "Zero";
                case '1': return "One";
                case '2': return "Two";
                case '3': return "Three";
                case '4': return "Four";
                case '5': return "Five";
                case '6': return "Six";
                case '7': return "Seven";
                case '8': return "Eight";
                case '9': return "Niner";
                case '.': return "Point";
                default:
                    throw new ArgumentException($"No matching phonetic alphabet equivalent for '{input}'.");
            }
        }

        public static string Phonetic(string input)
        {
            var sb = new StringBuilder();

            foreach (char c in input)
            {
                sb.Append(Phonetic(c));
                sb.Append(' ');
            }

            return sb.ToString().TrimEnd();
        }
    }
}
