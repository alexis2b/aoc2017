using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day04
    {
        public static void Run()
        {
            var res1 = CountValidPasswords(File.ReadAllLines("input\\day04.txt"));
            Console.WriteLine($"Day04 - part1 - result: {res1}");

            var res2 = CountValidPasswords2(File.ReadAllLines("input\\day04.txt"));
            Console.WriteLine($"Day04 - part2 - result: {res2}");
        }

        public static int CountValidPasswords(IEnumerable<string> passwords)
            => passwords.Where(IsValidPassword).Count();

        public static bool IsValidPassword(string password)
            => ! password.Split(' ', '\t').GroupBy(t => t).Any(g => g.Count() > 1);

        public static int CountValidPasswords2(IEnumerable<string> passwords)
            => passwords.Where(IsValidPassword2).Count();

        public static bool IsValidPassword2(string password)
            => ! password.Split(' ', '\t').Select(SortLetters).GroupBy(t => t).Any(g => g.Count() > 1);

        private static string SortLetters(string arg)
            => new string(arg.OrderBy(c => c).ToArray());
    }


    [TestFixture]
    internal class Day04Tests
    {
        [Test]
        public void Test1_1()
        {
            Assert.IsTrue(Day04.IsValidPassword("aa bb cc dd ee") );
        }

        [Test]
        public void Test1_2()
        {
            Assert.IsFalse(Day04.IsValidPassword("aa bb cc dd aa"));
        }

        [Test]
        public void Test1_3()
        {
            Assert.IsTrue(Day04.IsValidPassword("aa bb cc dd aaa"));
        }


        [Test]
        public void Test2_1()
        {
            Assert.IsTrue(Day04.IsValidPassword2("abcde fghij"));
        }

        [Test]
        public void Test2_2()
        {
            Assert.IsFalse(Day04.IsValidPassword2("abcde xyz ecdab"));
        }

        [Test]
        public void Test2_3()
        {
            Assert.IsTrue(Day04.IsValidPassword2("a ab abc abd abf abj"));
        }

        [Test]
        public void Test2_4()
        {
            Assert.IsTrue(Day04.IsValidPassword2("iiii oiii ooii oooi oooo"));
        }

        [Test]
        public void Test2_5()
        {
            Assert.IsFalse(Day04.IsValidPassword2("oiii ioii iioi iiio"));
        }
    }
}
