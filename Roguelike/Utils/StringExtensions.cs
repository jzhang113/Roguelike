using System.Linq;

namespace Roguelike.Utils
{
    static class StringExtensions
    {
        public static string Pluralize(this string name)
        {
            return name.Last() == 's' ? $"{name}es" : $"{name}s";
        }
    }
}
