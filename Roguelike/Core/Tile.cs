using MessagePack;
using Roguelike.Data;
using Roguelike.Interfaces;

namespace Roguelike.Core
{
    [MessagePackObject]
    public class Tile
    {
        [Key(0)]
        public int X { get; }
        [Key(1)]
        public int Y { get; }

        [Key(2)]
        public TerrainType Type
        {
            get => _type;
            internal set
            {
                _type = value;
                DrawingComponent = value.ToDrawable();
            }
        }

        [Key(3)]
        public float Light
        {
            get => _light;
            internal set
            {
                if (value < 0)
                    _light = 0;
                else if (value > 1)
                    _light = 1;
                else
                    _light = value;
            }
        }

        [Key(4)]
        public int Fuel { get; internal set; }
        [Key(5)]
        public bool IsOccupied { get; internal set; }
        [Key(6)]
        public bool IsExplored { get; internal set; }
        [Key(7)]
        public bool BlocksLight { get; internal set; }
        [Key(8)]
        public bool LosExists { get; internal set; }

        [IgnoreMember]
        public bool IsVisible => LosExists && Light > Constants.MIN_VISIBLE_LIGHT_LEVEL;
        [IgnoreMember]
        public bool IsWall => Type == TerrainType.Wall;
        [IgnoreMember]
        public bool IsWalkable => !IsWall && !IsOccupied;
        [IgnoreMember]
        public bool IsLightable => !IsWall && !BlocksLight;

        [IgnoreMember]
        public Drawable DrawingComponent { get; private set; }

        [IgnoreMember]
        private float _light;
        [IgnoreMember]
        private TerrainType _type;

        public Tile(int x, int y, TerrainType type)
        {
            X = x;
            Y = y;
            Type = type;

            Fuel = 10;
        }
    }
}