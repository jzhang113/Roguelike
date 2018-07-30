using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.State
{
    class CharSelectState : IState
    {
        private static readonly Lazy<CharSelectState> _instance =
            new Lazy<CharSelectState>(() => new CharSelectState());
        public static CharSelectState Instance => _instance.Value;

        private List<ColorInfo> _pixels;

        private CharSelectState()
        {
            _pixels = ImageConvert.Convert("circles.png", Game.Config.Screen.Width).ToList();
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            return null;
        }

        public void Update()
        {
            Game.StateHandler.HandleInput();
            Game.ForceRender();
        }

        public void Draw(RLConsole console)
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

                console.Set(info.X, info.Y, new RLColor(info.R, info.G, info.B), RLColor.White, character);
            }
        }
    }
}
