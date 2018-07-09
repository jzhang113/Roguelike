// A collection of the enums used
namespace Roguelike.Enums
{
    public enum ActorState { Wander, Chase, Flee, Sleep, Dead }

    public enum ArmorType { Helmet, Armor, Gloves, Boots, RingLeft, RingRight }

    enum MessageLevel { Minimal, Normal, Verbose }

    public enum TargetShape { Self, Range, Ray, Directional}
}
