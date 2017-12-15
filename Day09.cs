using System;
using System.IO;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day09
    {
        public static void Run()
        {
            var input = File.ReadAllText("input\\day09.txt");

            var res = FindGroupsCountAndScoreAndGarbageCount(input);
            Console.WriteLine($"Day09 - part1 - result: {res.Item2} (in {res.Item1} groups)");
            Console.WriteLine($"Day09 - part2 - result: {res.Item3}");
        }

        public static Tuple<int,int,int> FindGroupsCountAndScoreAndGarbageCount(string input)
        {
            var currentDepth = 0;
            var groupCount = 0;
            var accumulatedDepth = 0;
            var isInGarbage = false;
            var garbageCount = 0;
            for(var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if ( c == '!' )
                {
                    // cancellation, skip next
                    i++;
                }
                else if (isInGarbage)
                {
                    // will we still be in garbage mode next char ('>' ends it)
                    isInGarbage = (c != '>');
                    garbageCount += isInGarbage ? 1 : 0; // do not count '>' in the garbage count
                }
                else // we have content
                {
                    if ( c == '{' )
                    {
                        // group open
                        currentDepth++;
                        groupCount++;
                        accumulatedDepth += currentDepth;
                    }
                    if ( c == '}' )
                    {
                        // group close
                        currentDepth--;
                    }
                    if (c == '<')
                    {
                        // garbage from now on
                        isInGarbage = true;
                    }
                }
            }

            return Tuple.Create(groupCount, accumulatedDepth, garbageCount);
        }
    }



    [TestFixture]
    internal class Day09Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{}");
            Assert.AreEqual(1, res.Item1);
            Assert.AreEqual(1, res.Item2);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{{}}}");
            Assert.AreEqual(3, res.Item1);
            Assert.AreEqual(6, res.Item2);
        }

        [Test]
        public void Test1_3()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{},{}}");
            Assert.AreEqual(3, res.Item1);
            Assert.AreEqual(5, res.Item2);
        }

        [Test]
        public void Test1_4()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{{},{},{{}}}}");
            Assert.AreEqual(6, res.Item1);
            Assert.AreEqual(16, res.Item2);
        }

        [Test]
        public void Test1_5()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<{},{},{{}}>}");
            Assert.AreEqual(1, res.Item1);
        }

        [Test]
        public void Test1_6()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<a>,<a>,<a>,<a>}");
            Assert.AreEqual(1, res.Item1);
            Assert.AreEqual(1, res.Item2);
        }

        [Test]
        public void Test1_7()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{<a>},{<a>},{<a>},{<a>}}");
            Assert.AreEqual(5, res.Item1);
        }

        [Test]
        public void Test1_8()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{<!>},{<!>},{<!>},{<a>}}");
            Assert.AreEqual(2, res.Item1);
        }

        [Test]
        public void Test1_9()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{<ab>},{<ab>},{<ab>},{<ab>}}");
            Assert.AreEqual(5, res.Item1);
            Assert.AreEqual(9, res.Item2);
        }

        [Test]
        public void Test1_10()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{<!!>},{<!!>},{<!!>},{<!!>}}");
            Assert.AreEqual(5, res.Item1);
            Assert.AreEqual(9, res.Item2);
        }

        [Test]
        public void Test1_11()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{{<a!>},{<a!>},{<a!>},{<ab>}}");
            Assert.AreEqual(2, res.Item1);
            Assert.AreEqual(3, res.Item2);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<>}");
            Assert.AreEqual(0, res.Item3);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<random characters>}");
            Assert.AreEqual(17, res.Item3);
        }

        [Test]
        public void Test2_3()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<<<<>}");
            Assert.AreEqual(3, res.Item3);
        }

        [Test]
        public void Test2_4()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<{!>}>}");
            Assert.AreEqual(2, res.Item3);
        }

        [Test]
        public void Test2_5()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<!!>}");
            Assert.AreEqual(0, res.Item3);
        }

        [Test]
        public void Test2_6()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<!!!>>}");
            Assert.AreEqual(0, res.Item3);
        }

        [Test]
        public void Test2_7()
        {
            var res = Day09.FindGroupsCountAndScoreAndGarbageCount("{<{o\"i!a,<{i<a>}");
            Assert.AreEqual(10, res.Item3);
        }
    }
}
