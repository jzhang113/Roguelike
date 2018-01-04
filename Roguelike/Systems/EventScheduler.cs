using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    class EventScheduler
    {
        private MinActionHeap _eventSet;
        private int _allottedTime;

        public EventScheduler(int size)
        {
            _eventSet = new MinActionHeap(size);
            _allottedTime = 0;
        }

        public void Schedule(IAction action)
        {
            action.Source.QueuedTime = action.Time;
            action.Source.CanAct = false;
            _eventSet.Add(action);

            if (action.Source is Player)
                _allottedTime = action.Time;
        }

        public bool Update()
        {
            if (!_eventSet.IsEmpty() && _allottedTime > 0)
            {
                do
                {
                    IAction action = _eventSet.GetMin();
                    action.Source.QueuedTime -= action.Time;
                    _allottedTime -= action.Time;
                    _eventSet.UpdateAllActions(action.Time);

                    if (action.Source.State != State.Dead)
                    {
                        action.Execute();
                        action.Source.CanAct = true;
                    }
                } while (_eventSet.HasFreeAction());

                return true;
            }

            return false;
        }
    }
}
