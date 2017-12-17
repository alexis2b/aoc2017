using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day16
    {
        public static void Run()
        {
            var input = File.ReadAllText("input\\day16.txt").Split(',');

            var res1 = GetFinalPositions(16, input);
            Console.WriteLine($"Day16 - part1 - result: {res1}");

            var res2 = GetFinalPositions2(16, input, 1000000000);
            Console.WriteLine($"Day16 - part2 - result: {res2}");
        }

        public static string GetFinalPositions(int count, string[] steps, int repeat=1)
        {
            // build the list of programs a -> (n)
            var programs = new StringBuilder( Enumerable.Range(0, count).Aggregate(string.Empty, (s, c) => s + (char) (c+97)) );

            for(var i = 0; i < repeat; i++)
            foreach (var step in steps)
            {
                var kind = step[0];
                var data = step.Substring(1).Split('/');
                
                switch (kind)
                {
                    case 's':
                        var n = int.Parse(data[0]);
                        programs.Spin(n);
                        break;

                    case 'x':
                        var n1 = int.Parse(data[0]);
                        var n2 = int.Parse(data[1]);
                        programs.Swap(n1, n2);
                        break;

                    case 'p':
                        programs.Swap(data[0][0], data[1][0]);
                        break;
                }
            }

            return programs.ToString();
        }

        // Optimized implementation for part 2: find a cycle
        public static string GetFinalPositions2(int count, string[] steps, int repeat)
        {
            var start    = Enumerable.Range(0, count).Aggregate(string.Empty, (s, c) => s + (char) (c + 97));
            var programs = new StringBuilder(start);

            // Iterate but look for a cycle back to start
            var cycle = 0;
            for (var i = 0; i < repeat; i++)
            {
                foreach (var step in steps)
                {
                    var kind = step[0];
                    var data = step.Substring(1).Split('/');

                    switch (kind)
                    {
                        case 's':
                            var n = int.Parse(data[0]);
                            programs.Spin(n);
                            break;

                        case 'x':
                            var n1 = int.Parse(data[0]);
                            var n2 = int.Parse(data[1]);
                            programs.Swap(n1, n2);
                            break;

                        case 'p':
                            programs.Swap(data[0][0], data[1][0]);
                            break;
                    }
                }

                if (programs.ToString() == start)
                {
                    cycle = i;
                    break;
                }
            }

            if (cycle > 0) // we found a cycle, simplify!
                return GetFinalPositions2(count, steps, repeat % (cycle+1));

            return programs.ToString();
        }
    }

    public static class StringBuilderExtensions
    {
        public static void Spin(this StringBuilder sb, int n)
        {
            for (var i = 0; i < n; i++)
            {
                sb.Insert(0, sb[sb.Length - 1]);
                sb.Remove(sb.Length - 1, 1);
            }
        }

        public static void Swap(this StringBuilder sb, int n1, int n2)
        {
            var dummy = sb[n1];
            sb[n1] = sb[n2];
            sb[n2] = dummy;
        }

        public static void Swap(this StringBuilder sb, char c1, char c2)
        {
            var s = sb.ToString();
                sb.Swap(s.IndexOf(c1), s.IndexOf(c2));
        }
    }


    [TestFixture]
    internal class Day16Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day16.GetFinalPositions(5, new[] { "s1", "x3/4", "pe/b"});
            Assert.AreEqual("baedc", res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day16.GetFinalPositions(5, new[] { "s1", "x3/4", "pe/b"}, 2);
            Assert.AreEqual("ceadb", res);
        }

        [Test]
        public void Test2_2()
        {
            // performance testing, 1000 iterations should run in 10ms
            var res = Day16.GetFinalPositions2(5, new[] { "s1", "x3/4", "pe/b"}, 1000);
            Assert.AreEqual("abcde", res);
        }

        //[Test]
        //public void Test1_2()
        //{
        //    var res = Day16.CountJudgeMatches(65, 8921, 40000000);
        //    Assert.AreEqual(588, res);
        //}

        //[Test]
        //public void Test2_1()
        //{
        //    var res = Day16.CountJudgeMatches2(65, 8921, 1055);
        //    Assert.AreEqual(0, res);
        //}

        //[Test]
        //public void Test2_2()
        //{
        //    var res = Day16.CountJudgeMatches2(65, 8921, 1056);
        //    Assert.AreEqual(1, res);
        //}

        //[Test]
        //public void Test2_3()
        //{
        //    var res = Day16.CountJudgeMatches2(65, 8921, 5000000);
        //    Assert.AreEqual(309, res);
        //}
    }
}
