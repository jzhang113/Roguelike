using RLNET;

namespace Roguelike.Utils
{
    // ReSharper disable once InconsistentNaming
    static class RLKeyExtensions
    {
        // ReSharper disable once CyclomaticComplexity
        public static char ToChar(this RLKey key)
        {
            int keyValue = (int)key;
            int charValue;

            switch (keyValue)
            {
                case 51: charValue = ' '; break;
                case 67:
                case 109: charValue = '0'; break;
                case 68:
                case 110: charValue = '1'; break;
                case 69:
                case 111: charValue = '2'; break;
                case 70:
                case 112: charValue = '3'; break;
                case 71:
                case 113: charValue = '4'; break;
                case 72:
                case 114: charValue = '5'; break;
                case 73:
                case 115: charValue = '6'; break;
                case 74:
                case 116: charValue = '7'; break;
                case 75:
                case 117: charValue = '8'; break;
                case 76:
                case 118: charValue = '9'; break;
                case 77:
                case 128: charValue = '/'; break;
                case 78: charValue = '*'; break;
                case 79:
                case 120: charValue = '-'; break;
                case 80:
                case 121: charValue = '+'; break;
                case 81:
                case 127: charValue = '.'; break;
                case 119: charValue = '~'; break;
                case 122: charValue = '['; break;
                case 123: charValue = ']'; break;
                case 124: charValue = ';'; break;
                case 125: charValue = '\''; break;
                case 126: charValue = ','; break;
                case 129: charValue = '\\'; break;
                default: charValue = keyValue + 14; break;
            }

            return (char)charValue;
        }
    }
}
