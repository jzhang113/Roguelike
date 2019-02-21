using Optional;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class MoveAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => 0;
        public int EnergyCost => Data.Constants.FULL_TURN;
        public Option<IAnimation> Animation { get; private set; }

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(ISchedulable source, Loc target)
        {
            if (source is Actor actor && Game.Map.Field[target].IsWalkable)
            {
                Loc prevLoc = actor.Loc;
                Game.Map.SetActorPosition(actor, target);
                Animation = Option.Some<IAnimation>(new MoveAnimation(Game.StateHandler.CurrentLayer, actor, prevLoc));
            }
        }
    }
}
