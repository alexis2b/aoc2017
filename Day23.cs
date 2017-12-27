using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day23
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day23.txt").ToList();

            var res1 = GetMulExecutionCount(input);
            Console.WriteLine($"Day23 - part1 - result: {res1}");

            var res2 = Part2Code();
            Console.WriteLine($"Day23 - part2 - result: {res2}");
        }

        public static long GetMulExecutionCount(IEnumerable<string> instructions)
        {
            var computer = new Computer2();
            computer.Execute(instructions);
            return computer.MulExecutionsCount;
        }

        public static long Part2Code()
        {
            long h = 0;

            for (long b = 109300; b <= 126300; b += 17)
            {
                long f = 1;

                for (long d = 2; d < b; d++)
                {
                    // set f=0 if there is an e in (2 .. b-1) such that e*d == b
                    // => means that b is divisible by d, can be quickly checked with a modulo
                    if (b % d == 0)
                    {
                        f = 0;
                        break;
                    }
                }

                if (f == 0)
                    h++;
            }

            return h;
        }
    }

    public class Computer2
    {
        private readonly Dictionary<string, long> _registers = new Dictionary<string, long>();

        public long MulExecutionsCount { get; private set; }

        public Computer2()
        {
            // simpler if all registers are pre-initialized
            for (var c = 'a'; c <= 'z'; c++)
                _registers[c.ToString()] = 0;
        }

        public void Execute(IEnumerable<string> instructions)
        {
            var code = instructions.ToArray();
            var ip = 0;
            while (ip < code.Length)
            {
                var instr = code[ip].Split(' ');
                switch (instr[0])
                {
                    case "set":
                        _registers[instr[1]] = GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "sub":
                        _registers[instr[1]] -= GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "mul":
                        _registers[instr[1]] *= GetConstantOrRegisterValue(instr[2]);
                        MulExecutionsCount++;
                        break;

                    case "jnz":
                        if (GetConstantOrRegisterValue(instr[1]) != 0)
                            ip += int.Parse(instr[2]) - 1;
                        break;

                    default:
                        throw new ArgumentException($"unknown opcode ${instr[0]}");
                }

                ip++;
            }
        }

        private long GetConstantOrRegisterValue(string val)
        {
            if (val.Length == 1 && val[0] >= 'a' && val[0] <= 'z')
                return _registers[val];
            return int.Parse(val);
        }
    }

    [TestFixture]
    internal class Day23Tests
    {
        // no given tests on that day
    }
}
