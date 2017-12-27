using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day24
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day24.txt").ToList();

            var res = GetStrongestAndLongestBridge(input);
            Console.WriteLine($"Day24 - part1 - result: {res.Item1}");
            Console.WriteLine($"Day24 - part2 - result: {res.Item2}");
        }


        // { strongest, longest }
        public static Tuple<int,int> GetStrongestAndLongestBridge(IEnumerable<string> partDescriptions)
        {
            var parts       = partDescriptions.Select((d, i) => Tuple.Create(i, int.Parse(d.Split('/')[0]), int.Parse(d.Split('/')[1]))).ToList();
            var paths       = new List<Path>() {Path.Begin};
            var maxStrength     = 0;
            var longestStrength = 0;
            while (paths.Count > 0)
            {
                longestStrength = paths.Max(p => p.Strength);
                maxStrength     = Math.Max(maxStrength, longestStrength);

                // Find the next possible paths
                var newPaths = new List<Path>();
                foreach (var path in paths)
                {
                    // find all possible parts that can be added and create new Paths out of them
                    newPaths.AddRange(
                        parts
                            .Where(p => !path.ContainsPart(p.Item1) && (path.ExposedPort == p.Item2 || path.ExposedPort == p.Item3))
                            .Select( path.Append )
                    );
                }

                paths = newPaths;
            }

            return Tuple.Create(maxStrength, longestStrength);
        }
    }

    // Immutable
    // contains the full state of the construction history
    internal class Path
    {
        private readonly HashSet<int> _parts;

        public static readonly Path Begin = new Path(new HashSet<int>(), 0, 0 );

        public int ExposedPort { get; }
        public int Strength { get; }

        private Path(HashSet<int> parts, int exposedPort, int strength)
        {
            _parts = parts;
            ExposedPort = exposedPort;
            Strength = strength;
        }

        public bool ContainsPart(int partId) => _parts.Contains(partId);

        // Build a new Path from the current one with a new part appended
        public Path Append(Tuple<int, int, int> part)
            => new Path(
                    new HashSet<int>( _parts.Concat( new [] { part.Item1 }) ),
                    ExposedPort == part.Item2 ? part.Item3 : part.Item2,
                    Strength + part.Item2 + part.Item3
                );
    }




    [TestFixture]
    internal class Day24Tests
    {
        private static readonly string[] Input =
        {
            "0/2",
            "2/2",
            "2/3",
            "3/4",
            "3/5",
            "0/1",
            "10/1",
            "9/10"
        };

        [Test]
        public void Test1_1()
        {
            var res = Day24.GetStrongestAndLongestBridge(Input).Item1;
            Assert.AreEqual(31, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day24.GetStrongestAndLongestBridge(Input).Item2;
            Assert.AreEqual(19, res);
        }
    }
}
