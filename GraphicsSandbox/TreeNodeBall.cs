using System;
using System.Collections.Generic;
using System.Linq;
using AMG.FySics;

namespace GraphicsSandbox
{
    public class TreeNodeBall : Ball
    {
        private readonly TreeModule.TreeNode _node;
        
        public string NodeName
        {
            get { return _node.Handle; }
        }

        public TreeNodeBall (TreeModule.TreeNode node, Vector location, Velocity velocity) 
            : this(node, node.Count, Math.Sqrt(node.Count) * 3.6, location, velocity)
        {
            
        }

        public TreeNodeBall(TreeModule.TreeNode node, double mass, double radius, Vector location, Velocity velocity) : base(mass, radius, location, velocity)
        {
            _node = node;
        }
        

        public override IEnumerable<Element> Split()
        {
            Random rand = new Random();

            return _node.subNodes.Select(
                subNode => new TreeNodeBall(
                    subNode, 
                    Location + new Vector(rand.Next(100), rand.Next(100)), 
                    new Velocity(Velocity.Vector + new Vector(rand.Next(10), rand.Next(10)))
                    )
                );
        }
    }
}