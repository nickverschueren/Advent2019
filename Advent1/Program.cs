using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await GetInput();
            var output = input.Select(CalculateFuel).Sum();
            Console.WriteLine(output);
        }

        private static async Task<int[]> GetInput()
        {
            using (var reader = new StreamReader("./input.txt", true))
            {
                var content = await reader.ReadToEndAsync();
                return content.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s)).ToArray();
            }
        }

        private static int CalculateFuel(int weight)
        { 
            if (weight < 9) return 0;
            var fuel = ((int)Math.Floor(weight / 3d)) - 2;
            
            //Recursion added for 2nd star
            fuel += CalculateFuel(fuel); 

            return fuel;
        }
    }
}
