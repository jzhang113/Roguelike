using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Actions
{
    class ActionSequence
    {
        public int Speed { get; }
        public IEnumerable<IAction> Actions { get; }

        private IEnumerator<IAction> _actionEnumerator;

        public ActionSequence(int speed, IEnumerable<IAction> actions)
        {
            Speed = speed;
            Actions = actions;

            _actionEnumerator = Actions.GetEnumerator();
            _actionEnumerator.MoveNext();
        }

        public bool HasAction()
        {
            return (_actionEnumerator.Current != null);
        }

        public IAction GetAction()
        {
            System.Diagnostics.Debug.Assert(HasAction());
            return _actionEnumerator.Current;
        }

        internal void Advance()
        {
            _actionEnumerator.MoveNext();
        }
    }
}
