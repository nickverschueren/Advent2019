using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Advent6
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var things = await GetInput();

            /* For 1st star
            var output = things.Values.Sum(t => GetAncesters(t).Count());
            Console.WriteLine($"Total orbids: {output}");
            */

            // For 2nd star
            var output = FindFirstCommonAncestor(things["YOU"], things["SAN"]);
            Console.WriteLine($"Common ancester: {output.thing.Name}");
            Console.WriteLine($"Hops inbetween : {output.hops}");
        }

        private static (Thing thing, int hops) FindFirstCommonAncestor(Thing a, Thing b)
        {
            var aAncesters = GetAncesters(a).ToList();
            var bAncesters = GetAncesters(b).ToList();
            var firstCommon = aAncesters.Join(bAncesters, a => a.Item1, b => b.Item1,
                (a, b) => (a.thing, hops: a.hops + b.hops))
                .OrderBy(i => i.hops).First();
            return firstCommon;
        }

        private static IEnumerable<(Thing thing, int hops)> GetAncesters(Thing t)
        {
            int i = 0;
            while (t.Orbits != null)
            {
                yield return (t.Orbits, i++);
                t = t.Orbits;
            }
        }

        private static async Task<Dictionary<string, Thing>> GetInput()
        {
            var result = new Dictionary<string, Thing>();
            using (var reader = new StreamReader("input.txt", true))
            {
                var line = await reader.ReadLineAsync();
                while (line != null)
                {
                    var parts = line.Split(")");
                    Thing p = GetOrCreate(result, parts[0]);
                    Thing c = GetOrCreate(result, parts[1]);
                    c.Orbits = p;

                    line = await reader.ReadLineAsync();
                }
                return result;
            }
        }

        private static Thing GetOrCreate(Dictionary<string, Thing> result, string part)
        {
            if (!result.TryGetValue(part, out var p))
            {
                p = new Thing { Name = part };
                result.Add(p.Name, p);
            }

            return p;
        }

        class Thing
        {
            public string Name { get; set; }
            public Thing Orbits { get; set; }
        }
    }
}
