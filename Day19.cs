using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day19
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day19.txt").ToArray();

            var res = TraverseMaze(input);
            Console.WriteLine($"Day19 - part1 - result: {res.Item1}");
            Console.WriteLine($"Day19 - part2 - result: {res.Item2}");
        }

        public static Tuple<string,int> TraverseMaze(string[] m)
        {
            // (r,c) -> current position
            // (dr, dc) -> velocity vector
            // sb -> letter collector
            // s -> steps counter
            int r = 0, dr = 1, c = m[r].IndexOf('|'), dc = 0;
            int maxR = m.Length - 1, maxC = m[0].Length - 1;
            var sb = new StringBuilder();
            var s = 0;

            while(true)
            {
                // move
                s++;
                r += dr; c += dc;
                if (r < 0 || r > maxR || c < 0 || c > maxC )
                    break; // the end!

                var next = m[r][c];
                if (next == ' ')
                    break; // exit!
                if (next >= 'A' && next <= 'Z')
                    sb.Append(next);
                if (next == '+')
                {
                    int ndr = dr, ndc = dc;
                    // look for the next direction but don't go backward!
                    if (dc != -1 && c < maxC && (m[r][c + 1] == '-' || (m[r][c + 1] >= 'A' && m[r][c + 1] <= 'Z'))) { ndr =  0; ndc =  1; }
                    if (dc !=  1 && c > 0    && (m[r][c - 1] == '-' || (m[r][c - 1] >= 'A' && m[r][c - 1] <= 'Z'))) { ndr =  0; ndc = -1; }
                    if (dr != -1 && r < maxR && (m[r + 1][c] == '|' || (m[r + 1][c] >= 'A' && m[r + 1][c] <= 'Z'))) { ndr =  1; ndc =  0; }
                    if (dr !=  1 && r > 0    && (m[r - 1][c] == '|' || (m[r - 1][c] >= 'A' && m[r - 1][c] <= 'Z'))) { ndr = -1; ndc =  0; }
                    dr = ndr; dc = ndc;
                }
            }

            return Tuple.Create(sb.ToString(), s);
        }
    }



    [TestFixture]
    internal class Day19Tests
    {
        private static readonly string[] Input =
        {
            "      |          ",
            "      |  +--+    ",
            "      A  |  C    ",
            "  F---|----E|--+ ",
            "      |  |  |  D ",
            "      +B-+  +--+ "
        };


        [Test]
        public void Test1_1()
        {
            var res = Day19.TraverseMaze(Input).Item1;
            Assert.AreEqual("ABCDEF", res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day19.TraverseMaze(Input).Item2;
            Assert.AreEqual(38, res);
        }
    }
}
