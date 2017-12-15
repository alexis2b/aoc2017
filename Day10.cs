using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day10
    {
        public static void Run()
        {
            var input1 = File.ReadAllText("input\\day10.txt").Split(',').Select(int.Parse).ToArray();
            var res1 = TieKnotAndMultiplyFirstTwoNumbers(256, input1);
            Console.WriteLine($"Day10 - part1 - result: {res1}");

            var input2 = File.ReadAllText("input\\day10.txt");
            var res2 = GetDenseKnotHash(input2);
            Console.WriteLine($"Day10 - part2 - result: {res2}");
        }

        public static int TieKnotAndMultiplyFirstTwoNumbers(int length, int[] input)
        {
            var knot = TieKnot1(length, input); // we can afford the "expensive implementation for part 1
            return knot[0] * knot[1];
        }

        // relatively straight-forward but super expensive because of all the LinQ and cloning going around
        private static int[] TieKnot1(int size, int[] input)
        {
            // trick, we do not move a cursor in the array (the "current position" in the problem, but rather move the list so
            // that the current position is always index 0 - not a problem since the list is circular)
            var knot = Enumerable.Range(0, size).ToArray();
            var zeroOffset = 0; // because I move the array instead of the current position I need to keep track of where my 0 index is!
            for(var i = 0; i < input.Length; i++) // i corresponds to input and also to skip size
            {
                var length = input[i];
                if ( length > 1 )
                    knot = knot.Take(length).Reverse().Union(knot.Skip(length)).ToArray(); // reverse "sub" elements from the beginning
                // move forward current position by length + skip size (i) - we actually move the array so that current position is 0
                var offset = (length + i) % size;
                if ( offset > 0 )
                    knot = knot.Skip(offset).Union(knot.Take(offset)).ToArray();
                zeroOffset = (zeroOffset - offset) % size;
            }
            // put back the "zero index" first
            knot = knot.Skip(zeroOffset+size).Union(knot.Take(zeroOffset+size)).ToArray();

            return knot;
        }

        public static string GetDenseKnotHash(string inputStr)
        {
            // Build the input numers: ASCII of the input string + a fixed "salt"
            var input = inputStr.Select(c => (int) c).Concat(new[] {17, 31, 73, 47, 23}).ToArray();

            // Run 64 rounds
            var builder = new KnotBuilder(256);
            for(var i=0; i<64; i++)
                foreach(var val in input)
                    builder.Tie(val);

            // Densify the hash by XORing the values by blocks of 16
            var sparseHash = builder.Buffer;
            var denseHash  = new int[builder.Buffer.Length / 16];
            for (var i = 0; i < denseHash.Length; i++)
                denseHash[i] = sparseHash.Skip(16 * i).Take(16).Aggregate((a, n) => a ^ n);

            // Hexadecimalize
            var denseHashStr = denseHash.Aggregate("", (a, h) => a + h.ToString("x2"));

            return denseHashStr;

        }

        // Emulates a circular buffer with a "reverse" function for this exercise
        public class KnotBuilder
        {
            private readonly int[] _buffer;
            private int _cursor;
            private int _skipSize;

            /// <summary>Builds the know builder with "size" numbers from 0 to (size-1)</summary>
            public KnotBuilder(int size)
            {
                _buffer = Enumerable.Range(0, size).ToArray();
            }

            /// <summary>Get a copy of the current buffer state</summary>
            public int[] Buffer => _buffer.ToArray();

            /// <summary>Perform one step of the knot by applying the input value (reverse then skip)</summary>
            public void Tie(int input)
            {
                Reverse(input);
                _cursor = IndexOf(_cursor + input + _skipSize++);
            }

            private void Reverse(int length)
            {
                for (var i = 0; i < length / 2; i++) // swap two elements
                    SwapValues(IndexOf(_cursor + i), IndexOf(_cursor + length - 1 - i));
            }

            private int IndexOf(int i) => i % _buffer.Length;

            private void SwapValues(int i1, int i2)
            {
                var dummy = _buffer[i1];
                _buffer[i1] = _buffer[i2];
                _buffer[i2] = dummy;
            }
        }
    }





    [TestFixture]
    internal class Day10Tests
    {
        [Test]
        public void Test1_1()
        {
            var input = new[] {3, 4, 1, 5};
            var res = Day10.TieKnotAndMultiplyFirstTwoNumbers(5, input);
            Assert.AreEqual(12, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day10.GetDenseKnotHash("");
            Assert.AreEqual("a2582a3a0e66e6e86e3812dcb672a272", res);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day10.GetDenseKnotHash("AoC 2017");
            Assert.AreEqual("33efeb34ea91902bb2f59c9920caa6cd", res);
        }

        [Test]
        public void Test2_3()
        {
            var res = Day10.GetDenseKnotHash("1,2,3");
            Assert.AreEqual("3efbe78a8d82f29979031a4aa0b16a9d", res);
        }

        [Test]
        public void Test2_4()
        {
            var res = Day10.GetDenseKnotHash("1,2,4");
            Assert.AreEqual("63960835bcdc130f0b66d7ff4f6a5a8e", res);
        }
    }
}
