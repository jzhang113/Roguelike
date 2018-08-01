using RLNET;
using System;

namespace Roguelike.Core
{
    class MenuButton
    {
        public int X { get; }
        public int Y { get; }
        public string Text { get; }
        public Action Callback { get; }

        public int Width { get; set; }
        public int Height { get; set; }

        public RLColor BackgroundColor { get; set; }
        public RLColor BorderColor { get; set; }
        public RLColor HoverColor { get; set; }
        public RLColor TextColor { get; set; }

        internal bool Hover { get; set; }

        private bool _border;
        private Alignment _align;

        public MenuButton(int x, int y, string text, Action callback,
            bool border = false, Alignment alignment = Alignment.Center)
        {
            X = x;
            Y = y;
            Text = text;
            Callback = callback;

            Width = text.Length + 4;
            Height = 5;

            BackgroundColor = Swatch.AlternateDarker;
            BorderColor = Colors.TextHeading;
            HoverColor = Swatch.AlternateDarkest;
            TextColor = Colors.TextHeading;

            _border = border;
            _align = alignment;
        }

        public void Draw(RLConsole console)
        {
            RLColor backColor = Hover ? HoverColor : BackgroundColor;

            if (_border)
            {
                for (int dx = 1; dx < Width - 1; dx++)
                {
                    for (int dy = 1; dy < Height - 1; dy++)
                    {
                        console.SetBackColor(X + dx, Y + dy, backColor);
                    }
                }

                // top and bottom border
                for (int dx = 1; dx < Width - 1; dx++)
                {
                    console.Set(X + dx, Y, BorderColor, backColor, 196);
                    console.Set(X + dx, Y + Height - 1, BorderColor, backColor, 196);
                }

                // left and right border
                for (int dy = 1; dy < Height - 1; dy++)
                {
                    console.Set(X, Y + dy, BorderColor, backColor, 179);
                    console.Set(X + Width - 1, Y + dy, BorderColor, backColor, 179);
                }

                // corners
                console.Set(X, Y, BorderColor, backColor, 218);
                console.Set(X + Width - 1, Y, BorderColor, backColor, 191);
                console.Set(X, Y + Height - 1, BorderColor, backColor, 192);
                console.Set(X + Width - 1, Y + Height - 1, BorderColor, backColor , 217);
            }
            else
            {
                for (int dx = 0; dx < Width - 0; dx++)
                {
                    for (int dy = 0; dy < Height - 0; dy++)
                    {
                        console.SetBackColor(X + dx, Y + dy, backColor);
                    }
                }
            }

            switch (_align)
            {
                case Alignment.Center:
                    console.Print(X + (Width - Text.Length) / 2, Y + Height / 2, Text, TextColor);
                    break;
                case Alignment.Left:
                    console.Print(X + 1, Y + Height / 2, Text, TextColor);
                    break;
                case Alignment.Right:
                    console.Print(X + Width - Text.Length - 1, Y + Height / 2, Text, TextColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_align));
            }
        }
    }
}