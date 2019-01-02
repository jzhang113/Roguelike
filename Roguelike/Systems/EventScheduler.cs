using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Utils;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    // Effectively the main game loop. Actions are processed until one becomes null, which should
    // only occur with the Player. While the Player is in the heap as a sentinel, the actual input
    // handling is managed by the States.
    public class EventScheduler
    {
        private readonly ICollection<ISchedulable> _entities;
        private readonly MaxHeap<ISchedulable> _eventSet;

        public EventScheduler(int size)
        {
            _entities = new HashSet<ISchedulable>();
            _eventSet = new MaxHeap<ISchedulable>(size);
        }

        public void AddActor(ISchedulable schedulable) => _entities.Add(schedulable);
        public void RemoveActor(ISchedulable schedulable) => _entities.Remove(schedulable);

        // Instead of clearing everything immediately, give everything on the level a chance to
        // finish processing, then clear it before the next cycle begins.
        public void Clear()
        {
            _entities.Clear();
            _eventSet.Clear();
        }

        // Run updates for all actors until it is the Player's turn to act again.
        public void Run()
        {
            do
            {
                // No one is currently ready, so continually apply energy recovery to all entities in the
                // system, until the queue has at least one entity to execute.
                while (_eventSet.Count == 0)
                {
                    foreach (ISchedulable entity in _entities)
                    {
                        // Entities with sufficient energy are placed into the turn queue.
                        if (entity.Energy > Data.Constants.MIN_TURN_ENERGY)
                            _eventSet.Add(entity);

                        // and everyone gains some energy
                        entity.Energy += entity.RefreshRate;
                    }
                }
            } while (Update());
        }

        private bool Update()
        {
            // Dequeue and execute the handler for each entities in the turn queue until empty.
            while (_eventSet.Count > 0)
            {
                ISchedulable current = _eventSet.Peek();
                ICommand action = current.Act();

                if (Execute(current, action))
                    _eventSet.PopMax();
                else
                    return false;
            }

            return true;
        }

        // Perform a specified action immediately. Support to queue actions may be added as needed.
        internal static bool Execute(ISchedulable current, ICommand action)
        {
            // Break the event loop when there is no Action.
            // This should only happen with input handling for the Player's Actions. Note that even
            // though we never explicitly pass null for Player Actions, if a player gets to move
            // twice, we need to pass control back to let the player move
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
                    Game.MessageHandler.AddMessage("An invalid action was made.", MessageLevel.Verbose);
                    return false;
                }
                else
                {
                    System.Diagnostics.Debug.Fail("monster made invalid move");
                    current.Energy = 0;
                }
            }
            else
            {
                action.Execute();
                current.Energy -= action.EnergyCost;
            }

            return true;
        }
    }
}
