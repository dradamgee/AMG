using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using AMG.FySics;
using AMG.Physics;

namespace GraphicsSandbox
{
    public class Ball : ElementViewModel{


        public Ball(double mass, double radius, Vector location, Velocity velocity) : base(mass, location, velocity)
        {
            Radius = radius;
        }

        public double Diameter {
            get { return Radius * 2; }
        }

        public override IEnumerable<ElementViewModel> Split()
        {
            var halfmass = Mass / 2;
            var halfsize = Math.Sqrt(Radius * Radius / 2);
            yield return new Ball(halfmass, halfsize, Location * 1.01, Velocity);
            yield return new Ball(halfmass, halfsize, Location * 0.99, Velocity);
        }

        public override double Radius { get; }

        public static implicit operator Ball(string s)
        {
            var xx = s.Split('|');
            return new Ball(
                double.Parse(xx[0], CultureInfo.InvariantCulture),
                int.Parse(xx[1], CultureInfo.InvariantCulture),
                new Vector(double.Parse(xx[2], CultureInfo.InvariantCulture), double.Parse(xx[3], CultureInfo.InvariantCulture)), 
                new Velocity(
                    new Vector(double.Parse(xx[4], CultureInfo.InvariantCulture), double.Parse(xx[5], CultureInfo.InvariantCulture)
                    )));
        }
    }
}