using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    internal class IgniteAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        public IgniteAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(ISchedulable source, Tile target)
        {
            System.Diagnostics.Debug.Assert(target != null);

            if (!Game.Map.TryGetActor(target.X, target.Y, out Actor targetUnit))
                return;

            targetUnit.StatusHandler.AddStatus(Statuses.StatusType.Burning, 10);

            Game.MessageHandler.AddMessage(source is Fire
                ? $"{targetUnit.Name} caught on fire!"
                : $"{source.Name} set {targetUnit.Name} on fire!");
        }
    }
}
