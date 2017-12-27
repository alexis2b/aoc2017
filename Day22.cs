using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day22
    {
        // clockwise direction starting from North
        public static Tuple<int,int>[] Directions =
        {
            Tuple.Create( 0, -1), // North
            Tuple.Create( 1,  0), // East
            Tuple.Create( 0,  1), // South
            Tuple.Create(-1,  0)  // West
        };
        public enum State { Clean, Weakened, Infected, Flagged };

        public static void Run()
        {
            var input = File.ReadAllLines("input\\day22.txt").ToList();

            var res1 = CountInfectionBursts(input, 10000);
            Console.WriteLine($"Day22 - part1 - result: {res1}");

            var res2 = CountInfectionBursts2(input, 10000000);
            Console.WriteLine($"Day22 - part2 - result: {res2}");
        }


        public static int CountInfectionBursts(IEnumerable<string> initialMap, int iterations)
        {
            var res    = InitializeMap(initialMap.ToList());
            var nodes  = res.Item1;
            var width  = res.Item2;
            var height = res.Item3;
            var pos    = Tuple.Create(width / 2, height / 2);
            var dir    = 0;
            var infectionBursts = 0;

            for (var it = 0; it < iterations; it++)
            {
                if (nodes.Contains(pos)) // is infected?
                {
                    dir = (dir+1) % Directions.Length; // turn right
                    nodes.Remove(pos); // cleanup
                }
                else
                {
                    dir = ((dir - 1) + Directions.Length) % Directions.Length;
                    nodes.Add(pos); // infect
                    infectionBursts++;
                }
                // move forward
                pos = Tuple.Create(pos.Item1 + Directions[dir].Item1, pos.Item2 + Directions[dir].Item2);
            }

            return infectionBursts;
        }

        // second part
        public static int CountInfectionBursts2(IEnumerable<string> initialMap, int iterations)
        {
            var res    = InitializeMap(initialMap.ToList());
            var nodes  = res.Item1.ToDictionary(n => n, n => State.Infected);
            var width  = res.Item2;
            var height = res.Item3;
            var pos    = Tuple.Create(width / 2, height / 2);
            var dir    = 0;
            var infectionBursts = 0;

            for (var it = 0; it < iterations; it++)
            {
                if ( ! nodes.TryGetValue(pos, out State state))
                    nodes[pos] = state = State.Clean;

                switch (state)
                {
                    case State.Clean   : /* turn left  */ dir = ((dir - 1) + Directions.Length) % Directions.Length; break;
                    case State.Weakened: /* no change  */ infectionBursts++; break;
                    case State.Infected: /* turn right */ dir = (dir + 1) % Directions.Length; break;
                    case State.Flagged : /* reverse    */ dir = (dir + 2) % Directions.Length; break;
                }

                nodes[pos] = (State) (((int) state + 1) % 4);

                // move forward
                pos = Tuple.Create(pos.Item1 + Directions[dir].Item1, pos.Item2 + Directions[dir].Item2);
            }

            return infectionBursts;
        }


        // returns { infected nodes set, width, height }
        private static Tuple<HashSet<Tuple<int,int>>, int, int> InitializeMap(IReadOnlyList<string> initialMap)
        {
            var nodes = new HashSet<Tuple<int, int>>();
            for (var y = 0; y < initialMap.Count; y++)
            {
                var row = initialMap[y];
                for (var x = 0; x < row.Length; x++)
                    if (row[x] == '#')
                        nodes.Add(Tuple.Create(x, y)); // add infected node
            }
            return Tuple.Create(nodes, initialMap[0].Length, initialMap.Count);
        }
    }


    [TestFixture]
    internal class Day22Tests
    {
        private static readonly string[] Input =
        {
            "..#",
            "#..",
            "..."
        };

        [Test]
        public void Test1_1()
        {
            var res = Day22.CountInfectionBursts(Input, 1);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day22.CountInfectionBursts(Input, 7);
            Assert.AreEqual(5, res);
        }

        [Test]
        public void Test1_3()
        {
            var res = Day22.CountInfectionBursts(Input, 70);
            Assert.AreEqual(41, res);
        }

        [Test]
        public void Test1_4()
        {
            var res = Day22.CountInfectionBursts(Input, 10000);
            Assert.AreEqual(5587, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day22.CountInfectionBursts2(Input, 100);
            Assert.AreEqual(26, res);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day22.CountInfectionBursts2(Input, 10000000);
            Assert.AreEqual(2511944, res);
        }
    }
}
