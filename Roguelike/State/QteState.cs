using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.UI;
using System;

namespace Roguelike.State
{
    internal class QteState : IState
    {
        private const int _WIDTH = Constants.MAPVIEW_WIDTH - 2;
        private const int _PADDING = 2;

        public bool Nonblocking => true;

        private readonly QtePanel _panel;
        private float _current;

        public QteState()
        {
            int mid = Game.Random.Next(_WIDTH - 4 * _PADDING) + _PADDING;
            _panel = new QtePanel(_WIDTH, mid - 2, mid + 2);
            _current = 0;
        }

        public ICommand HandleKeyInput(int key)
        {
            // TODO: proper command returned should by fixed by reaction system
            return key != Terminal.TK_INPUT_NONE ?
                new WaitCommand(Game.Player) : null;
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // =TODO= what does mouse do?
            throw new NotImplementedException();
        }

        public void Update(ICommand command)
        {
            // TODO: need a new system to deal with out of turn actions
            if (command == null && _current < _WIDTH)
                _current += 0.8f;
            else
                Game.StateHandler.PopState();
        }

        public void Draw(LayerInfo layer)
        {
            _panel.Draw(layer, _current);
        }
    }
}
