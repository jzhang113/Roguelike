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
            _eventSet.Add(action);
        }

        public bool Update()
        {
            if (!_eventSet.IsEmpty())
            {
                IAction action = _eventSet.GetMin();
                action.Execute();
                _eventSet.UpdateAllActions(action.Time);

                return true;
            }

            return false;
        }
    }
}
