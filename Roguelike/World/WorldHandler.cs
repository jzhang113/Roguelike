using Roguelike.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Roguelike.World
{
    [Serializable]
    class WorldHandler
    {
        private Random Random { get; }
        private Random MapRandom { get; }
        public Random CombatRandom { get; }

        public MapHandler Map { get; private set; }
        public LevelId CurrentLevel { get; private set; }

        private readonly Dictionary<LevelId, LevelData> _levels;

        public WorldHandler(WorldParameter parameters) : this(parameters, (int)DateTime.Now.Ticks)
        {
        }

        public WorldHandler(WorldParameter parameters, int seed)
        {
            Random = new Random(seed);
            MapRandom = new Random(Random.Next());
            CombatRandom = new Random(Random.Next());

            _levels = BuildWorld(parameters);
            CurrentLevel = parameters.StartLevel;
            Map = CreateLevel(CurrentLevel);
        }

        public bool IsValidLevel(LevelId id)
        {
            return _levels.ContainsKey(id);
        }

        public void ChangeLevel(LevelId id)
        {
            if (!IsValidLevel(id))
            {
                Game.MessageHandler.AddMessage($"Can't go to {id}.");
                return;
            }

            if (id == CurrentLevel)
            {
                Game.MessageHandler.AddMessage($"You're already at {id}!");
                return;
            }

            SaveLevel(CurrentLevel);

            bool seen = _levels[id].Seen;
            Map = seen ? LoadLevel(id) : CreateLevel(id);
            CurrentLevel = id;
        }

        private Dictionary<LevelId, LevelData> BuildWorld(WorldParameter parameters)
        {
            Dictionary<LevelId, LevelData> world = new Dictionary<LevelId, LevelData>();

            // TODO: randomly select regions from list
            foreach (WorldParameter.Region region in parameters.Regions)
            {
                int maxDepth = Random.Next(region.MinLength, region.MaxLength);

                for (int depth = 1; depth <= maxDepth; depth++)
                {
                    LevelId id = new LevelId
                    {
                        RegionName = region.Name,
                        Depth = depth
                    };
                    LevelData level = new LevelData
                    {
                        Seen = false,
                        Exits = new List<LevelId>
                        {
                            new LevelId
                            {
                                RegionName = region.Name,
                                Depth = depth + 1
                            }
                        }
                    };
                    world.Add(id, level);
                }
            }

            // TODO: apply connection constraints and ensure connectedness

            return world;
        }

        private MapHandler CreateLevel(LevelId id)
        {
            System.Diagnostics.Debug.Assert(IsValidLevel(id), $"unknown level: {id}");
            if (!IsValidLevel(id))
                return null;

            var sw = new System.Diagnostics.Stopwatch();
            LevelData level = _levels[id];

            sw.Start();
            MapGenerator mapGenerator = new MapGenerator(Game.Config.Map.Width, Game.Config.Map.Height, level.Exits, MapRandom);
            MapHandler map = mapGenerator.CreateMapBsp();
            sw.Stop();

            Console.WriteLine($"Map generated in: {sw.Elapsed}");

            level.Seen = true;

            return map;
        }

        #region IO Operations
        private void SaveLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenWrite($"{id.RegionName}_{id.Depth}.dat"))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, Map);
            }
        }

        private static MapHandler LoadLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenRead($"{id.RegionName}_{id.Depth}.dat"))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                return (MapHandler)deserializer.Deserialize(saveFile);
            }
        }
        #endregion
    }
}