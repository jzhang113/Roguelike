using Newtonsoft.Json;
using Roguelike.Core;
using Roguelike.Systems;
using System.IO;

namespace Roguelike
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration configs = LoadData<Configuration>("config", Directory.GetCurrentDirectory());
            Game.Initialize(configs, OptionHandler.ParseOptions(args));
            Game.Run();
        }

        internal static T LoadData<T>(string filename, string path = "Data")
        {
            using (StreamReader sr = new StreamReader($"{path}/{filename}.json"))
            {
                string json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
