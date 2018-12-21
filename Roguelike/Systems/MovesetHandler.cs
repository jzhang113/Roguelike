using BearLib;
using Roguelike.Actions;
using Roguelike.Core;
using System;

namespace Roguelike.Systems
{
    [Serializable]
    internal class MovesetHandler
    {
        private ActionNode Root { get; }
        private ActionNode Current { get; set; }

        public MovesetHandler(ActionNode starter)
        {
            Root = starter;
            Current = starter;
        }

        public IAction ChooseLeft()
        {
            IAction action = Current.Action;
            Current = Current?.Left ?? Root;

            return action;
        }

        public IAction ChooseRight()
        {
            IAction action = Current.Action;
            Current = Current?.Right ?? Root;

            return action;
        }

        public void Reset()
        {
            Current = Root;
        }

        public void Draw(LayerInfo layer)
        {
            RecursivePrint(layer, Root, 0, 0);
        }

        private void RecursivePrint(LayerInfo layer, ActionNode action, int depth, int line)
        {
            if (action == null)
                return;

            string text;

            if (action == Current.Left)
                text = "z-";
            else if (action == Current.Right)
                text = "x-";
            else
                text = depth > 0 ? "+-" : "  ";

            Terminal.Color(action == Current ? Swatch.DbBlood : Colors.Text);
            layer.Print(2 * depth - 2, line, text + action.Name);

            RecursivePrint(layer, action.Left, depth + 1, line + 1);
            RecursivePrint(layer, action.Right, depth + 1, line + 1);
        }
    }

    [Serializable]
    internal class ActionNode
    {
        public ActionNode Left { get; }
        public ActionNode Right { get; }
        public IAction Action { get; set; }
        public string Name { get; set; }

        public ActionNode(ActionNode left, ActionNode right, IAction action, string name)
        {
            Left = left;
            Right = right;
            Action = action;
            Name = name;
        }
    }
}
