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

        public void Activate(ISchedulable source, Tile target)
        {
            if (source is Actor actor && target.IsWalkable)
            {
                int prevX = actor.X;
                int prevY = actor.Y;
                Game.Map.SetActorPosition(actor, target.X, target.Y);
                Animation = Option.Some<IAnimation>(new MoveAnimation(Game.StateHandler.CurrentLayer, actor, prevX, prevY));
            }
        }
    }
}
