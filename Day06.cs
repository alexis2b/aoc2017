using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day06
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\Day06.txt").SelectMany(l => l.Split(' ', '\t')).Select(int.Parse).ToArray();

            var res1 = TurnsBeforeCycle(input);
            Console.WriteLine($"Day06 - part1 - result: {res1}");

            var res2 = TurnsBeforeCycle2(input);
            Console.WriteLine($"Day06 - part2 - result: {res2.Item2}");
        }

        public static int TurnsBeforeCycle(IEnumerable<int> initialState)
        {
            var knownStates  = new HashSet<MemoryState>();
            var currentState = new MemoryState(initialState);
            int turns;

            for (turns = 0; knownStates.Add(currentState); turns++)
                currentState = currentState.Next();

            return turns;
        }

        public static Tuple<int, int> TurnsBeforeCycle2(IEnumerable<int> initialState)
        {
            var knownStates = new HashSet<MemoryState>();
            var currentState = new MemoryState(initialState);
            int turns;

            for (turns = 0; knownStates.Add(currentState); turns++)
                currentState = currentState.Next();

            var previous = knownStates.First(currentState.Equals);

            return Tuple.Create(turns, turns - previous.Index);
        }
    }

    // Immutable
    // Index added for part2 but not part of the Equality Key
    internal class MemoryState : IEquatable<MemoryState>
    {
        private readonly int[] _state;

        public MemoryState(IEnumerable<int> state, int index=0)
        {
            _state = state.ToArray();
            Index  = index;
        }

        public int Index { get; }

        public MemoryState Next()
        {
            // find bank index with max elements with Select( (value, index) => ) trick
            var newState   = _state.ToArray(); // clone
            var maxBank    = _state.Select((v, i) => Tuple.Create(v, i)).OrderByDescending(t => t.Item1).ThenBy(t => t.Item2).First().Item2;
            var reallocate = _state[maxBank];
            newState[maxBank] = 0;
            for (var i = 1; i <= reallocate; i++)
                newState[(maxBank + i) % _state.Length]++;

            return new MemoryState(newState, Index+1);
        }

        public bool Equals(MemoryState other)
        {
            return other != null && _state.SequenceEqual(other._state);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MemoryState);
        }

        public override int GetHashCode()
        {
            return _state.Aggregate(_state.Length.GetHashCode(), (current, item) => current ^ item.GetHashCode());
        }
    }


    [TestFixture]
    internal class Day06Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day06.TurnsBeforeCycle(new[] {0, 2, 7, 0});
            Assert.AreEqual(5, res);
        }

        [Test]
        public void Test2_1()
        {
            var res = Day06.TurnsBeforeCycle2(new[] { 0, 2, 7, 0 });
            Assert.AreEqual(5, res.Item1);
            Assert.AreEqual(4, res.Item2);
        }
    }
}
