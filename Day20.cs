using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace aoc2017
{
    internal class Day20
    {
        public static void Run()
        {
            var input = File.ReadAllLines("input\\day20.txt").ToList();
            
            var res1 = FindParticleClosestToOrigin(input, false).Item1;
            Console.WriteLine($"Day20 - part1 - result: {res1}");

            var res2 = FindParticleClosestToOrigin(input, true).Item2;
            Console.WriteLine($"Day20 - part2 - result: {res2}");
        }

        // (closest particle id, number of particles left)
        public static Tuple<int,int> FindParticleClosestToOrigin(IEnumerable<string> particleDescriptions, bool withCollisions)
        {
            var particles = particleDescriptions.Select((d, i) => Particle.FromDescription(i, d)).ToList();
            var closestParticleId       = -1;
            var closestParticleDuration = 0;

            while(closestParticleDuration < 1000) // particle must be the closest for 1000 iterations
            {
                // Remove particles that have collided
                if (withCollisions)
                {
                    var collidingParticles = particles.GroupBy(p => p.Position).Where(g => g.Count() > 1).SelectMany(g => g);
                    foreach (var p in collidingParticles)
                        particles.Remove(p);
                }

                foreach (var particle in particles)
                    particle.Move();

                var closestToOrigin = particles.OrderBy(p => p.DistanceToOrigin).First();
                if (closestToOrigin.Id != closestParticleId)
                    closestParticleDuration = 0; // reset the counter, we are tracking a new particle

                closestParticleId = closestToOrigin.Id;
                closestParticleDuration++;
            }

            return Tuple.Create(closestParticleId, particles.Count);
        }
    }

    public class Particle
    {
        private static readonly Regex DescriptionEx = new Regex(@"p=<(?<p>[\d-,]+)>, v=<(?<v>[\d-,]+)>, a=<(?<a>[\d-,]+)>");

        private readonly int[] _p;
        private readonly int[] _v;
        private readonly int[] _a;

        public int Id { get; }
        public int DistanceToOrigin => Math.Abs(_p[0]) + Math.Abs(_p[1]) + Math.Abs(_p[2]);
        public Tuple<int, int, int> Position => Tuple.Create(_p[0], _p[1], _p[2]);

        private Particle(int id, int[] p, int[] v, int[] a)
        {
            Id = id;
            _p = p;
            _v = v;
            _a = a;
        }

        public static Particle FromDescription(int id, string description)
        {
            var match = DescriptionEx.Match(description);
            Debug.Assert(match.Success);

            return new Particle(
                id,
                match.Groups["p"].Value.Split(',').Select(int.Parse).ToArray(),
                match.Groups["v"].Value.Split(',').Select(int.Parse).ToArray(),
                match.Groups["a"].Value.Split(',').Select(int.Parse).ToArray()
                );
        }

        public void Move()
        {
            _v[0] += _a[0];
            _v[1] += _a[1];
            _v[2] += _a[2];
            _p[0] += _v[0];
            _p[1] += _v[1];
            _p[2] += _v[2];
        }
    }



    [TestFixture]
    internal class Day20Tests
    {
        [Test]
        public void Test1_1()
        {
            string[] input =
            {
                "p=<3,0,0>, v=<2,0,0>, a=<-1,0,0>",
                "p=<4,0,0>, v=<0,0,0>, a=<-2,0,0>"
            };
            var res = Day20.FindParticleClosestToOrigin(input, false).Item1;
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Test2_1()
        {
            string[] input =
            {
                "p=<-6,0,0>, v=<3,0,0>, a=<0,0,0>",
                "p=<-4,0,0>, v=<2,0,0>, a=<0,0,0>",
                "p=<-2,0,0>, v=<1,0,0>, a=<0,0,0>",
                "p=<3,0,0>, v=<-1,0,0>, a=<0,0,0>"                                         
            };
            var res = Day20.FindParticleClosestToOrigin(input, true).Item2;
            Assert.AreEqual(1, res);
        }
    }
}
