using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day25
    {
        public static void Run()
        {
            var res1 = GetDiagnosticChecksum(12302209);
            Console.WriteLine($"Day25 - part1 - result: {res1}");
            // no part2 on day 25: all done \o/ !
        }

        public static int GetDiagnosticChecksum(int iterations, bool isTest=false)
        {
            var m = new TuringMachine(isTest);
            for(var it = 0; it < iterations; it++)
                m.Next();
            return m.DiagnosticChecksum;
        }
    }

    // InstructionsProd implemented as per my Day 25 instructions, your mileage may vary
    public class TuringMachine
    {
        private readonly Dictionary<int,int> _tape;  // not an efficient storage mechanism but much simpler memory management and is sufficient for this puzzle
        private int  _cursor;
        private char _state;
        private readonly Tuple<int, int, char>[,] _instructions;

        public int DiagnosticChecksum => _tape.Values.Sum();

        // State machine description: turing[initialState,currentValue] -> { value to write, cursor increment, next state }
        private static readonly Tuple<int, int, char>[,] InstructionsProd =
        { // Current State - Current Value 0         - Current Value 1
          /* A           */ {Tuple.Create(1,  1, 'B'), Tuple.Create(0, -1, 'D')},
          /* B           */ {Tuple.Create(1,  1, 'C'), Tuple.Create(0,  1, 'F')},
          /* C           */ {Tuple.Create(1, -1, 'C'), Tuple.Create(1, -1, 'A')},
          /* D           */ {Tuple.Create(0, -1, 'E'), Tuple.Create(1,  1, 'A')},
          /* E           */ {Tuple.Create(1, -1, 'A'), Tuple.Create(0,  1, 'B')},
          /* F           */ {Tuple.Create(0,  1, 'C'), Tuple.Create(0,  1, 'E')}
        };
        private static readonly Tuple<int, int, char>[,] InstructionsTest =
        { // Current State - Current Value 0         - Current Value 1
          /* A           */ {Tuple.Create(1,  1, 'B'), Tuple.Create(0, -1, 'B')},
          /* B           */ {Tuple.Create(1, -1, 'A'), Tuple.Create(1,  1, 'A')},
        };

        public TuringMachine(bool isTest=false)
        {
            _tape   = new Dictionary<int, int>();
            _cursor = 0;
            _state  = 'A';
            _instructions = isTest ? InstructionsTest : InstructionsProd;
        }

        public void Next()
        {
            _tape.TryGetValue(_cursor, out int val);
            var instruction = _instructions[_state - 'A', val];
            _tape[_cursor]  = instruction.Item1;
            _cursor        += instruction.Item2;
            _state          = instruction.Item3;
        }
    }


    [TestFixture]
    internal class Day25Tests
    {
        [Test]
        public void Test1_1()
        {
            var res = Day25.GetDiagnosticChecksum(6, true);
            Assert.AreEqual(3, res);
        }
    }
}
