using Roguelike.Actions;
using System;

namespace Roguelike.Systems
{
    [Serializable]
    internal class MovesetHandler
    {
        private readonly ActionNode _starter;
        private ActionNode _current;

        public MovesetHandler(ActionNode starter)
        {
            _starter = starter;
            _current = starter;
        }

        public IAction ChooseLeft()
        {
            IAction action = _current.Action;
            _current = _current?.Left ?? _starter;

            return action;
        }

        public IAction ChooseRight()
        {
            IAction action = _current.Action;
            _current = _current?.Right ?? _starter;

            return action;
        }

        public void Reset()
        {
            _current = _starter;
        }
    }

    [Serializable]
    internal class ActionNode
    {
        public ActionNode Left { get; }
        public ActionNode Right { get; }
        public IAction Action { get; set; }

        public ActionNode(ActionNode left, ActionNode right, IAction action)
        {
            Left = left;
            Right = right;
            Action = action;
        }
    }
}
