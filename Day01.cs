using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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


    internal class Day01Tests
    {
        public void Test1_1()
        {
            var res = Day01.ComputeCaptcha1("1122");
            Debug.Assert(res == 3);
        }

        public void Test1_2()
        {
            var res = Day01.ComputeCaptcha1("1111");
            Debug.Assert(res == 4);
        }

        public void Test1_3()
        {
            var res = Day01.ComputeCaptcha1("1234");
            Debug.Assert(res == 0);
        }

        public void Test1_4()
        {
            var res = Day01.ComputeCaptcha1("91212129");
            Debug.Assert(res == 9);
        }



        public void Test2_1()
        {
            var res = Day01.ComputeCaptcha2("1212");
            Debug.Assert(res == 6);
        }

        public void Test2_2()
        {
            var res = Day01.ComputeCaptcha2("1221");
            Debug.Assert(res == 0);
        }

        public void Test2_3()
        {
            var res = Day01.ComputeCaptcha2("123425");
            Debug.Assert(res == 4);
        }

        public void Test2_4()
        {
            var res = Day01.ComputeCaptcha2("123123");
            Debug.Assert(res == 12);
        }

        public void Test2_5()
        {
            var res = Day01.ComputeCaptcha2("12131415");
            Debug.Assert(res == 4);
        }
    }
}
