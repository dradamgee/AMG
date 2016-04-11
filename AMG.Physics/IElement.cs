namespace AMG.Physics
{
    public interface IElement
    {
        Dimensions Location { get; set; }
        Velocity Velocity { get; set; }
        double Radius { get; }
    }
}
