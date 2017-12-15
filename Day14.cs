using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day14
    {
        public static void Run()
        {
            const string input = "hxtvlmkl";

            var res1 = CountSquares(input);
            Console.WriteLine($"Day14 - part1 - result: {res1}");

            var res2 = CountRegions(input);
            Console.WriteLine($"Day14 - part2 - result: {res2}");
        }

        public static int CountSquares(string hashPrefix)
            => Enumerable.Range(0, 128).SelectMany(r => KnotHash($"{hashPrefix}-{r}")).Select(CountBits).Sum();

        private static int[] KnotHash(string inputStr)
        {
            // Build the input numers: ASCII of the input string + a fixed "salt"
            var input = inputStr.Select(c => (int)c).Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();

            // Run 64 rounds
            var builder = new Day10.KnotBuilder(256);
            for (var i = 0; i < 64; i++)
                foreach (var val in input)
                    builder.Tie(val);

            // Densify the hash by XORing the values by blocks of 16
            var sparseHash = builder.Buffer;
            var denseHash = new int[builder.Buffer.Length / 16];
            for (var i = 0; i < denseHash.Length; i++)
                denseHash[i] = sparseHash.Skip(16 * i).Take(16).Aggregate((a, n) => a ^ n);

            return denseHash;
        }


        // Thank you https://stackoverflow.com/questions/109023/how-to-count-the-number-of-set-bits-in-a-32-bit-integer
        private static int CountBits(int number)
        {
            // i must be an unsigned type for >> to act as logical shift instead of arithmetic sign
            // see https://stackoverflow.com/questions/1499647/shifting-the-sign-bit-in-net
            var i = (uint)number;
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            var bitCount = (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
            return (int) bitCount;
        }


        public static int CountRegions(string hashPrefix)
        {
            // Extract a tuple (x,y) for EVERY marked bit
            var markedBits = new HashSet<Tuple<int, int>>(
                Enumerable.Range(0, 128).SelectMany(r => KnotHash($"{hashPrefix}-{r}")).SelectMany(GetSetBitPositions)
            );

            // Then do a region count by extracting neighbours from the first item in the set until the set is empty (like day 12)
            int regionCount;
            for (regionCount = 0; markedBits.Any(); regionCount++)
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed - using side effect of Remove
                FindConnectedBits(markedBits.First(), markedBits).Select(p => markedBits.Remove(p)).ToList();

            return regionCount;
        }

        public static IEnumerable<Tuple<int, int>> GetSetBitPositions(int hash, int index)
        {
            var uHash = (uint) hash;
            uint mask = 1;
            for (var i = 0; i < 8; i++)
            {
                if ((uHash & mask) == mask)
                    yield return Tuple.Create(index / 16, 8 * (1 + index%16) - i - 1);
                mask <<= 1;
            }
        }

        private static IEnumerable<Tuple<int,int>> FindConnectedBits(Tuple<int, int> first, HashSet<Tuple<int, int>> set)
        {
            // Perform a breadth-first search over the edges
            var frontier = new List<Tuple<int, int>> { first };
            var visited  = new HashSet<Tuple<int, int>>(frontier);

            IEnumerable<Tuple<int, int>> GetNeighbours(Tuple<int, int> position)
            {
                Tuple<int, int> p;
                p = Tuple.Create(position.Item1,     position.Item2 - 1); if ( set.Contains(p) ) yield return p;
                p = Tuple.Create(position.Item1 + 1, position.Item2    ); if ( set.Contains(p) ) yield return p;
                p = Tuple.Create(position.Item1,     position.Item2 + 1); if ( set.Contains(p) ) yield return p;
                p = Tuple.Create(position.Item1 - 1, position.Item2    ); if ( set.Contains(p) ) yield return p;
            }

            // expand the frontier - Add is storing the visited ones as a side effect
            while (frontier.Any())
                frontier = frontier.SelectMany(GetNeighbours).Where(f => visited.Add(f)).ToList();

            return visited;
        }
    }


    [TestFixture]
    internal class Day14Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day14.CountSquares("flqrgnkx");
            Assert.AreEqual(8108, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day14.CountRegions("flqrgnkx");
            Assert.AreEqual(1242, res);
        }
    }
}
