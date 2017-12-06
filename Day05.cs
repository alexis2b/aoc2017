using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day05
    {
        public static void Run()
        {
            var res1 = StepsToExit(File.ReadAllLines("input\\day05.txt").Select(int.Parse));
            Console.WriteLine($"Day05 - part1 - result: {res1}");

            var res2 = StepsToExit2(File.ReadAllLines("input\\day05.txt").Select(int.Parse));
            Console.WriteLine($"Day05 - part2 - result: {res2}");
        }


        public static int StepsToExit(IEnumerable<int> instructions)
        {
            var program     = instructions.ToArray();
            var stepCounter = 0;
            var ip          = 0; // instgruction pointer

            while (true)
            {
                ip += program[ip]++;
                stepCounter++;

                if (ip < 0 || ip >= program.Length)
                    break;
            }

            return stepCounter;
        }

        public static int StepsToExit2(IEnumerable<int> instructions)
        {
            var program     = instructions.ToArray();
            var stepCounter = 0;
            var ip          = 0; // instgruction pointer

            while (true)
            {
                var offset = program[ip];
                if (offset < 3) program[ip]++; else program[ip]--; // new rule
                ip += offset;
                stepCounter++;

                if (ip < 0 || ip >= program.Length)
                    break;
            }

            return stepCounter;
        }
    }


    [TestFixture]
    internal class Day05Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day05.StepsToExit(new[] {0, 3, 0, 1, -3});
            Assert.AreEqual(5, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day05.StepsToExit2(new[] {0, 3, 0, 1, -3});
            Assert.AreEqual(10, res);
        }
    }
}
