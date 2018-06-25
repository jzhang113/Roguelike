using System.Linq;

namespace Roguelike
{
    static class StringExtensions
    {
        public static string Pluralize(this string name)
        {
            return name.Last() == 's' ? $"{name}es" : $"{name}s";
        }
    }
}
