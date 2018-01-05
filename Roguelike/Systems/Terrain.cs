namespace Roguelike.Systems
{
    public struct Terrain
    {
        public bool IsWalkable { get; set; }
        public int MoveCost { get; }

        public Terrain(bool walkable, int moveCost)
        {
            IsWalkable = walkable;
            MoveCost = moveCost;
        }
    }
}