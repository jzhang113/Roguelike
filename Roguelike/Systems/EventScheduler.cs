using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    class EventScheduler
    {
        private MaxHeap<ISchedulable> _eventSet;
        private ISchedulable _current;
        
        public EventScheduler(int size)
        {
            _eventSet = new MaxHeap<ISchedulable>(size);
        }

        public void AddActor(ISchedulable schedulable) => _eventSet.Add(schedulable);
        public void RemoveActor(ISchedulable schedulable) => _eventSet.Remove(schedulable);
        public void RefreshAll() => _eventSet.UpdateAll();

        public bool Update()
        {
            if (_eventSet.IsEmpty())
                return false;

            _current = _eventSet.Peek();

            //if (_current.Energy < 0)
            //    return false;

            IAction action = _current.Act();
            if (action == null)
            {
                return false;
            }

            action.Execute();
            _current.Energy -= action.EnergyCost;
            _eventSet.Add(_current);
            _current = _eventSet.GetMax();

            return true;
        }
    }
}
