using Roguelike.Animations;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Systems
{
    // Effectively the main game loop. Actions are processed until one becomes null, which should
    // only occur with the Player. While the Player is in the heap as a sentinel, the actual input
    // handling is managed by the States.
    public class EventScheduler
    {
        public static int Turn { get; private set; }

        private readonly ICollection<ISchedulable> _entities;
        private readonly MaxHeap<ISchedulable> _eventSet;
        private bool _stopping;

        public EventScheduler(int size)
        {
            Turn = 0;
            _entities = new HashSet<ISchedulable>();
            _eventSet = new MaxHeap<ISchedulable>(size);
            _stopping = false;
        }

        public void AddActor(ISchedulable schedulable) => _entities.Add(schedulable);

        public void RemoveActor(ISchedulable schedulable)
        {
            if (schedulable is DelayAttack attack)
            {
                foreach (Loc point in attack.Targets)
                {
                    Game.Threatened.Unset(point.X, point.Y);
                }
            }

            _entities.Remove(schedulable);
        }

        // Instead of clearing everything immediately, give everything on the level a chance to
        // finish processing, then clear it before the next cycle begins.
        public void Clear()
        {
            _entities.Clear();
            _stopping = true;
        }

        public void Stop()
        {
            _stopping = true;
        }

        // Run updates for all actors until it is the Player's turn to act again.
        public void Run()
        {
            do
            {
                // TODO: Not certain if this is a correct implementation. However, it fixes an issue
                // where the first input after Game.NewGame() is "eaten" since Clear() is called, but
                // _stopping doesn't get set to false until Update() - which forces it to return false
                if (_stopping)
                {
                    _eventSet.Clear();
                    _stopping = false;
                }

                // No one is currently ready, so continually apply energy recovery to all entities in the
                // system, until the queue has at least one entity to execute.
                while (_eventSet.Count == 0)
                {
                    foreach (ISchedulable entity in _entities.ToList())
                    {
                        // Everyone gains some energy
                        entity.Energy += Data.Constants.DEFAULT_REFRESH_RATE;

                        // Entities with sufficient energy are placed into the turn queue.
                        if (entity.Energy >= entity.ActivationEnergy)
                            _eventSet.Add(entity);
                    }
                }

                foreach (ISchedulable entity in _entities)
                {
                    // Manage other stuff tied to Actor speeds
                    if (entity is DelayAttack attack && attack.Lifetime > 0)
                    {
                        var blend = Colors.EnemyThreat.Blend(Colors.FloorBackground, (double)attack.Energy / attack.ActivationEnergy);
                        foreach (Loc point in attack.Targets)
                        {
                            Game.Threatened.Set(point.X, point.Y, blend);
                            Game.Animations.Add(new FlashAnimation(Game.StateHandler.CurrentLayer, point.X, point.Y, blend));
                        }
                    }
                }
            } while (Update());
        }

        private bool Update()
        {
            // Dequeue and execute the handler for each entities in the turn queue until empty.
            while (_eventSet.Count > 0)
            {
                if (_stopping)
                {
                    _eventSet.Clear();
                    _stopping = false;
                    return false;
                }

                ISchedulable current = _eventSet.Peek();
                ICommand action = current.Act();

                if (Execute(current, action))
                {
                    _eventSet.PopMax();

                    if (current.Lifetime == 0) // -1 is used as nonexpiring
                    {
                        RemoveActor(current);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        // Perform a specified action immediately. Support to queue actions may be added as needed.
        internal static bool Execute(ISchedulable current, ICommand command)
        {
            // Break the event loop when there is no Action.
            // This should only happen with input handling for the Player's Actions. Note that even
            // though we never explicitly pass null for Player Actions, if a player gets to move
            // twice, we need to pass control back to let the player move
            if (command == null)
            {
                System.Diagnostics.Debug.Assert(current is Actors.Player);
                return false;
            }

            // Check that the action can succeed before executing it. If there are potential
            // alternative actions, try them as well.
            RedirectMessage status = command.Validate();
            while (!status.Success && status.Alternative != null)
            {
                command = status.Alternative;
                status = command.Validate();
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
                if (current is Actors.Player)
                    Turn++;

                command.Execute();
                command.Animation.MatchSome(animation => Game.Animations.Add(animation));
                current.Energy -= command.EnergyCost;
            }

            return true;
        }
    }
}
