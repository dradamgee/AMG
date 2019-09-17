﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using AMG.FySics;
using AMG.Physics;

namespace GraphicsSandbox
{
    public class TreeNode<T>
    {
        public int Count { get; private set; }
        public T Key { get; }
        public Dictionary<T, TreeNode<T>> subNodes { get; }

        public TreeNode(T key)
        {
            Key = key;
            subNodes = new Dictionary<T, TreeNode<T>>();
        }

        public void AddNode(IEnumerable<T> node)
        {
            Count++;
            if (node.Any())
            {
                var key = node.First();
                var values = node.Skip(1);
                if (!subNodes.ContainsKey(key))
                {
                    subNodes.Add(key, new TreeNode<T>(key));
                }
                subNodes[key].AddNode(values);
            }
        }
    }



    public class TreeNodeBall : Ball
    {
        private readonly TreeNode<string> _node;
        
        public string _nodeName
        {
            get { return _node.Key; }
        }

        public static TreeNodeBall CreateBall(TreeNode<string> node, Vector location, Velocity velocity)
        {
            double mass = node.Count;
            var radius = Math.Sqrt(mass) * 5;
            return new TreeNodeBall(node, mass, radius, location, velocity);
        }

        public TreeNodeBall(TreeNode<string> node, double mass, double radius, Vector location, Velocity velocity) : base(mass, radius, location, velocity)
        {
            _node = node;
        }
        

        public override IEnumerable<Element> Split()
        {
            Random rand = new Random();

            return _node.subNodes.Select(subNode => CreateBall(subNode.Value, Location, new Velocity(new Vector(Velocity.Vector.X + rand.Next(10), Velocity.Vector.Y + rand.Next(10)))));
        }
    }


    public class Ball : Element{
        private double m_radius;

        public Ball(double mass, double radius, Vector location, Velocity velocity) : base(mass, location, velocity)
        {
            m_radius = radius;
        }

        public double Diameter {
            get { return m_radius * 2; }
        }

        public override IEnumerable<Element> Split()
        {
            var halfmass = Mass / 2;
            var halfsize = Math.Sqrt(m_radius * m_radius / 2);
            yield return new Ball(halfmass, halfsize, Location * 1.01, Velocity);
            yield return new Ball(halfmass, halfsize, Location * 0.99, Velocity);
        }

        public override double Radius {
            get { return m_radius; }
        }

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