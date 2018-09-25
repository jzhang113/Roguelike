using BearLib;
using System;
using System.Drawing;

namespace Roguelike.Core
{
    internal class MenuButton
    {
        public int X { get; }
        public int Y { get; }
        public string Text { get; }
        public Action Callback { get; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }
        public Color HoverColor { get; set; }
        public Color TextColor { get; set; }

        internal bool Hover { get; set; }

        private readonly bool _border;
        private readonly ContentAlignment _align;

        public MenuButton(int x, int y, string text, Action callback,
            bool border = false, ContentAlignment alignment = ContentAlignment.MiddleCenter)
        {
            X = x;
            Y = y;
            Text = text;
            Callback = callback;

            Width = text.Length + 4;
            Height = 5;

            BackgroundColor = Colors.ButtonBackground;
            BorderColor = Colors.ButtonBorder;

            HoverColor = Colors.ButtonHover;
            TextColor = Colors.Text;

            _border = border;
            _align = alignment;
        }

        public void Draw()
        {
            Color backColor = Hover ? HoverColor : BackgroundColor;

            if (_border)
            {
                Terminal.BkColor(backColor);
                for (int dx = 1; dx < Width - 1; dx++)
                {
                    for (int dy = 1; dy < Height - 1; dy++)
                    {
                        Terminal.Put(X + dx, Y + dy, Terminal.Pick(X + dx, Y + dy));
                    }
                }

                Terminal.Color(BorderColor);
                // top and bottom border
                for (int dx = 1; dx < Width - 1; dx++)
                {
                    Terminal.Put(X + dx, Y, 196);
                    Terminal.Put(X + dx, Y + Height - 1, 196);
                }

                // left and right border
                for (int dy = 1; dy < Height - 1; dy++)
                {
                    Terminal.Put(X, Y + dy, 179);
                    Terminal.Put(X + Width - 1, Y + dy, 179);
                }

                // corners
                Terminal.Put(X, Y, 218);
                Terminal.Put(X + Width - 1, Y, 191);
                Terminal.Put(X, Y + Height - 1, 192);
                Terminal.Put(X + Width - 1, Y + Height - 1, 217);
            }
            else
            {
                Terminal.BkColor(backColor);
                for (int dx = 0; dx < Width; dx++)
                {
                    for (int dy = 0; dy < Height; dy++)
                    {
                        Terminal.Put(X + dx, Y + dy, Terminal.Pick(X + dx, Y + dy));
                    }
                }
            }

            Terminal.Color(TextColor);
            Terminal.Print(X + 1, Y + Height / 2, _align, Text);
        }
    }
}