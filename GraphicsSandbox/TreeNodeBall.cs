using System;
using System.Collections.Generic;
using System.Linq;
using AMG.FySics;

namespace GraphicsSandbox
{
    public class TreeNode
    {
        public int Count { get; private set; }
        public string Handle { get; }
        public string FullPath { get; }

        public Dictionary<string, TreeNode> subNodes { get; }

        public TreeNode(string handle, string fullPath)
        {
            Handle = handle;
            FullPath = fullPath;
            subNodes = new Dictionary<string, TreeNode>();
        }

        public void AddNode(IEnumerable<string> node)
        {
            Count++;
            if (node.Any())
            {
                var handle = node.First();
                var values = node.Skip(1);
                if (!subNodes.ContainsKey(handle))
                {
                    subNodes.Add(handle, new TreeNode(handle, FullPath + '/' + handle));
                }
                subNodes[handle].AddNode(values);
            }
        }
    }

    public class TreeNodeBall : Ball
    {
        private readonly TreeNode _node;
        
        public string NodeName
        {
            get { return _node.Handle; }
        }

        public TreeNodeBall (TreeNode node, Vector location, Velocity velocity) 
            : this(node, node.Count, Math.Sqrt(node.Count) * 3.6, location, velocity)
        {
            
        }

        public TreeNodeBall(TreeNode node, double mass, double radius, Vector location, Velocity velocity) : base(mass, radius, location, velocity)
        {
            _node = node;
        }
        

        public override IEnumerable<Element> Split()
        {
            Random rand = new Random();

            return _node.subNodes.Select(
                subNode => new TreeNodeBall(
                    subNode.Value, 
                    Location + new Vector(rand.Next(100), rand.Next(100)), 
                    new Velocity(Velocity.Vector + new Vector(rand.Next(10), rand.Next(10)))
                    )
                );
        }
    }
}