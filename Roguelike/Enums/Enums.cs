// A collection of the enums used
namespace Roguelike.Enums
{
    public enum ActorState { Wander, Chase, Flee, Sleep, Dead }

    public enum ArmorType { Helmet, Armor, Gloves, Boots, RingLeft, RingRight }

    internal enum MessageLevel { Minimal, Normal, Verbose }

    public enum TargetShape { Area, Directional, Range, Ray, Self }
}
