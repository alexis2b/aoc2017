using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day08
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day08.txt").ToArray();

            var res1 = FindMaxRegisterValue(input);
            Console.WriteLine($"Day08 - part1 - result: {res1}");

            var res2 = FindMaxRegisterValueEver(input);
            Console.WriteLine($"Day08 - part2 - result: {res2}");
        }

        public static int FindMaxRegisterValue(string[] input)
        {
            var cpu = new Cpu();
            foreach (var row in input)
            {
                var instruction = Instruction.FromString(row);
                cpu.Execute(instruction);
            }

            return cpu.Registers.Values.Max();
        }

        public static int FindMaxRegisterValueEver(string[] input)
        {
            var cpu = new Cpu();
            var max = 0;
            foreach (var row in input)
            {
                var instruction = Instruction.FromString(row);
                cpu.Execute(instruction);
                max = Math.Max(max, cpu.Registers.Values.Max());
            }

            return max;
        }


        public class Cpu
        {
            private readonly Dictionary<string, int> _registers = new Dictionary<string, int>();

            public IReadOnlyDictionary<string, int> Registers => _registers;

            public void Execute(Instruction instruction)
            {
                // Is predicate true?
                if ( IsTrue(GetRegisterValue(instruction.PReg), instruction.POp, instruction.PVal) )
                {
                    // Apply action
                    var reg = instruction.AReg;
                    _registers[reg] = ResultOf(GetRegisterValue(reg), instruction.AOp, instruction.AVal);
                }
            }

            private int GetRegisterValue(string register) // will initialize at 0 on first call
            {
                if (!_registers.TryGetValue(register, out int val))
                    _registers[register] = val = 0;
                return val;
            }

            private static bool IsTrue(int lValue, string comparison, int rValue)
            {
                switch (comparison)
                {
                    case "==": return lValue == rValue;
                    case "!=": return lValue != rValue;
                    case ">":  return lValue >  rValue;
                    case ">=": return lValue >= rValue;
                    case "<":  return lValue <  rValue;
                    case "<=": return lValue <= rValue;
                    default:
                        throw new ArgumentException($"unknown comparison operator: {comparison}");
                }
            }

            private static int ResultOf(int lValue, string operation, int rValue)
            {
                switch (operation)
                {
                    case "inc": return lValue + rValue;
                    case "dec": return lValue - rValue;
                    default:
                        throw new ArgumentException($"unknown operation: {operation}");
                }
            }
        }

        public class Instruction
        {
            private static readonly Regex RowEx = new Regex(@"(?<ireg>[a-z]+) (?<opcode>inc|dec) (?<ival>-?\d+) if (?<preg>[a-z]+) (?<comp>[!=<>]+) (?<pval>-?\d+)");
            public string AReg { get; } // action register
            public string AOp  { get; } // action operation (inc or dec)
            public int    AVal { get; } // action right value
            public string PReg { get; } // condition register
            public string POp  { get; } // condition operation
            public int    PVal { get; } // condition right value


            private Instruction(string aReg, string aOp, int aVal, string pReg, string pOp, int pVal)
            {
                AReg = aReg;
                AOp  = aOp;
                AVal = aVal;
                PReg = pReg;
                POp  = pOp;
                PVal = pVal;
            }

            public static Instruction FromString(string text)
            {
                var match = RowEx.Match(text);
                Debug.Assert(match.Success);
                return new Instruction(
                    match.Groups["ireg"].Value,
                    match.Groups["opcode"].Value,
                    int.Parse(match.Groups["ival"].Value),
                    match.Groups["preg"].Value,
                    match.Groups["comp"].Value,
                    int.Parse(match.Groups["pval"].Value)
                );
            }
        }
    }



    [TestFixture]
    internal class Day08Tests
    {
        private static readonly string[] Input = {
            "b inc 5 if a > 1",
            "a inc 1 if b < 5",
            "c dec -10 if a >= 1",
            "c inc -20 if c == 10"
        };

        [Test]
        public void Test1_1()
        {
            var res = Day08.FindMaxRegisterValue(Input);
            Assert.AreEqual(1, res);
        }

        [Test]
        public void Test1_2()
        {
            var res = Day08.FindMaxRegisterValueEver(Input);
            Assert.AreEqual(10, res);
        }
    }
}
