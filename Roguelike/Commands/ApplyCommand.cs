using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.State;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ApplyCommand : ITargetCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = Utils.Constants.FULL_TURN;
        public IEnumerable<Terrain> Target { get; set; }

        private readonly IUsable _usableItem;

        public ApplyCommand(Actor source, IUsable item, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            Target = targets;

            _usableItem = item;
        }

        public RedirectMessage Validate()
        {
            IAction action = _usableItem.ApplySkill;

            if (action.Area.Aimed)
            {
                if (Target != null)
                    return new RedirectMessage(true);

                if (action.Area.Target != null)
                {
                    Target = action.Area.GetTilesInRange(Source);
                }
                else
                {
                    Game.StateHandler.PushState(new TargettingState(this, Source, action));
                    return new RedirectMessage(false);
                }
            }
            else
            {
                if (Target == null)
                    Target = action.Area.GetTilesInRange(Source);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _usableItem.Apply(Source, Target);
        }
    }
}
