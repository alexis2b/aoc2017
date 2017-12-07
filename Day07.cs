using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day07
    {
        private static readonly Regex RowEx = new Regex(@"(?<left>[a-z]+) \((?<weight>\d+)\)( -> )?(?<right>[a-z, ]+)?");

        public static void Run()
        {
            var input = File.ReadAllLines("input\\day07.txt").ToArray();

            var res1 = FindBottomProgram(input);
            Console.WriteLine($"Day07 - part1 - result: {res1}");

            var res2 = FindBalancingWeight(input);
            Console.WriteLine($"Day07 - part2 - result: bad node is {res2.Item1.Name}, weight is {res2.Item1.Weight} but should be {res2.Item2}");
        }

        public static string FindBottomProgram(string[] input)
        {
            // No need to build the tree here, we just keep track of all program names
            // on the left of an arrow (root) and on the right (child), the bottom program is the one which is not a child
            var left  = new HashSet<string>();
            var right = new HashSet<string>();
            foreach (var row in input)
            {
                var match = RowEx.Match(row);
                Debug.Assert(match.Success);

                left.Add(match.Groups["left"].Value);
                foreach(var r in match.Groups["right"].Value.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries))
                    right.Add(r);
            }

            return left.First(l => !right.Contains(l));
        }

        public static Tuple<Node, int> FindBalancingWeight(string[] input)
        {
            var tree     = BuildTree(input);
            var nodes    = tree.Item2.ToList();

            // search a node which is not balanced but for which all children are balanced
            var badFather = nodes.First(n => ! n.IsBalanced() && n.Children.All(c => c.IsBalanced()));

            // find which children is wrong
            var gChildren = badFather.Children.GroupBy(c => c.GetTotalWeight()).OrderBy(g => g.Count()).ToList();
            // we expect two elements in that list: 1st elemnt has count 1 and is the bad node, else has count (size-1) and contains the correct nodes
            Debug.Assert(gChildren.Count == 2);
            var badChild     = gChildren[0].First();
            var weightOffset = gChildren[1].Key - gChildren[0].Key;

            return Tuple.Create(badChild, badChild.Weight + weightOffset);
        }

        private static Tuple<Node,IEnumerable<Node>> BuildTree(IEnumerable<string> input)  // (root, all nodes)
        {
            // this time we must build the tree!
            var nodes = new Dictionary<string, Node>();

            foreach (var row in input)
            {
                var match = RowEx.Match(row);
                Debug.Assert(match.Success);

                // Left side - add and assign weight
                var leftName = match.Groups["left"].Value;
                if (!nodes.TryGetValue(leftName, out Node program))
                    nodes[leftName] = program = new Node(leftName);
                program.Weight = int.Parse(match.Groups["weight"].Value);

                // right sides, if any, allocate to left and create (with unknown weight for now)
                var rightNames = match.Groups["right"].Value.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var rightName in rightNames)
                {
                    if (!nodes.TryGetValue(rightName, out Node child))
                        nodes[rightName] = child = new Node(rightName);

                    program.Children.Add(child);
                }
            }

            // Find root node
            var lefts    = nodes.Values.Select(n => n.Name);
            var rights   = nodes.Values.SelectMany(n => n.Children).Select(n => n.Name).Distinct();
            var rootName = lefts.Except(rights).First();

            return new Tuple<Node, IEnumerable<Node>>(nodes[rootName], nodes.Values);
        }

        public class Node
        {
            private int? _totalWeight;

            public Node(string name)
            {
                Name     = name;
                Children = new List<Node>();
            }

            public string     Name     { get;      }
            public int        Weight   { get; set; }
            public List<Node> Children { get;      }

            public int GetTotalWeight()
            {
                if (!_totalWeight.HasValue)
                    _totalWeight = Children.Aggregate(Weight, (w, n) => w + n.GetTotalWeight()); // memo-ize for perfs

                return _totalWeight.Value;
            }

            public bool IsBalanced()
                => Children.Select(c => c.GetTotalWeight()).Distinct().Count() < 2;
        }
    }



    [TestFixture]
    internal class Day07Tests
    {
        private static readonly string[] Input = {
            "pbga (66)",
            "xhth (57)",
            "ebii (61)",
            "havc (66)",
            "ktlj (57)",
            "fwft (72) -> ktlj, cntj, xhth",
            "qoyq (66)",
            "padx (45) -> pbga, havc, qoyq",
            "tknk (41) -> ugml, padx, fwft",
            "jptl (61)",
            "ugml (68) -> gyxo, ebii, jptl",
            "gyxo (61)",
            "cntj (57)"
        };

        [Test]
        public void Test1_1()
        {
            var res = Day07.FindBottomProgram(Input);
            Assert.AreEqual("tknk", res);
        }
        [Test]
        public void Test1_2()
        {
            var res = Day07.FindBalancingWeight(Input);
            Assert.AreEqual("ugml", res.Item1.Name);
            Assert.AreEqual(68, res.Item1.Weight);
            Assert.AreEqual(60, res.Item2);
        }
    }
}
