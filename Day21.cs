using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day21
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day21.txt").ToList();
            
            var res1 = CountPixelsAfterTransform(input, 5);
            Console.WriteLine($"Day21 - part1 - result: {res1}");

            var res2 = CountPixelsAfterTransform(input, 18);
            Console.WriteLine($"Day21 - part2 - result: {res2}");
        }

        public static int CountPixelsAfterTransform(IEnumerable<string> rules, int iterations)
        {
            var mapping = rules
                .Select(d => d.Split(new[] {" => "}, StringSplitOptions.None))
                .ToDictionary(s => Pattern.FromString(s[0]), s => Pattern.FromString(s[1]));
            var picture = Pattern.FirstBlock;


            for (var it = 0; it < iterations; it++)
                picture = NextPicture(picture, mapping);

            // find all 1s
            var sum = 0;
            for(var d1 = 0; d1 < picture.GetLength(0); d1++)
            for (var d2 = 0; d2 < picture.GetLength(1); d2++)
                sum += picture[d1, d2];

            return sum;
        }

        private static int[,] NextPicture(int[,] pic, IReadOnlyDictionary<Pattern, Pattern> mapping)
        {
            var n = pic.GetLength(0);      // number of elements in one side
            var s = n % 2 == 0 ? 2 : 3;    // size of the element blocks "2x2" or 3x3"
            var d = n / s;                 // number of blocks in one side
            var r = new int[n + d, n + d]; // new picture increases by 1 element for d block, so total elements increase by d on each side
            // for each block, extract the elements, map into the new pattern, and apply them into the new picture
            for (var d1 = 0; d1 < d; d1++)
            for (var d2 = 0; d2 < d; d2++)
                Apply(r, d1 * (s + 1), d2 * (s + 1), mapping[Extract(pic, d1 * s, d2 * s, s)]);
            return r;
        }

        private static Pattern Extract(int[,] picture, int r0, int c0, int size)
        {
            var res = new int[size, size];
            for (var r = 0; r < size; r++)
            for (var c = 0; c < size; c++)
                res[r,c] = picture[r+r0, c+c0];
            return new Pattern(res);
        }

        private static void Apply(int[,] picture, int r0, int c0, Pattern pattern)
        {
            var src = pattern.Block;
            for (var r = 0; r < src.GetLength(0); r++)
            for (var c = 0; c < src.GetLength(1); c++)
                picture[r+r0, c+c0] = src[r, c];
        }
    }

    public class Pattern : IEquatable<Pattern>
    {
        public static int[,] FirstBlock => new[,] {{0, 1, 0}, {0, 0, 1}, {1, 1, 1}};

        public int[,] Block { get; }

        public Pattern(int[,] block)
        {
            Block = block;
        }

        public static Pattern FromString(string description)
        {
            var size  = description.Length == 5 ? 2 : description.Length == 11 ? 3 : 4;
            var block = new int[size, size];
            var dummy = description
                .Replace("/", string.Empty)
                .Select((c, i) => block[i/size, i%size] = c == '#' ? 1 : 0)
                .ToList(); // execute
            return new Pattern(block);
        }

        public bool Equals(Pattern other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Patterns are equals even after flip or rotation
            int[,] r1, r2, r3;
            if (AreEquals(Block, other.Block)) return true;
            if (AreEquals(Block, r1 = Rotate(other.Block))) return true;
            if (AreEquals(Block, FlipH(r1))) return true;
            if (AreEquals(Block, FlipV(r1))) return true;
            if (AreEquals(Block, r2 = Rotate(r1))) return true;
            if (AreEquals(Block, FlipH(r2))) return true;
            if (AreEquals(Block, FlipV(r2))) return true;
            if (AreEquals(Block, r3 = Rotate(r2))) return true;
            if (AreEquals(Block, FlipH(r3))) return true;
            if (AreEquals(Block, FlipV(r3))) return true;

            return false;
        }

        private static bool AreEquals(int[,] block1, int[,] block2)
        {
            if (block1.GetLength(0) != block2.GetLength(0) ||
                block1.GetLength(1) != block2.GetLength(1))
                return false;

            for (var d1 = 0; d1 < block1.GetLength(0); d1++)
                for (var d2 = 0; d2 < block1.GetLength(1); d2++)
                    if (block1[d1, d2] != block2[d1, d2])
                        return false;

            return true;
        }

        // 90° rotation clockwise
        private static int[,] Rotate(int[,] block)
        {
            var res = new int[block.GetLength(1), block.GetLength(0)];
            for (var d1 = 0; d1 < block.GetLength(0); d1++)
            for (var d2 = 0; d2 < block.GetLength(1); d2++)
                res[d2,block.GetLength(0)-d1-1] = block[d1, d2];
            return res;
        }

        private static int[,] FlipH(int[,] block)
        {
            var res = new int[block.GetLength(0), block.GetLength(1)];
            for (var d1 = 0; d1 < block.GetLength(0); d1++)
            for (var d2 = 0; d2 < block.GetLength(1); d2++)
                res[d1,block.GetLength(1)-d2-1] = block[d1, d2];
            return res;
        }

        private static int[,] FlipV(int[,] block)
        {
            var res = new int[block.GetLength(0), block.GetLength(1)];
            for (var d1 = 0; d1 < block.GetLength(0); d1++)
            for (var d2 = 0; d2 < block.GetLength(1); d2++)
                res[block.GetLength(0)-d1-1, d2] = block[d1, d2];
            return res;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Pattern) obj);
        }

        public override int GetHashCode()
        {
            var res = Block.Length;
            for (var d1 = 0; d1 < Block.GetLength(0); d1++)
            for (var d2 = 0; d2 < Block.GetLength(1); d2++)
                res ^= (31*Block[d1, d2]) ^ (9*(d1 + 1)) ^ (13*(d2+1));
            return res;
        }
    }


    [TestFixture]
    internal class Day21Tests
    {
        [Test]
        public void Test1_1()
        {
            string[] input =
            {
                "../.# => ##./#../...",
                ".#./..#/### => #..#/..../..../#..#"
            };
            var res = Day21.CountPixelsAfterTransform(input, 2);
            Assert.AreEqual(12, res);
        }
    }
}
