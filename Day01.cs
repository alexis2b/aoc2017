using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day01
    {
        public static void Run()
        {
            // Check
            var tests = new Day01Tests();
            tests.Test1_1();
            tests.Test1_2();
            tests.Test1_3();
            tests.Test1_4();

            var res1 = ComputeCaptcha1(File.ReadAllText("input\\day01.txt"));
            Console.WriteLine($"Day01 - part1 - result: {res1}");

            // Check
            tests.Test2_1();
            tests.Test2_2();
            tests.Test2_3();
            tests.Test2_4();
            tests.Test2_5();

            var res2 = ComputeCaptcha2(File.ReadAllText("input\\day01.txt"));
            Console.WriteLine($"Day01 - part2 - result: {res2}");
        }

        public static int ComputeCaptcha1(string input)
        {
            // Zip the sequence with itself shifted by 1 character (and the last added at the end),
            // select only the tuples where both characters are equals
            return
                input.Zip((input + input[0]).Skip(1), Tuple.Create) // create (c1, c2) tuples with first character added as c2 of the last tuple
                    .Where(t => t.Item1 == t.Item2)                 // only take tuples where (c1, c2) are the same
                        .Select(t => t.Item1 - '0')                 // char to int
                            .Sum();                                 // aggregate
        }

        public static int ComputeCaptcha2(string input)
        {
            // build the second sequence
            var halfpoint = input.Length / 2;
            var input2    = input.Substring(halfpoint) + input.Substring(0, halfpoint);

            // Zip the sequence with itself shifted by 1 character (and the last added at the end),
            // select only the tuples where both characters are equals
            return
                input.Zip(input2, Tuple.Create)                     // create (c1, c2) tuples with first character added as c2 of the last tuple
                    .Where(t => t.Item1 == t.Item2)                 // only take tuples where (c1, c2) are the same
                        .Select(t => t.Item1 - '0')                 // char to int
                            .Sum();                                 // aggregate
        }
    }


    [TestFixture]
    internal class Day01Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day01.ComputeCaptcha1("1122");
            Assert.AreEqual(3, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day01.ComputeCaptcha1("1111");
            Assert.AreEqual(4, res);
        }

        [Test]
        public void Test1_3()
        {
            var res = Day01.ComputeCaptcha1("1234");
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Test1_4()
        {
            var res = Day01.ComputeCaptcha1("91212129");
            Assert.AreEqual(9, res);
        }



        [Test]
        public void Test2_1()
        {
            var res = Day01.ComputeCaptcha2("1212");
            Assert.AreEqual(6, res);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day01.ComputeCaptcha2("1221");
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Test2_3()
        {
            var res = Day01.ComputeCaptcha2("123425");
            Assert.AreEqual(4, res);
        }

        [Test]
        public void Test2_4()
        {
            var res = Day01.ComputeCaptcha2("123123");
            Assert.AreEqual(12, res);
        }

        [Test]
        public void Test2_5()
        {
            var res = Day01.ComputeCaptcha2("12131415");
            Assert.AreEqual(4, res);
        }
    }
}
