using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace aoc2017
{
    internal class Day03
    {
        public static void Run()
        {
            // Check
            var tests = new Day03Tests();
            tests.Test1_1();

            var res1 = ComputeDistanceToPort(289326);
            Console.WriteLine($"Day03 - part1 - result: {res1}");

            // Check
            tests.Test2_1();

            var res2 = FirstValueWrittenLargerThan(289326);
            Console.WriteLine($"Day03 - part2 - result: {res2}");
        }

        public static int ComputeDistanceToPort(int target)
        {
            // Find the segment containing the target value
            var segment = Segment.First;
            while (segment.FinalValue < target)
                segment = segment.Next();

            var position = segment.GetValuePosition(target);

            return Math.Abs(position.Item1) + Math.Abs(position.Item2);
        }


        public static int FirstValueWrittenLargerThan(int target)
        {
            var spiral = new Dictionary<Tuple<int, int>, int>(); // (x,y) -> value
            spiral[new Tuple<int, int>(0, 0)] = 1;

            int GetCellValueOrZero(Tuple<int, int> position)
            {
                return spiral.TryGetValue(position, out int value) ? value : 0;
            }

            int AddCellToSpiral(Tuple<int, int> position) // returns the value
            {
                var val =
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 + 1, position.Item2 + 0)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 + 1, position.Item2 + 1)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 + 0, position.Item2 + 1)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 - 1, position.Item2 + 1)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 - 1, position.Item2 + 0)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 - 1, position.Item2 - 1)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 + 0, position.Item2 - 1)) +
                    GetCellValueOrZero(new Tuple<int, int>(position.Item1 + 1, position.Item2 - 1));


                Console.WriteLine("({0,3},{1,3}) -> {2,7}", position.Item1, position.Item2, val);
                return spiral[position] = val;
            }

            // Use the segments to build this large spiral
            var segment  = Segment.First;
            while (true)
            {
                for(int i=segment.StartValue; i < segment.FinalValue; i++)
                {
                    if (i == 1) continue; // first value has already been initialized to 1

                    var val = AddCellToSpiral(segment.GetValuePosition(i));
                    if (val >= target)
                        return val;
                }
                segment = segment.Next();
            }
        }

        internal class Segment
        {
            public enum Direction { right, up, left, down };
            public static readonly Segment First = new Segment
                { StartValue = 1, StartPosition = Tuple.Create(0, 0), SegmentDirection = Direction.right, Length = 2, NextSegmentLengthIncrease = 0 };
            public static Tuple<int, int>[] Velocity = new[]
            {
                Tuple.Create( 1,  0), // right
                Tuple.Create( 0,  1), // up
                Tuple.Create(-1,  0), // left
                Tuple.Create( 0, -1), // down
            };

            public int             StartValue       { get; private set; }
            public Tuple<int, int> StartPosition    { get; private set; } // x, y
            public Direction       SegmentDirection { get; private set; }
            public int             Length           { get; private set; }
            public int             NextSegmentLengthIncrease { get; private set; }
            public int             FinalValue => StartValue + Length - 1;


            // Constructs the next segment
            public Segment Next() => new Segment
                {
                    StartValue                = FinalValue,
                    StartPosition             = GetValuePosition(FinalValue),
                    SegmentDirection          = (Direction) (((int) SegmentDirection + 1) % 4),
                    Length                    = Length + NextSegmentLengthIncrease,
                    NextSegmentLengthIncrease = 1 - NextSegmentLengthIncrease  // 0,1 oscillator
                };


            public Tuple<int,int> GetValuePosition(int target)
            {
                if (target < StartValue || target > FinalValue)
                    throw new ArgumentException($"target must be between {StartValue} and {FinalValue}", "target");

                var distance = target - StartValue;

                return Tuple.Create(
                    StartPosition.Item1 + Velocity[(int)SegmentDirection].Item1 * distance,
                    StartPosition.Item2 + Velocity[(int)SegmentDirection].Item2 * distance
                );
            }
        }
    }


    internal class Day03Tests
    {
        public void Test1_1()
        {
            var res = Day03.ComputeDistanceToPort(1);
            Debug.Assert(res == 0);
        }

        public void Test1_2()
        {
            var res = Day03.ComputeDistanceToPort(12);
            Debug.Assert(res == 3);
        }

        public void Test1_3()
        {
            var res = Day03.ComputeDistanceToPort(23);
            Debug.Assert(res == 2);
        }

        public void Test1_4()
        {
            var res = Day03.ComputeDistanceToPort(1024);
            Debug.Assert(res == 31);
        }


        public void Test2_1()
        {
            var res = Day03.FirstValueWrittenLargerThan(2);
            Debug.Assert(res == 2);
        }

        public void Test2_2()
        {
            var res = Day03.FirstValueWrittenLargerThan(3);
            Debug.Assert(res == 4);
        }

        public void Test2_3()
        {
            var res = Day03.FirstValueWrittenLargerThan(58);
            Debug.Assert(res == 59);
        }

        public void Test2_4()
        {
            var res = Day03.FirstValueWrittenLargerThan(748);
            Debug.Assert(res == 806);
        }
    }
}
