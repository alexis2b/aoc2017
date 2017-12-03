using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace aoc2017
{
    internal class Day02
    {
        public static void Run()
        {
            // Check
            var tests = new Day02Tests();
            tests.Test1_1();

            var res1 = ComputeChecksum1(File.ReadAllLines("input\\Day02.txt"));
            Console.WriteLine($"Day02 - part1 - result: {res1}");

            // Check
            tests.Test2_1();

            var res2 = ComputeChecksum2(File.ReadAllLines("input\\Day02.txt"));
            Console.WriteLine($"Day02 - part2 - result: {res2}");
        }

        public static int ComputeChecksum1(string[] input)
        {
            int LineMinMaxDifference(string line)
            {
                return line.Split('\t').Select(int.Parse).Max() - line.Split('\t').Select(int.Parse).Min();
            }

            return input.Select(LineMinMaxDifference).Sum();
        }

        public static int ComputeChecksum2(string[] input)
        {
            int EvenDivisionOfNumbers(string line)
            {
                var numbers = line.Split('\t').Select(int.Parse).ToList();
                numbers.Sort();
                numbers.Reverse();
                for (int i1 = 0; i1 < numbers.Count - 1; i1++)
                    for (int i2 = i1 + 1; i2 < numbers.Count; i2++)
                        if ( numbers[i1] % numbers[i2] == 0 )
                            return numbers[i1] / numbers[i2];

                throw new InvalidDataException($"Failed to find an even division in {line}");
            }

            return input.Select(EvenDivisionOfNumbers).Sum();
        }
    }


    internal class Day02Tests
    {
        public void Test1_1()
        {
            var res = Day02.ComputeChecksum1(new [] { "5\t1\t9\t5", "7\t5\t3", "2\t4\t6\t8" });
            Debug.Assert(res == 18);
        }


        public void Test2_1()
        {
            var res = Day02.ComputeChecksum2(new[] { "5\t9\t2\t8", "9\t4\t7\t3", "3\t8\t6\t5" });
            Debug.Assert(res == 9);
        }
    }
}
