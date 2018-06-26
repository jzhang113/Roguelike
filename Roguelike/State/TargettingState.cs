using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;

namespace Roguelike.State
{
    class TargettingState : IState
    {
        private ITargetCommand _targetCommand;
        private Actor _targetSource;
        private IAction _targetAction;

        public TargettingState(ITargetCommand targetCommand, Actor source, IAction targetAction)
        {
            _targetCommand = targetCommand;
            _targetSource = source;
            _targetAction = targetAction;
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            throw new NotImplementedException();
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
