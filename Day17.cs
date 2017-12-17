using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day17
    {
        public static void Run()
        {
            var res1 = GetNextValueAfterLastInserted(369, 2017);
            Console.WriteLine($"Day17 - part1 - result: {res1}");

            var res2 = GetNextValueAfterZero(369, 50000000);
            Console.WriteLine($"Day17 - part2 - result: {res2}");
        }

        public static int GetNextValueAfterLastInserted(int steps, int lastValue)
        {
            var buffer = new List<int> { 0 };
            var pos    = 0;
            for (var i = 1; i <= lastValue; i++)
            {
                // step circularly (steps) time and insert
                pos = (pos + steps) % buffer.Count + 1;
                buffer.Insert(pos, i);
            }
            return buffer[pos + 1];
        }

        // optimized version for part 2, we only need to keep track of what we insert in position 1
        // (no need to retain the full buffer)
        public static int GetNextValueAfterZero(int steps, int lastValue)
        {
            var res = 0;
            var pos = 0;
            for (var i = 1; i <= lastValue; i++)
            {
                // step circularly (steps) time and insert
                pos = (pos + steps) % i + 1;
                if (pos == 1)
                    res = i;
            }
            return res;
        }
    }


    [TestFixture]
    internal class Day17Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day17.GetNextValueAfterLastInserted(3, 2017);
            Assert.AreEqual(638, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day17.GetNextValueAfterZero(3, 2017);
            Assert.AreEqual(1226, res);
        }
    }
}
