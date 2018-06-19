using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Roguelike.Systems
{
    [Serializable]
    class WorldHandler
    {
        public Random Random { get; }
        public Random MapRandom { get; }
        public Random CombatRandom { get; }

        public MapHandler Map { get; private set; }

        private Dictionary<string, bool> _levels;
        private string _currentLevel;

        public WorldHandler() : this((int)DateTime.Now.Ticks)
        {
        }

        public WorldHandler(int seed)
        {
            Random = new Random(seed);
            MapRandom = new Random(Random.Next());
            CombatRandom = new Random(Random.Next());

            _levels = new Dictionary<string, bool>
            {
                { "main_1", false },
                { "main_2", false },
                { "main_3", false }
            };
        }

        public MapHandler CreateLevel(string levelName)
        {
            System.Diagnostics.Debug.Assert(IsValidLevel(levelName), $"unknown level: {levelName}");
            if (!IsValidLevel(levelName))
                return null;

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            MapGenerator mapGenerator = new MapGenerator(Game.Config.Map.Width, Game.Config.Map.Height, MapRandom);
            MapHandler map = mapGenerator.CreateMapBsp();
            sw.Stop();

            Console.WriteLine($"Map generated in: {sw.Elapsed}");

            _levels[levelName] = true;

            return map;
        }

        public bool IsValidLevel(string levelName)
        {
            return _levels.ContainsKey(levelName);
        }

        public void ChangeLevel(string levelName)
        {
            if (!IsValidLevel(levelName))
            {
                Game.MessageHandler.AddMessage($"Can't go to {levelName}.");
                return;
            }

            if (levelName == _currentLevel)
            {
                Game.MessageHandler.AddMessage($"You're already at {levelName}!");
                return;
            }

            if (_currentLevel != null)
                SaveLevel(_currentLevel);

            bool seen = _levels[levelName];
            Map = seen ? LoadLevel(levelName) : CreateLevel(levelName);
            _currentLevel = levelName;
        }

        #region IO Operations
        private void SaveLevel(string currentLevel)
        {
            using (Stream saveFile = File.OpenWrite($"{currentLevel}.dat"))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, Map);
            }
        }

        private MapHandler LoadLevel(string levelName)
        {
            using (Stream saveFile = File.OpenRead($"{_currentLevel}.dat"))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                return (MapHandler)deserializer.Deserialize(saveFile);
            }
        }
        #endregion
    }
}