using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.UI;
using System;
using System.Drawing;
using Roguelike.Utils;

namespace Roguelike.State
{
    internal class QteState : IState
    {
        private const int _WIDTH = 32;
        private const int _PADDING = 2;

        public bool Nonblocking => true;

        private readonly QtePanel _panel;
        private float _current;
        private int _left;
        private int _right;

        private int _ticks;
        private bool _done;

        public QteState()
        {
            int mid = Game.Random.Next(_WIDTH - 4 * _PADDING) + _PADDING;
            // TODO: customizable regions, multiple regions, different bar widths
            _left = mid - 2;
            _right = mid + 2;
            _panel = new QtePanel(_WIDTH, 1, _left, _right);
            _current = 0;

            _ticks = 20;
            _done = false;
        }

        public ICommand HandleKeyInput(int key)
        {
            // TODO: proper command returned should by fixed by reaction system
            if (key != Terminal.TK_INPUT_NONE)
            {
                _done = true;
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
            if (command == null && _current < _WIDTH && !_done)
            {
                // TODO: customize speed
                _current += 0.3f;
            }
            else
            {
                if (_ticks <= 0)
                    Game.StateHandler.PopState();
                else
                    _ticks--;
            }
        }

        public void Draw(LayerInfo layer)
        {
            if (!_done)
                _panel.Draw(layer, _current);
            else
            {
                if (_current > _left && _current < _right)
                {
                    _panel.Draw(layer, Colors.Grass.Blend(Color.Black, _ticks / (double)20));
                }
                else
                {
                    _panel.Draw(layer, Swatch.DbBlood.Blend(Color.Black, _ticks / (double)20));
                }
            }
        }
    }
}
