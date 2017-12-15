using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day11
    {
        public static void Run()
        {
            var input = File.ReadAllText("input\\day11.txt").Split(',').ToList();

            var res1 = ShortestDistanceToTarget(input);
            Console.WriteLine($"Day11 - part1 - result: {res1}");

            var res2 = FarthestDistanceVisited(input);
            Console.WriteLine($"Day11 - part2 - result: {res2}");
        }

        public static int ShortestDistanceToTarget(IEnumerable<string> steps)
        {
            var target = FindTargetLocation(steps);
            var res    = FindShortestPathToTarget(target.Item1, target.Item2);
            return res;
        }

        private static Tuple<int, int> FindTargetLocation(IEnumerable<string> steps)
        {
            // Initialize from starting point
            var current      = Node.StartingNode;
            var visitedNodes = new HashSet<Node> {current};

            foreach (var step in steps)
                visitedNodes.Add(current = current.GetNextNode(step));

            return Tuple.Create(current.X, current.Y);
        }

        // Uses a breadth first algorithm
        private static int FindShortestPathToTarget(int targetX, int targetY)
        {
            var target          = new Node(targetX, targetY);
            var currentFrontier = new List<Node> { Node.StartingNode };
            var visited         = new HashSet<Node>(currentFrontier);
  
            // Search loop (breadth first)
            for (var steps = 0; ; steps++)
            {
                // Found?
                if (currentFrontier.Any(n => n.Equals(target))) return steps;
                // Expand the frontier (since Add returns false if element is already in, true means we only filter on the new ones)
                currentFrontier = currentFrontier.SelectMany(n => n.GetNeighbours()).Where(n => visited.Add(n)).ToList();
            }
        }

        public static int FarthestDistanceVisited(IEnumerable<string> steps)
        {
            var locations = FindVisitedLocations(steps);
            var res       = FindFarthestDistanceVisited(locations);
            return res;
        }

        private static HashSet<Node> FindVisitedLocations(IEnumerable<string> steps)
        {
            // Initialize from starting point
            var current = Node.StartingNode;
            var visitedNodes = new HashSet<Node> { current };

            foreach (var step in steps)
                visitedNodes.Add(current = current.GetNextNode(step));

            return visitedNodes;
        }

        // Breadth first, but we reach the goal when we have visited ALL the locations
        private static int FindFarthestDistanceVisited(IEnumerable<Node> locations)
        {
            var targets         = new HashSet<Node>(locations);
            var currentFrontier = new List<Node> { Node.StartingNode };
            var visited         = new HashSet<Node>(currentFrontier);

            // Search loop (breadth first)
            for (var steps = 0; ; steps++)
            {
                // Remove visited nodes from target locations
                foreach (var visitedNode in currentFrontier)
                    targets.Remove(visitedNode);
                // If None are left, this was the farthest!
                if (targets.Count == 0) return steps;
                // Expand the frontier (since Add returns false if element is already in, true means we only filter on the new ones)
                currentFrontier = currentFrontier.SelectMany(n => n.GetNeighbours()).Where(n => visited.Add(n)).ToList();
            }
        }


        public class Node : IEquatable<Node>
        {
            public static readonly Node StartingNode = new Node(0, 0);

            public Node(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; } // horizontal coordinates, increases to the right
            public int Y { get; } // vertical coordinations, increases to the top (one step North is +2!)

            public Node GetNextNode(string direction)
            {
                switch (direction)
                {
                    case "n" : return new Node(X,   Y+2);
                    case "ne": return new Node(X+1, Y+1);
                    case "se": return new Node(X+1, Y-1);
                    case "s" : return new Node(X,   Y-2);
                    case "sw": return new Node(X-1, Y-1);
                    case "nw": return new Node(X-1, Y+1);
                    default: throw new ArgumentException($"'{direction}' is not a valid direction", nameof(direction));
                }
            }

            public IEnumerable<Node> GetNeighbours()
            {
                return new[] {"n", "ne", "se", "s", "sw", "nw"}.Select(GetNextNode);
            }

            public bool Equals(Node other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return X == other.X && Y == other.Y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((Node) obj);
            }

            public override int GetHashCode() => (X * 397) ^ Y;
        }
    }


    [TestFixture]
    internal class Day11Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day11.ShortestDistanceToTarget(new [] {"ne", "ne", "ne"});
            Assert.AreEqual(3, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day11.ShortestDistanceToTarget(new [] { "ne", "ne", "sw", "sw" });
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Test1_3()
        {
            var res = Day11.ShortestDistanceToTarget(new [] { "ne", "ne", "s", "s" });
            Assert.AreEqual(2, res);
        }

        [Test]
        public void Test1_4()
        {
            var res = Day11.ShortestDistanceToTarget(new [] { "se", "sw", "se", "sw", "sw" });
            Assert.AreEqual(3, res);
        }

        // No tests provided for Part2! doing my own based on Part1 cases
        [Test]
        public void Test2_1()
        {
            var res = Day11.FarthestDistanceVisited(new[] { "ne", "ne", "ne" });
            Assert.AreEqual(3, res);
        }

        [Test]
        public void Test2_2()
        {
            var res = Day11.FarthestDistanceVisited(new[] { "ne", "ne", "sw", "sw" });
            Assert.AreEqual(2, res);
        }

        [Test]
        public void Test2_3()
        {
            var res = Day11.FarthestDistanceVisited(new[] { "ne", "ne", "s", "s" });
            Assert.AreEqual(2, res);
        }

        [Test]
        public void Test2_4()
        {
            var res = Day11.FarthestDistanceVisited(new[] { "se", "sw", "se", "sw", "sw" });
            Assert.AreEqual(3, res);
        }
    }
}
