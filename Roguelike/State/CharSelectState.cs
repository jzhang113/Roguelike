using BearLib;
using Roguelike.Commands;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Roguelike.State
{
    internal sealed class CharSelectState : IState
    {
        private static readonly Lazy<CharSelectState> _instance =
            new Lazy<CharSelectState>(() => new CharSelectState());
        public static CharSelectState Instance => _instance.Value;

        private List<ColorInfo> _pixels;

        private CharSelectState()
        {
            _pixels = ImageConvert.Convert("circles.png", Game.Config.ScreenWidth).ToList();
        }

        public ICommand HandleKeyInput(int key)
        {
            return null;
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return null;
        }

        public void Update()
        {
            Game.StateHandler.HandleInput();
        }

        public void Draw()
        {
            foreach (ColorInfo info in _pixels)
            {
                int character;
                if (info.Brightness > 0.875)
                    character = 0; // empty block;
                else if (info.Brightness > 0.625)
                    character = 176; // 1/4 shaded block;
                else if (info.Brightness > 0.375)
                    character = 177; // 1/2 shaded block
                else if (info.Brightness > 0.125)
                    character = 178; // 3/4 shaded block
                else
                    character = 219; // full block 

                Terminal.Color(Color.FromArgb(info.R, info.G, info.B));
                Terminal.BkColor(Color.Wheat);
                Terminal.Put(info.X, info.Y, character);
            }
        }
    }
}
