using BearLib;
using System;
using System.Drawing;

namespace Roguelike.Core
{
    internal class MenuButton
    {
        public Rectangle Position { get; }
        public string Text { get; }
        public Action Callback { get; }

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
            Position = new Rectangle(x, y, text.Length + 4, 5);
            Text = text;
            Callback = callback;

            BackgroundColor = Colors.ButtonBackground;
            BorderColor = Colors.ButtonBorder;

            HoverColor = Colors.ButtonHover;
            TextColor = Colors.Text;

            _border = border;
            _align = alignment;
        }

        public void Draw()
        {
            Terminal.Composition(true);
            Terminal.Color(Hover ? HoverColor : BackgroundColor);

            if (_border)
            {
                for (int x = Position.Left + 1; x < Position.Right - 1; x++)
                {
                    for (int y = Position.Top + 1; y < Position.Bottom - 1; y++)
                    {
                        Terminal.Put(x, y, '█'); // 219
                    }
                }

                Terminal.Color(BorderColor);
                // top and bottom border
                for (int x = Position.Left + 1; x < Position.Right - 1; x++)
                {
                    Terminal.Put(x, Position.Top, '─'); // 196
                    Terminal.Put(x, Position.Bottom - 1, '─');
                }

                // left and right border
                for (int y = Position.Top + 1; y < Position.Bottom - 1; y++)
                {
                    Terminal.Put(Position.Left, y, '│'); // 179
                    Terminal.Put(Position.Right - 1, y, '│');
                }

                // corners
                Terminal.Put(Position.Left, Position.Top, '┌'); // 218
                Terminal.Put(Position.Right - 1, Position.Top, '┐'); // 191
                Terminal.Put(Position.Left, Position.Bottom - 1, '└'); // 192
                Terminal.Put(Position.Right - 1, Position.Bottom - 1, '┘'); // 217
            }
            else
            {
                for (int x = Position.Left; x < Position.Right; x++)
                {
                    for (int y = Position.Top; y < Position.Bottom; y++)
                    {
                        Terminal.Put(x, y, '█');
                    }
                }
            }

            Terminal.Color(TextColor);
            Terminal.Print(Position, _align, Text);
            Terminal.Composition(false);
        }
    }
}