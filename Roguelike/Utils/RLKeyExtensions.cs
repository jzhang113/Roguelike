using BearLib;

namespace Roguelike.Utils
{
    // ReSharper disable once InconsistentNaming
    internal static class RLKeyExtensions
    {
        // ReSharper disable once CyclomaticComplexity
        public static char ToChar(this int key)
        {
            char charValue;
            switch (key)
            {
                case Terminal.TK_SPACE: charValue = ' '; break;
                case Terminal.TK_0:
                case Terminal.TK_KP_0: charValue = '0'; break;
                case Terminal.TK_1:
                case Terminal.TK_KP_1: charValue = '1'; break;
                case Terminal.TK_2:
                case Terminal.TK_KP_2: charValue = '2'; break;
                case Terminal.TK_3:
                case Terminal.TK_KP_3: charValue = '3'; break;
                case Terminal.TK_4:
                case Terminal.TK_KP_4: charValue = '4'; break;
                case Terminal.TK_5:
                case Terminal.TK_KP_5: charValue = '5'; break;
                case Terminal.TK_6:
                case Terminal.TK_KP_6: charValue = '6'; break;
                case Terminal.TK_7:
                case Terminal.TK_KP_7: charValue = '7'; break;
                case Terminal.TK_8:
                case Terminal.TK_KP_8: charValue = '8'; break;
                case Terminal.TK_9:
                case Terminal.TK_KP_9: charValue = '9'; break;
                case Terminal.TK_SLASH:
                case Terminal.TK_KP_DIVIDE: charValue = '/'; break;
                case Terminal.TK_KP_MULTIPLY:charValue = '*'; break;
                case Terminal.TK_MINUS:
                case Terminal.TK_KP_MINUS: charValue = '-'; break;
                case Terminal.TK_KP_PLUS: charValue = '+'; break;
                case Terminal.TK_PERIOD:
                case Terminal.TK_KP_PERIOD: charValue = '.'; break;
                case Terminal.TK_GRAVE: charValue = '`'; break;
                case Terminal.TK_LBRACKET: charValue = '['; break;
                case Terminal.TK_RBRACKET: charValue = ']'; break;
                case Terminal.TK_SEMICOLON: charValue = ';'; break;
                case Terminal.TK_APOSTROPHE: charValue = '\''; break;
                case Terminal.TK_COMMA: charValue = ','; break;
                case Terminal.TK_BACKSLASH: charValue = '\\'; break;
                default: charValue = (char)(key + 14); break;
            }

            return charValue;
        }
    }
}
