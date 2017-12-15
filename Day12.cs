using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day12
    {
        private static readonly Regex RowExp = new Regex(@"(?<from>\d+) <-> (?<tos>[\d, ]+)");

        public static void Run()
        {
            var input = File.ReadAllLines("input\\day12.txt").ToList();

            var res1 = NumberOfConnectedProgramsToZero(input);
            Console.WriteLine($"Day12 - part1 - result: {res1}");

            var res2 = NumberOfProgramGroups(input);
            Console.WriteLine($"Day12 - part2 - result: {res2}");
        }

        // Solution:
        // - build all the (p1)-(p2) edges
        // - use an expanding breadth-first search starting from program 0 to find all connected programs
        // - count them
        public static int NumberOfConnectedProgramsToZero(IEnumerable<string> input)
        {
            var edges           = GetEdges(input).ToList();
            var connectedToZero = GetConnectedPrograms(0, edges);
            return connectedToZero.Count();
        }

        // Solution:
        // - build all the (p1)-(p2) edges
        // - use an expanding breadth-first search starting from first program in the list to find all connected programs
        // - remove all the connected programs from the set of known programs and repeat until there are no more known programs
        // -> solution is the number of loops taken to remove all the known programs from the set (each loop removes one group)
        public static int NumberOfProgramGroups(IEnumerable<string> input)
        {
            var edgesList = GetEdges(input).ToList();
            var nodesLeft = new HashSet<int>( edgesList.SelectMany(e => new[] {e.Item1, e.Item2}) );
            int groupCount;
            for (groupCount = 0; nodesLeft.Any(); groupCount++)
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed - using side effect of Remove
                GetConnectedPrograms(nodesLeft.First(), edgesList).Select(p => nodesLeft.Remove(p)).ToList();

            return groupCount;
        }

        private static IEnumerable<int> GetConnectedPrograms(int root, IEnumerable<Tuple<int, int>> edges)
        {
            var edgesList = edges.ToList();
            var edgesTo   = edgesList.ToLookup(e => e.Item1, e => e.Item2);
            var edgesFrom = edgesList.ToLookup(e => e.Item2, e => e.Item1);

            // Perform a breadth-first search over the edges
            var frontier = new List<int>   { root };
            var visited  = new HashSet<int>(frontier);

            // expand the frontier - Add is storing the visited ones as a side effect
            while (frontier.Any())
                frontier = frontier.SelectMany(f => edgesTo[f].Union(edgesFrom[f])).Where(f => visited.Add(f)).ToList();

            return visited;
        }

        private static IEnumerable<Tuple<int, int>> GetEdges(IEnumerable<string> input)
            => input
                .Select(r => RowExp.Match(r))
                .Where(m => m.Success)
                .SelectMany(
                    m => m.Groups["tos"].Value.Split(new [] {", "}, StringSplitOptions.None)
                        .Select(t => Tuple.Create(int.Parse(m.Groups["from"].Value), int.Parse(t))));
    }


    [TestFixture]
    internal class Day12Tests
    {
        private static readonly string[] Input = {
            "0 <-> 2",
            "1 <-> 1",
            "2 <-> 0, 3, 4",
            "3 <-> 2, 4",
            "4 <-> 2, 3, 6",
            "5 <-> 6",
            "6 <-> 4, 5"
        };

        [Test]
        public void Test1_1()
        {
            var res = Day12.NumberOfConnectedProgramsToZero(Input);
            Assert.AreEqual(6, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day12.NumberOfProgramGroups(Input);
            Assert.AreEqual(2, res);
        }
    }
}
