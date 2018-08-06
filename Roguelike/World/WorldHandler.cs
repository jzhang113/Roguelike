using Pcg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Roguelike.World
{
    [Serializable]
    class WorldHandler
    {
        // TODO: generate dynamic world names
        private const string _ROOT_NAME = "root";
        private static int _counter;

        public MapHandler Map { get; private set; }
        public LevelId CurrentLevel { get; private set; }
        private readonly Dictionary<LevelId, LevelData> _levels;

        public WorldHandler(WorldParameter parameters)
        {
            _levels = BuildWorld(parameters);
            CurrentLevel = new LevelId
            {
                Name = _ROOT_NAME,
                RegionType = RegionType.Root,
                Depth = 1
            };
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
            // TODO: save and restore monsters on level

            bool seen = _levels[id].Seen;
            Map = seen ? LoadLevel(id) : CreateLevel(id);
            CurrentLevel = id;

            Map.Refresh();
        }

        private Dictionary<LevelId, LevelData> BuildWorld(WorldParameter parameters)
        {
            int maxRegions = Game.Random.Next(parameters.MinWorldSize, parameters.MaxWorldSize);
            int regionCount = 0;

            // initialize the map with the overworld
            Dictionary<LevelId, LevelData> world = new Dictionary<LevelId, LevelData>
            {
                {
                    new LevelId
                    {
                        Name = _ROOT_NAME,
                        RegionType = RegionType.Root,
                        Depth = 1
                    },
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
            string levelName = GenerateLevelName(region.Type);

            // attach new region to existing world
            LevelId parentId;
            LevelData parentLevel;

            if (region.Constraints.Require != null)
            {
                parentId = region.Constraints.Require.First();
                parentLevel = FindLevelByRegion(parentId.RegionType, world);
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
                var keyValuePair = world.ElementAt(Game.Random.Next(world.Count()));
                parentId = keyValuePair.Key;
                parentLevel = keyValuePair.Value;
            }

            LevelId firstId = new LevelId
            {
                Name = levelName,
                RegionType = region.Type,
                Depth = 1
            };
            parentLevel.Exits.Add(firstId);

            // attach the opposite connection as well
            LevelData firstLevel = new LevelData
            {
                Seen = false,
                Exits = new List<LevelId>
                {
                    parentId,
                    new LevelId
                    {
                        Name = levelName,
                        RegionType = region.Type,
                        Depth = 2
                    }
                },
                Seed = Game.Random.Next()
            };
            world.Add(firstId, firstLevel);

            // create rest of connections in the region
            for (int depth = 2; depth <= maxDepth - 1; depth++)
            {
                LevelId id = new LevelId
                {
                    Name = levelName,
                    RegionType = region.Type,
                    Depth = depth
                };
                LevelData level = new LevelData
                {
                    Seen = false,
                    Exits = new List<LevelId>
                    {
                        new LevelId
                        {
                            Name = levelName,
                            RegionType = region.Type,
                            Depth = depth - 1
                        },
                        new LevelId
                        {
                            Name = levelName,
                            RegionType = region.Type,
                            Depth = depth + 1
                        }
                    },
                    Seed = Game.Random.Next()
                };
                world.Add(id, level);
            }

            LevelId lastId = new LevelId
            {
                Name = levelName,
                RegionType = region.Type,
                Depth = maxDepth
            };
            LevelData lastLevel = new LevelData
            {
                Seen = false,
                Exits = new List<LevelId>
                {
                    new LevelId
                    {
                        Name = levelName,
                        RegionType = region.Type,
                        Depth = maxDepth - 1
                    }
                },
                Seed = Game.Random.Next()
            };
            world.Add(lastId, lastLevel);
        }

        private static string GenerateLevelName(RegionType region)
        {
            switch (region)
            {
                // TODO: branch name generation
                case RegionType.Root:
                    return _ROOT_NAME;
                case RegionType.Main:
                    return "herp" + _counter++;
                case RegionType.Side:
                    return "perp" + _counter++;
                case RegionType.Otherside:
                    return "derp" + _counter++;
                default:
                    throw new ArgumentException($"undefined value of region");
            }
        }

        private LevelData FindLevelByRegion(RegionType regionType,
            Dictionary<LevelId, LevelData> world)
        {
            foreach (var kvp in world)
            {
                if (kvp.Key.RegionType == regionType)
                    return kvp.Value;
            }

            // TODO: handle parent region not added yet
            return null;
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

            return map;
        }

        #region IO Operations
        private void SaveLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenWrite($"{id}.dat"))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, Map);
            }
        }

        private static MapHandler LoadLevel(LevelId id)
        {
            using (Stream saveFile = File.OpenRead($"{id}.dat"))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                return (MapHandler)deserializer.Deserialize(saveFile);
            }
        }
        #endregion
    }
}