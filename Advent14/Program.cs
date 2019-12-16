using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent14
{
    class Program
    {
        private const string Fuel = "FUEL";
        private const string Ore = "ORE";

        static async Task Main(string[] args)
        {
            var input = await GetInput();

            //For 1st star
            //For1stStar(input);

            //For 2nd star
            For2ndStart(input);
        }

        private static void For1stStar(Dictionary<string, (long count, Dictionary<string, long> recipe)> recipes)
        {
            long result = CalculateForFuelQtty(1, recipes);
            Console.WriteLine($"{result} {Ore} => 1 {Fuel}");
        }

        private static void For2ndStart(Dictionary<string, (long count, Dictionary<string, long> recipe)> recipes)
        {
            var fuelQtty = 7860000L;
            long result;
            do
            {
                fuelQtty++;
                result = CalculateForFuelQtty(fuelQtty, recipes);
                Console.WriteLine($"{result} {Ore} => {fuelQtty} {Fuel}");
            } while (result <= 1000000000000L);

        }

        private static long CalculateForFuelQtty(long fuelQtty,
            Dictionary<string, (long count, Dictionary<string, long> recipe)> recipes)
        {
            var requirements = recipes.ToDictionary(i => i.Key, i => new Dictionary<string, long>());
            var recalc = new List<string> { Fuel };
            requirements[Fuel] = new Dictionary<string, long> { { "", fuelQtty } };
            while (recalc.Count > 0)
            {
                recalc = Produce(recipes, requirements, recalc);
            }
            return requirements[Ore].Values.Sum();
        }

        private static List<string> Produce(Dictionary<string, (long count,
            Dictionary<string, long> recipe)> recipes,
            Dictionary<string, Dictionary<string, long>> requirements,
            List<string> recalc)
        {
            var next = new List<string>();
            foreach (var name in recalc)
            {
                if (!recipes.TryGetValue(name, out var formula)) continue;
                var req = requirements[name].Values.Sum();
                var qtty = (((req - 1) / formula.count) + 1);
                foreach (var ingredient in formula.recipe)
                {
                    if (requirements.TryGetValue(ingredient.Key, out var r))
                    {
                        if (r.ContainsKey(name))
                        {
                            r[name] = qtty * ingredient.Value;
                        }
                        else
                        {
                            r.Add(name, qtty * ingredient.Value);
                        }
                    }
                    else
                    {
                        requirements.Add(ingredient.Key, new Dictionary<string, long> { { name, qtty * ingredient.Value } });
                    }
                    next.Add(ingredient.Key);
                }
            }
            return next;
        }

        private static async Task<Dictionary<string, (long count, Dictionary<string, long> recipe)>> GetInput()
        {
            var result = new Dictionary<string, (long, Dictionary<string, long>)>();
            using (var reader = new StreamReader("input.txt", true))
            {
                var line = await reader.ReadLineAsync();
                while (!string.IsNullOrWhiteSpace(line))
                {
                    var parts = line.Split(" => ", StringSplitOptions.RemoveEmptyEntries);
                    var product = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var dev = long.Parse(product[0]);
                    var ingredients = parts[0].Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(p =>
                    {
                        var i = p.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        return (i[1], long.Parse(i[0]));
                    }).ToDictionary(i => i.Item1, i => i.Item2);

                    result.Add(product[1], (dev, ingredients));

                    line = await reader.ReadLineAsync();
                }
                return result;
            }
        }


    }
}
