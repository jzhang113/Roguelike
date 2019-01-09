using Optional;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    internal class IgniteAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => 0;
        public int EnergyCost => Data.Constants.FULL_TURN;
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        public IgniteAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(ISchedulable source, Tile target) =>
            Game.Map.GetActor(target.X, target.Y).MatchSome(targetUnit =>
            {
                targetUnit.StatusHandler.AddStatus(Statuses.StatusType.Burning, 10);

                Game.MessageHandler.AddMessage(source is Fire
                    ? $"{targetUnit.Name} caught on fire!"
                    : $"{source.Name} set {targetUnit.Name} on fire!");
            });
    }
}
