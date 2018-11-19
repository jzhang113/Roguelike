using BearLib;
using Roguelike.Animations;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.UI;
using System;

namespace Roguelike.State
{
    internal class QteState : IState
    {
        private const int _WIDTH = 32;
        private const int _PADDING = 2;

        public bool Nonblocking => true;

        private readonly QtePanel _panel;
        private float _current;
        private readonly int _left;
        private readonly int _right;

        public QteState()
        {
            int mid = Game.Random.Next(_WIDTH - 4 * _PADDING) + _PADDING;
            // TODO: customizable regions, multiple regions, different bar widths
            _left = mid - 2;
            _right = mid + 2;
            _panel = new QtePanel(_WIDTH, 1, _left, _right);
            _current = 0;
        }

        public ICommand HandleKeyInput(int key)
        {
            // TODO: proper command returned should by fixed by reaction system
            if (key != Terminal.TK_INPUT_NONE)
            {
                return new WaitCommand(Game.Player);
            }
            else
            {
                return null;
            }
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // =TODO= what does mouse do?
            if (leftClick || rightClick)
                throw new NotImplementedException();

            return null;
        }

        public void Update(ICommand command)
        {
            // TODO: need a new system to deal with out of turn actions
            if (command == null && _current < _WIDTH)
            {
                // TODO: customize speed
                _current += 0.3f;
            }
            else
            {
                Game.StateHandler.PopState();

                var color = _current > _left && _current < _right ? Swatch.DbGrass : Swatch.DbBlood;
                Game.StateHandler.PushState(new AnimationState(new QteFlash(_panel, color)));
            }
        }

        public void Draw(LayerInfo layer)
        {
            _panel.Draw(layer, _current);
        }
    }
}
