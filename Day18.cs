using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day18
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day18.txt").ToList();

            var res1 = GetFirstRecoveredSound(input);
            Console.WriteLine($"Day18 - part1 - result: {res1}");

            var res2 = GetSecondProgramSendCounter(input);
            Console.WriteLine($"Day18 - part2 - result: {res2}");
        }

        public static long GetFirstRecoveredSound(IEnumerable<string> instructions)
        {
            var computer = new Computer();
            computer.Execute(instructions);
            return computer.FirstRecoveredSound;
        }

        public static long GetSecondProgramSendCounter(List<string> instructions)
        {
            var programs = new[]
            {
                new ProgramInstance(0, instructions),
                new ProgramInstance(1, instructions)
            };

            var active = 0;
            while (true)
            {
                programs[active].Execute( programs[1-active].SendBuffer );
                if (programs[active].SendBuffer.Count == 0)
                    return programs[1].SendCounter;
                active = 1 - active;
            }
        }
    }

    public class Computer
    {
        private readonly Dictionary<string, long> _registers = new Dictionary<string, long>();

        public long LastPlayedSound { get; private set; }
        public long LastRecoveredSound { get; private set; }
        public long FirstRecoveredSound { get; private set; }

        public Computer()
        {
            // simpler if all registers are pre-initialized
            for (var c = 'a'; c <= 'z'; c++)
                _registers[c.ToString()] = 0;
        }

        public void Execute(IEnumerable<string> instructions)
        {
            var code = instructions.ToArray();
            var ip   = 0;
            while (ip < code.Length)
            {
                var instr = code[ip].Split(' ');
                switch (instr[0])
                {
                    case "snd":
                        var val = _registers[instr[1]];
                        LastPlayedSound = val;
                        break;

                    case "set":
                        _registers[instr[1]] = GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "add":
                        _registers[instr[1]] += GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "mul":
                        _registers[instr[1]] *= GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "mod":
                        _registers[instr[1]] %= GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "rcv":
                        if (GetConstantOrRegisterValue(instr[1]) != 0)
                        {
                            LastRecoveredSound = LastPlayedSound;
                            if (FirstRecoveredSound == 0)
                                FirstRecoveredSound = LastPlayedSound;
                            ip = code.Length; // exit!
                        }
                        break;

                    case "jgz":
                        if (GetConstantOrRegisterValue(instr[1]) > 0)
                        {
                            ip += int.Parse(instr[2]) - 1;
                        }
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


    public class ProgramInstance
    {
        private readonly string[] _code;
        private int _ip;
        private readonly Dictionary<string, long> _registers = new Dictionary<string, long>();

        public ProgramInstance(int id, IEnumerable<string> code)
        {
            _code      = code.ToArray();
            _ip        = 0;
            SendBuffer = new List<long>();

            // simpler if all registers are pre-initialized
            for (var c = 'a'; c <= 'z'; c++)
                _registers[c.ToString()] = 0;
            _registers["p"] = id;
        }

        public int SendCounter { get; private set; }
        public List<long> SendBuffer { get; }

        public void Execute(List<long> recvBuffer)
        {
            while (_ip < _code.Length)
            {
                var instr = _code[_ip].Split(' ');
                switch (instr[0])
                {
                    case "snd":
                        var val = GetConstantOrRegisterValue(instr[1]);
                        SendBuffer.Add(val);
                        SendCounter++;
                        break;

                    case "set":
                        _registers[instr[1]] = GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "add":
                        _registers[instr[1]] += GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "mul":
                        _registers[instr[1]] *= GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "mod":
                        _registers[instr[1]] %= GetConstantOrRegisterValue(instr[2]);
                        break;

                    case "rcv":
                        if (recvBuffer.Count > 0)
                        {
                            _registers[instr[1]] = recvBuffer[0];
                            recvBuffer.RemoveAt(0);
                            break;
                        }
                        else
                            return; // nothing to receive, will come back to receive since IP has not changed

                    case "jgz":
                        if (GetConstantOrRegisterValue(instr[1]) > 0)
                        {
                            _ip += (int) GetConstantOrRegisterValue(instr[2]) - 1;
                        }
                        break;

                    default:
                        throw new ArgumentException($"unknown opcode ${instr[0]}");
                }
                _ip++;
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
    internal class Day18Tests
    {


        [Test]
        public void Test1_1()
        {
            var input = new [] {
                "set a 1",
                "add a 2",
                "mul a a",
                "mod a 5",
                "snd a",
                "set a 0",
                "rcv a",
                "jgz a -1",
                "set a 1",
                "jgz a -2"
            };
            var res = Day18.GetFirstRecoveredSound(input);
            Assert.AreEqual(4, res);
        }

        [Test]
        public void Test2_1()
        {
            var input = new List<string> {
                "snd 1",
                "snd 2",
                "snd p",
                "rcv a",
                "rcv b",
                "rcv c",
                "rcv d"
            };
            var res = Day18.GetSecondProgramSendCounter(input);
            Assert.AreEqual(3, res);
        }
    }
}
