using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day13
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day13.txt").ToList();

            var res1 = TripSeverity(input);
            Console.WriteLine($"Day13 - part1 - result: {res1}");

            var res2 = FirstSafeTrip(input);
            Console.WriteLine($"Day13 - part2 - result: {res2}");
        }

        public static int TripSeverity(IEnumerable<string> input)
        {
            var fw      = GetFirewall(input);
            var fwState = Enumerable.Range(0, fw.Length).Select(i => fw[i] > 0 ? 1 : 0).ToArray(); // starts at 1 except where there is no range
            var fwDir   = Enumerable.Range(0, fw.Length).Select(i => fw[i] > 1 ? 1 : 0).ToArray(); // all existing scanners move down (+1) except where there is no scanner (0) or only 1 depth (no move range)

            var severity = 0;
            for (var position = 0; position < fw.Length; position++)
            {
                // Hit check
                severity += fwState[position] == 1 ? position * fw[position] : 0;

                // Update state - move by direction
                for (var i = 0; i < fwState.Length; i++)
                {
                    fwState[i] += fwDir[i];
                    if (fwState[i] == 0 && fwDir[i] != 0) // bounce from top
                    {
                        fwState[i] = 2;
                        fwDir[i]   = 1;
                    }
                    else if (fwState[i] > fw[i]) // bounce from bottom
                    {
                        fwState[i] = fw[i] - 1;
                        fwDir[i]   = -1;
                    }
                }
            }

            return severity;
        }

        // Better approach that is not quadratic!
        public static int TripSeverity2(IEnumerable<string> input)
        {
            var ranges  = GetFirewall(input);
            var periods = ranges.Select(r => r + Math.Max(r - 2, 0)).ToArray(); // "period" of a scanner oscillation is: r + max(r-2, 0) where r is the "range" of the scanner
            var length  = ranges.Length;

            int ScannerPosition(int depth, int time)
            {
                var stepInPeriod = ranges[depth] <= 1 ? ranges[depth] : time % periods[depth] + 1; // position in a full cycle
                var position     = stepInPeriod <= ranges[depth] ? stepInPeriod : 2 * ranges[depth] - stepInPeriod;  // oscillator
                return position;
            }

            // calculate severity in one go, the index range corresponds to time and position
            var s = Enumerable.Range(0, length) // iterate over d(depth) which also corresponds to t(ime) (start together)
                .Select(d => Tuple.Create(d, ScannerPosition(d, d))) // create { d, p(osition) } tuples for where we intersect with each scanner
                .Where(v => v.Item2 == 1) // only keep the elements where we have been intercepted by the scanner
                .Aggregate(0, (a,v) => a + v.Item1 * ranges[v.Item1] ); // sum( d*r ) for each interception

            return s;
        }

        // variation on the previous one, adding a loop over an incremental "delay" which is the difference between depth and time
        public static int FirstSafeTrip(IEnumerable<string> input)
        {
            var ranges  = GetFirewall(input);
            var periods = ranges.Select(r => r + Math.Max(r - 2, 0)).ToArray(); // "period" of a scanner oscillation is: r + max(r-2, 0) where r is the "range" of the scanner
            var length  = ranges.Length;

            int ScannerPosition(int depth, int time)
            {
                var stepInPeriod = ranges[depth] <= 1 ? ranges[depth] : time % periods[depth] + 1; // position in a full cycle
                var position = stepInPeriod <= ranges[depth] ? stepInPeriod : 2 * ranges[depth] - stepInPeriod;  // oscillator
                return position;
            }

            for (var delay = 0; ; delay++)
            {
                // did we hit?
                var hit = Enumerable.Range(0, length) // iterate over d(depth), since we have a delay, the scanner time is d(epth) + delay
                    .Select(d => ScannerPosition(d, delay+d)) // we only need the position this time
                    .Any(v => v == 1);

                if (!hit) return delay;
            }
        }

        private static int[] GetFirewall(IEnumerable<string> input)
        {
            var depths = input
                .Select(i => i.Split(new[] {": "}, StringSplitOptions.None))
                .Select(s => Tuple.Create(int.Parse(s[0]), int.Parse(s[1]))).ToList();

            var maxDepth = depths.Max(d => d.Item1);
            var res = new int[maxDepth + 1];
            foreach (var d in depths)
                res[d.Item1] = d.Item2;

            return res;
        }
    }


    [TestFixture]
    internal class Day13Tests
    {
        private static readonly string[] Input = {
            "0: 3",
            "1: 2",
            "4: 4",
            "6: 4"
        };

        [Test]
        public void Test1_1()
        {
            var res = Day13.TripSeverity(Input);
            Assert.AreEqual(24, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day13.TripSeverity2(Input);
            Assert.AreEqual(24, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day13.FirstSafeTrip(Input);
            Assert.AreEqual(10, res);
        }
    }
}
