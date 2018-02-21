using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    // Effectively the main game loop. Actions are processed until one becomes null, which should
    // only occur with the Player.
    class EventScheduler
    {
        private MaxHeap<ISchedulable> _eventSet;
        
        public EventScheduler(int size)
        {
            _eventSet = new MaxHeap<ISchedulable>(size);
        }

        public void AddActor(ISchedulable schedulable) => _eventSet.Add(schedulable);
        public void RemoveActor(ISchedulable schedulable) => _eventSet.Remove(schedulable);

        public bool Update()
        {
            if (_eventSet.IsEmpty())
                return false;

            ISchedulable current = _eventSet.Peek();

            // If the current Actor has negative energy, then everyone must have negative energy.
            // Give everyone some energy and carry on acting.
            if (current.Energy <= 0)
            {
                Game.MessageHandler.AddMessage("Energy Refresh", OptionHandler.MessageLevel.Verbose);
                System.Console.WriteLine("New turn!");
                System.Console.WriteLine("Turn order");
                System.Console.WriteLine("----------");
                for (int i = 0; i < _eventSet.Size(); i++)
                    System.Console.WriteLine(string.Format("#{0} {1}\t {2} energy", _eventSet.GetOrder()[i], ((Actors.Actor)_eventSet.GetHeap()[i]).Name, _eventSet.GetHeap()[i].Energy));

                _eventSet.UpdateAll();
            }

            // Break the event loop when there is no Action. Currently, the only situation where
            // this occurs is in the input handling for the Player's Actions.
            IAction action = current.Act();
            if (action == null)
                return false;

            // Check that the action can succeed before executing it. If there are potential
            // alternative actions, try them as well.
            RedirectMessage status = action.Validate();
            while (!status.Success && status.Alternative != null)
            {
                action = status.Alternative;
                status = action.Validate();
            } 

            // If we still don't succeed, the action is bad and should be cancelled. However, we
            // should be careful that the AI does not give invalid Actions, as this will lead to
            // an infinite loop.
            if (!status.Success)
            {
                // TODO: cancellation code
                return false;
            }

            action.Execute();
            current.Energy -= action.EnergyCost;

            // Move the current Actor to the bottom of the heap.
            _eventSet.GetMax();
            _eventSet.Add(current);

            return true;
        }
    }
}
