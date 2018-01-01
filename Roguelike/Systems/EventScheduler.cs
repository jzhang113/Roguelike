using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    class EventScheduler
    {
        private MinHeap _eventSet;

        public EventScheduler(int size)
        {
            _eventSet = new MinHeap(size);
        }

        public void Schedule(IAction action)
        {
            action.Source.QueuedTime = action.Time;
            _eventSet.Add(action);
        }

        public bool Update()
        {
            if (!_eventSet.IsEmpty())
            {
                IAction action = _eventSet.GetMin();
                action.Execute();
                action.Source.QueuedTime -= action.Time;
                _eventSet.UpdateAllActions(action.Time, action.Source);

                return true;
            }

            return false;
        }
    }
}
