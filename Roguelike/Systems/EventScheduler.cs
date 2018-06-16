using Roguelike.Interfaces;
using Roguelike.Utils;
using System.Collections.Generic;
using Roguelike.Commands;

namespace Roguelike.Systems
{
    // Effectively the main game loop. Actions are processed until one becomes null, which should
    // only occur with the Player.
    class EventScheduler
    {
        private readonly ICollection<ISchedulable> _entities;
        private readonly MaxHeap<ISchedulable> _eventSet;

        public EventScheduler(int size)
        {
            _entities = new HashSet<ISchedulable>();
            _eventSet = new MaxHeap<ISchedulable>(size);
        }

        public void AddActor(ISchedulable schedulable) => _entities.Add(schedulable);
        public void RemoveActor(ISchedulable schedulable) => _eventSet.Remove(schedulable);

        public void Clear()
        {
            _entities.Clear();
        }

        public bool Update()
        {
            if (_entities.Count == 0)
                return false;

            _eventSet.Clear();
            
            // Check if any actor is already ready to act
            foreach (ISchedulable entity in _entities)
            {
                if (entity.Energy >= Constants.MIN_TURN_ENERGY)
                {
                    _eventSet.Add(entity);
                }
            }

            // No one is currently ready, so continually apply energy recovery to all entities in the
            // system, until the queue has at least one entity to execute
            while (_eventSet.Count == 0)
            {
                foreach (ISchedulable entity in _entities)
                {
                    entity.Energy += entity.RefreshRate;

                    // Once the entity has enough energy, it can be added to the turn queue
                    // The queue will prioritize entities by their energy ratings, so the entities
                    // with the most amount of energy will execute their turn first
                    if (entity.Energy >= Constants.MIN_TURN_ENERGY)
                    {
                        _eventSet.Add(entity);
                    }
                }
            }

            // Dequeue and execute the handler for each entities in the turn queue until empty
            foreach (ISchedulable current in _eventSet)
            {
                // Break the event loop when there is no Action.
                // This should only happen with input handling for the Player's Actions.
                ICommand action = current.Act();
                if (action == null)
                {
                    System.Diagnostics.Debug.Assert(current is Actors.Player);
                    return false;
                }

                // Check that the action can succeed before executing it. If there are potential
                // alternative actions, try them as well.
                RedirectMessage status = action.Validate();
                while (!status.Success && status.Alternative != null)
                {
                    action = status.Alternative;
                    status = action.Validate();
                }

                // If we still don't succeed, the action is bad and should be cancelled. Otherwise,
                // we can execute the action which should succeed at this point.
                if (!status.Success)
                {
                    // Let the Player pick another move. Otherwise, if the AI made an invalid move,
                    // perform a wait action to prevent an infinite loop.
                    if (current is Actors.Player)
                    {
                        Game.MessageHandler.AddMessage("Can't do that.");
                        return false;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "monster made invalid move");
                        current.Energy = 0;
                    }
                }
                else
                {
                    action.Execute();
                    current.Energy -= action.EnergyCost;
                }
            }

            return true;
        }
    }
}
