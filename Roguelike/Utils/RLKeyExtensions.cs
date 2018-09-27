using BearLib;

namespace Roguelike.Utils
{
    internal static class RLKeyExtensions
    {
        public static char ToChar(this int key)
        {
            switch (key)
            {
                case Terminal.TK_SPACE:
                    return ' ';
                case Terminal.TK_0:
                case Terminal.TK_KP_0:
                    return '0';
                case Terminal.TK_1:
                case Terminal.TK_KP_1:
                    return '1';
                case Terminal.TK_2:
                case Terminal.TK_KP_2:
                    return '2';
                case Terminal.TK_3:
                case Terminal.TK_KP_3:
                    return '3';
                case Terminal.TK_4:
                case Terminal.TK_KP_4:
                    return '4';
                case Terminal.TK_5:
                case Terminal.TK_KP_5:
                    return '5';
                case Terminal.TK_6:
                case Terminal.TK_KP_6:
                    return '6';
                case Terminal.TK_7:
                case Terminal.TK_KP_7:
                    return '7';
                case Terminal.TK_8:
                case Terminal.TK_KP_8:
                    return '8';
                case Terminal.TK_9:
                case Terminal.TK_KP_9:
                    return '9';
                case Terminal.TK_SLASH:
                case Terminal.TK_KP_DIVIDE:
                    return '/';
                case Terminal.TK_KP_MULTIPLY:
                    return '*';
                case Terminal.TK_MINUS:
                case Terminal.TK_KP_MINUS:
                    return '-';
                case Terminal.TK_KP_PLUS:
                    return '+';
                case Terminal.TK_PERIOD:
                case Terminal.TK_KP_PERIOD:
                    return '.';
                case Terminal.TK_GRAVE:
                    return '`';
                case Terminal.TK_LBRACKET:
                    return '[';
                case Terminal.TK_RBRACKET:
                    return ']';
                case Terminal.TK_SEMICOLON:
                    return ';';
                case Terminal.TK_APOSTROPHE:
                    return '\'';
                case Terminal.TK_COMMA:
                    return ',';
                case Terminal.TK_BACKSLASH:
                    return '\\';
                default:
                    return (char)(key + 'a' - 4);
            }
        }
    }
}
