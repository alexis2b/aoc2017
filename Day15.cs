using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day15
    {
        public static void Run()
        {
            var res1 = CountJudgeMatches(699, 124, 40000000);
            Console.WriteLine($"Day15 - part1 - result: {res1}");

            var res2 = CountJudgeMatches2(699, 124, 5000000);
            Console.WriteLine($"Day15 - part2 - result: {res2}");
        }

        public static int CountJudgeMatches(int aStart, int bStart, int loops)
        {
            ulong a = (ulong) aStart;
            ulong b = (ulong) bStart;
            int match = 0;
            for (var i = 0; i < loops; i++)
            {
                a = (a * 16807) % 2147483647;
                b = (b * 48271) % 2147483647;
                if ((a & 0xFFFF) == (b & 0xFFFF))
                    match++;
            }

            return match;
        }

        public static int CountJudgeMatches2(int aStart, int bStart, int loops)
        {
            ulong a = (ulong) aStart;
            ulong b = (ulong) bStart;
            int match = 0;
            for (var i = 0; i < loops; i++)
            {
                do
                {
                    a = (a * 16807) % 2147483647;
                } while (a % 4 != 0);

                do
                {
                    b = (b * 48271) % 2147483647;
                } while (b % 8 != 0);

                if ((a & 0xFFFF) == (b & 0xFFFF))
                    match++;
            }

            return match;
        }
    }


    [TestFixture]
    internal class Day15Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day15.CountJudgeMatches(65, 8921, 5);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day15.CountJudgeMatches(65, 8921, 40000000);
            Assert.AreEqual(588, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day15.CountJudgeMatches2(65, 8921, 1055);
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day15.CountJudgeMatches2(65, 8921, 1056);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void Test2_3()
        {
            var res = Day15.CountJudgeMatches2(65, 8921, 5000000);
            Assert.AreEqual(309, res);
        }
    }
}
