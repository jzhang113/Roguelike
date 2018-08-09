using Pcg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Roguelike.Actors;

namespace Roguelike.World
{
    [MessagePackObject]
    class WorldHandler
    {
        // TODO: generate dynamic world names
        private const string _ROOT_NAME = "root";

        [Key(0)]
        public MapHandler Map { get; private set; }
        [Key(1)]
        public LevelId CurrentLevel { get; private set; }
        [Key(2)]
        public Player Player { get; private set; }

        [Key(3)]
        private readonly Dictionary<LevelId, LevelData> _levels;

        // Deserialization constructor
        public WorldHandler()
        {
        }

        public WorldHandler(WorldParameter parameters)
        {
            Player = new Player(new ActorParameters("Player")
            {
                Awareness = 10,
                MaxHp = 100,
                MaxMp = 50,
                MaxSp = 50
            });
            _levels = BuildWorld(parameters);
            CurrentLevel = new LevelId(_ROOT_NAME, RegionType.Root, 1);
        }

        public void Initialize()
        {
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
            Game.EventScheduler.Clear();
            // TODO: save and restore monsters on level if monsters also change levels

            bool seen = _levels[id].Seen;
            Map = seen ? LoadLevel(id) : CreateLevel(id);

            if (Map.TryGetExit(CurrentLevel, out Core.Exit exit))
                Map.SetActorPosition(Game.Player, exit.X, exit.Y);

            Map.Refresh();
            CurrentLevel = id;
        }

        private Dictionary<LevelId, LevelData> BuildWorld(WorldParameter parameters)
        {
            int maxRegions = Game.Random.Next(parameters.MinWorldSize, parameters.MaxWorldSize);
            int regionCount = 0;

            // initialize the map with the overworld
            Dictionary<LevelId, LevelData> world = new Dictionary<LevelId, LevelData>
            {
                {
                    new LevelId(_ROOT_NAME, RegionType.Root, 1),
                    new LevelData
                    {
                        Seen = false,
                        Exits = new List<LevelId>(),
                        Seed = Game.Random.Next()
                    }
                }
            };

            foreach (WorldParameter.RegionData region in
                parameters.Regions.Where(r => r.Require).ToList())
            {
                BuildLevelConnections(region, world);
                if (region.Unique)
                    parameters.Regions.Remove(region);

                regionCount++;
            }

            for (int i = regionCount; i < maxRegions; i++)
            {
                // TODO: weighted region probabilities
                WorldParameter.RegionData region
                    = parameters.Regions.ElementAt(Game.Random.Next(parameters.Regions.Count));
                BuildLevelConnections(region, world);
                if (region.Unique)
                    parameters.Regions.Remove(region);
            }

            return world;
        }

        // TODO: potentially connect the end of branches to other areas?
        private void BuildLevelConnections(WorldParameter.RegionData region,
            Dictionary<LevelId, LevelData> world)
        {
            int maxDepth = Game.Random.Next(region.MinLength, region.MaxLength);

            // attach new region to existing world
            LevelId parentId;
            LevelData parentLevel;

            if (region.Constraints.Require != null)
            {
                WorldParameter.RequireId partialId = region.Constraints.Require
                    .ElementAt(Game.Random.Next(region.Constraints.Require.Count));
                var keyValuePair = FindLevelByRegion(partialId, world);
                parentId = keyValuePair.Key;
                parentLevel = keyValuePair.Value;
            }

            else if (region.Constraints.Avoid != null)
            {
                var remaining = world
                    .Where(kvp => !region.Constraints.Avoid.Contains(kvp.Key.RegionType))
                    .ToList();
                var keyValuePair = remaining.ElementAt(Game.Random.Next(remaining.Count));
                parentId = keyValuePair.Key;
                parentLevel = keyValuePair.Value;
            }
            else
            {
                var keyValuePair = world.ElementAt(Game.Random.Next(world.Count));
                parentId = keyValuePair.Key;
                parentLevel = keyValuePair.Value;
            }

            int seed = Game.Random.Next();
            string levelName = GenerateLevelName(region.Type,seed);
            LevelId firstId = new LevelId(levelName, region.Type, 1);
            parentLevel.Exits.Add(firstId);

            // attach the opposite connection as well
            LevelData firstLevel = new LevelData
            {
                Seen = false,
                Exits = new List<LevelId>
                {
                    parentId,
                    new LevelId(levelName, region.Type, 2)
                },
                Seed = seed
            };
            world.Add(firstId, firstLevel);

            // create rest of connections in the region
            for (int depth = 2; depth <= maxDepth - 1; depth++)
            {
                seed = Game.Random.Next();
                levelName = GenerateLevelName(region.Type, seed);
                LevelId id = new LevelId(levelName, region.Type, depth);
                LevelData level = new LevelData
                {
                    Seen = false,
                    Exits = new List<LevelId>
                    {
                        new LevelId(levelName, region.Type, depth - 1),
                        new LevelId(levelName, region.Type, depth + 1)
                    },
                    Seed = seed
                };
                world.Add(id, level);
            }

            seed = Game.Random.Next();
            levelName = GenerateLevelName(region.Type, seed);
            LevelId lastId = new LevelId(levelName, region.Type, maxDepth);
            LevelData lastLevel = new LevelData
            {
                Seen = false,
                Exits = new List<LevelId>
                {
                    new LevelId(levelName, region.Type, maxDepth - 1)
                },
                Seed = seed
            };
            world.Add(lastId, lastLevel);
        }

        private static string GenerateLevelName(RegionType region, int seed)
        {
            switch (region)
            {
                // TODO: branch name generation
                case RegionType.Root:
                    return _ROOT_NAME;
                case RegionType.Main:
                    return "herp" + seed;
                case RegionType.Side:
                    return "perp" + seed;
                case RegionType.Otherside:
                    return "derp" + seed;
                default:
                    throw new ArgumentException($"undefined value of region");
            }
        }

        private KeyValuePair<LevelId, LevelData> FindLevelByRegion(
            WorldParameter.RequireId partialId,
            Dictionary<LevelId, LevelData> world)
        {
            foreach (var kvp in world)
            {
                if (kvp.Key.RegionType == partialId.RegionType
                    && kvp.Key.Depth >= partialId.MinDepth)
                    return kvp;
            }

            // TODO: handle parent region not added yet
            return default;
        }

        private MapHandler CreateLevel(LevelId id)
        {
            System.Diagnostics.Debug.Assert(IsValidLevel(id), $"unknown level: {id}");
            if (!IsValidLevel(id))
                return null;

            var sw = new System.Diagnostics.Stopwatch();
            LevelData level = _levels[id];

            sw.Start();
            MapGenerator mapGenerator = new JaggedMapGenerator(
                Game.Config.Map.Width, Game.Config.Map.Height,
                level.Exits, new PcgRandom(level.Seed));
            MapHandler map = mapGenerator.Generate();
            sw.Stop();

            Console.WriteLine($"Map generated in: {sw.Elapsed}");

            level.Seen = true;
            map.Refresh();

            return map;
        }

        #region IO Operations
        private void SaveLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenWrite($"{id}.dat"))
            {
                MessagePackSerializer.Typeless.Serialize(saveFile, Map);
            }
        }

        private static MapHandler LoadLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenRead($"{id}.dat"))
            {
                return MessagePackSerializer.Typeless.Deserialize(saveFile) as MapHandler;
            }
        }
        #endregion
    }
}