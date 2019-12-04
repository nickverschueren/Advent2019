using System;
using System.Collections.Generic;
using System.Linq;

namespace Advent4
{
    class Program
    {
        static void Main(string[] args)
        {
            var startCode = "125730";
            var endcode = "579381";
            var currentCode = startCode;
            var digits = currentCode.Select(c => int.Parse(c.ToString())).ToArray();
            var goodCodes = new List<string>();
            var numberOfDigits = digits.Length;

            for (int i = 1; i < numberOfDigits; i++)
            {
                if (digits[i] < digits[i - 1])
                    digits[i] = digits[i - 1];
            }
            currentCode = string.Join(null, digits);

            do
            {
                /* For 1st star
                if (digits.Distinct().Count() < numberOfDigits)
                    goodCodes.Add(currentCode);
                */

                // For 2nd star
                if (digits.GroupBy(d => d).Select(g => g.Count()).Contains(2))
                    goodCodes.Add(currentCode);

                for (int j = numberOfDigits - 1; j >= 0; j--)
                {
                    digits[j]++;
                    if (digits[j] == 10) continue;
                    for (int k = j; k < numberOfDigits; k++)
                    {
                        digits[k] = digits[j];
                    }
                    break;
                }

                currentCode = string.Join(null, digits);
            } while (StringComparer.InvariantCulture.Compare(currentCode, endcode) < 0);

            Console.WriteLine("Codes: " + string.Join(",", goodCodes));
            Console.WriteLine("Count: " + goodCodes.Count);
        }
    }
}
