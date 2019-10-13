using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMG.FySics;

namespace AMG.Physics {
    //public class QuadTreeCollisionDetector : ICollisionDetector {
    //    private readonly IEnumerable<Element> _elements;
    //    Boundry boundry;


    //    public QuadTreeCollisionDetector(IEnumerable<Element> elements, Boundry boundry) {
    //        _elements = elements;
    //        this.boundry = boundry;
    //    }

    //    public IEnumerable<Tuple<Element, Element>> Detect(){
    //        var rootNode = new QuadTreeNode(1, new Rectangle(0.0d, boundry.Size.X, 0.0d, boundry.Size.Y));

    //        foreach (var element in _elements)
    //        {
    //            rootNode.Add(element);
    //        }

    //        return rootNode.Detect();
    //    }

    //}

    //public class position
    //{
    //    public double X;
    //    public double Y;
    //}

    //public class Rectangle
    //{
    //    public double X1;
    //    public double X2;
    //    public double Y1;
    //    public double Y2;

    //    position Middle;

    //    public Rectangle(double x1, double x2, double y1, double y2)
    //    {
    //        X1 = x1;
    //        X2 = x2;
    //        Y1 = y1;
    //        Y2 = y2;
    //        Middle = new position() { X = (X1 + X2) / 2.0d, Y = (Y1 + Y2) / 2.0d, };
    //    }

    //    public Rectangle Quadrant1 { get { return new Rectangle(X1, Middle.X, Y1, Middle.Y); } }
    //    public Rectangle Quadrant2 { get { return new Rectangle(Middle.X, X2, Y1, Middle.Y); } }
    //    public Rectangle Quadrant3 { get { return new Rectangle(Middle.X, X2, Middle.Y, Y2); } }
    //    public Rectangle Quadrant4 { get { return new Rectangle(X1, Middle.X, Middle.Y, Y2); } }
        
    //}


    //public class QuadTreeNode {
    //    int Capacity;
    //    Rectangle Area;
    //    QuadTreeNode[] Subnodes;
    //    QuadTreeNode ParentNode;

    //    public QuadTreeNode(int capacity, Rectangle area, QuadTreeNode parentNode = null) {
    //        Capacity = capacity;
    //        Area = area;
    //        ParentNode = parentNode;
    //    }

    //    private List<Element> Elements = new List<Element>();

    //    private bool IsDivided {
    //        get {
    //            return Subnodes != null;
    //        }
    //    }

    //    public bool PushDown(Element element) {
    //        return Subnodes.Any(qtn => qtn.Add(element));
    //    }

    //    public bool PushUp(Element element) {
    //        if (ParentNode == null) {
    //            return false;
    //        }
    //        Elements.Remove(element);
    //        return ParentNode.PushUpTarget(element);
    //    }

    //    public bool PushUpTarget(Element element) {
    //        if (Add(element))
    //        {
    //            return true;
    //        }
    //        if (ParentNode == null) {
    //            return false; // Perhaps throw an exception now.
    //        }
    //        return ParentNode.PushUpTarget(element);
    //    }
        
    //    private void Divide() {
    //        if (!IsDivided)
    //        {
    //            Subnodes = new QuadTreeNode[4]
    //            {
    //                new QuadTreeNode(Capacity, Area.Quadrant1), 
    //                new QuadTreeNode(Capacity, Area.Quadrant2), 
    //                new QuadTreeNode(Capacity, Area.Quadrant3), 
    //                new QuadTreeNode(Capacity, Area.Quadrant4), 
    //            };

    //            TryPushDownAll();
    //        }
    //    }

    //    private void TryPushDownAll()
    //    {
    //        var pushedDown = Elements.Where(PushDown).ToArray();

    //        foreach (var element in pushedDown)
    //        {
    //            Elements.Remove(element);
    //        }
    //    }

    //    private bool Encaptulates(Element element) {
    //        return
    //            Area.Y1 < element.Top
    //            && Area.Y2 > element.Bottom
    //            && Area.X1 < element.Left
    //            && Area.X2 > element.Right;
    //    }

    //    public bool Add(Element element) {
    //        if (Subnodes != null && Subnodes.Any(qtn => qtn.Add(element))) {
    //            return true;
    //        }

    //        if (Encaptulates(element)) 
    //        {
    //            Elements.Add(element);

    //            if (!IsDivided && Elements.Count > Capacity)
    //            {
    //                Divide();
    //            }

    //            return true;
    //        }
    //        return false;
    //    }

    //    public void Recalculate()
    //    {
    //        if (IsDivided) 
    //        {
    //            foreach (var qtn in Subnodes)
    //            {
    //                qtn.Recalculate();
    //            }
    //        }

    //        var pushedUp = Elements.Where(e => !Encaptulates(e)).Where(PushUp).ToArray();

    //        foreach (var element in pushedUp) {
    //            Elements.Remove(element);
    //        }

    //        TryPushDownAll();
    //    }

    //    public IEnumerable<Element> SubTreeElements
    //    {
    //        get
    //        {
    //            return IsDivided ? Subnodes.SelectMany(qtn => qtn.Elements) : Enumerable.Empty<Element>();
    //        }
    //    }

    //    public IEnumerable<Tuple<Element, Element>> Detect() {
    //        var combinations = from item1 in Elements
    //                           from item2 in Elements
    //                           where item1.Id < item2.Id
    //                           select Tuple.Create(item1, item2);

    //        var subCombinations = from item1 in Elements
    //                              from item2 in SubTreeElements
    //                              select Tuple.Create(item1, item2);

    //        var subSubConbinations = IsDivided ? Subnodes.SelectMany(qtn => qtn.Detect()) : Enumerable.Empty<Tuple<Element, Element>>();

    //        return combinations.Concat(subCombinations).Concat(subSubConbinations);
    //    }
        
    //}
}
