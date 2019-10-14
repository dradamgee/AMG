using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMG.FySics;
using Microsoft.FSharp.Collections;

namespace GraphicsSandbox
{
    public class God // not sure if God should be static, or maybe immutable.
    {
        private static double accelerationDueToGravity = -98.0;
        private static int NumberOfBalls = 10;
        private static int BallSize = 20;
        private static double loss = 1.0;
        private static double viscosity = 0.01;

        public static Universe CreateUniverseFromFile(string path)
        {
            var lines = System.IO.File.ReadLines(path);
            return UniverseFromPaths(lines);
        }

        public static Universe UniverseFromPaths(IEnumerable<string> lines)
        {
            var universe = new Universe(accelerationDueToGravity, loss, viscosity);
            
            var asd = ListModule.OfSeq((lines.Select(
                        line => ListModule.OfSeq(line.Split('/')
                    ))));

            var rootNode = TreeModule.buildRoot(asd);
            
            var nodeBall = new TreeNodeBall(rootNode, new Vector(400, 400), new Velocity(new Vector(0, 0)));
            
            foreach (var element in nodeBall.Split())
            {
                var leash = new Leash(new Vector(500.0, 10.0), nodeBall.Radius * 1.1, 10000.0);
                var leashVM = new LeashViewModel(leash, element);
                universe.Add(element);
                universe.Add(leashVM);
            }


            return universe;
        }

        public static Universe CreateUniverse() {
            var universe = new Universe(accelerationDueToGravity, loss, viscosity);
            
            int i = NumberOfBalls;
            while (i-- > 0) {
                universe.Add(NewBall());
            }

            return universe;
        }

        public static  Universe CreateUniverseToFixRotationError() {
            var universe = new Universe(accelerationDueToGravity, loss, viscosity);
            var e1 = (Ball)"1 | 8 | 196.5716629592 | 225.237471658389 | 4.96841730694 | 5.77965926300911";
            var e2 = (Ball)"1 | 8 | 185.307610553024 | 214.62117119859 | 15.1709404889631 | -5.65428572085577";
            universe.Add(e1);
            universe.Add(e2);
            return universe;
        }

        public static Universe CreateUniverseWithTwoBalls()
        {
            var universe = new Universe(0.0, 1.0, 0.0);
            var e1 = (Ball)"1 | 20 | 0.0 | 400.0 | 250.0 | 0.0";
            var e2 = (Ball)"1 | 20 | 400.0 | 400.0 | 0.0 | 0.0";
            var e3 = (Ball)"1 | 20 | 440.0 | 400.0 | 0.0 | 0.0";
            var e4 = (Ball)"1 | 20 | 480.0 | 400.0 | 0.0 | 0.0";
            var e5 = (Ball)"1 | 20 | 520.0 | 400.0 | 0.0 | 0.0";
            universe.Add(e1);
            universe.Add(e2);
            universe.Add(e3);
            universe.Add(e4);
            universe.Add(e5);
            return universe;
        }

        public static Universe CreateUniverseBallCollidesWithTwoBalls()
        {
            var universe = new Universe(0.0, 1.0, 0.0);
            var e1 = (Ball)"1 | 20 | 0.0 | 400.0 | 250.0 | 0.0";
            var e2 = (Ball)"1 | 20 | 400.0 | 380.0 | 0.0 | 0.0";
            var e3 = (Ball)"1 | 20 | 400.0 | 420.0 | 0.0 | 0.0";
            var e4 = (Ball)"1 | 20 | 480.0 | 400.0 | 0.0 | 0.0";
            var e5 = (Ball)"1 | 20 | 520.0 | 400.0 | 0.0 | 0.0";
            universe.Add(e1);
            universe.Add(e2);
            universe.Add(e3);
            //universe.Add(e4);
            //universe.Add(e5);
            return universe;
        }

        private static Square NewSquare() {
            return new Square(1.0d, 12, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
        }

        private static Ball NewBall() {
            return new Ball(1.0d, BallSize, new Vector(RandomX, RandomY), new Velocity(new Vector(10, 20)));
        }

        static Random random = new Random();

        private static int RandomX
        {
            get { return random.Next(525); }
        }
        private static int RandomY
        {
            get { return random.Next(350); }
        }
        
    }
}