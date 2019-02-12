using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Linq;

namespace Roguelike.Actors
{
    [Serializable]
    public class Player : Actor, IEquipped
    {
        public EquipmentHandler Equipment { get; }

        public ICommand NextCommand
        {
            set => _nextCommand = value;
        }

        [NonSerialized]
        private ICommand _nextCommand;

        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
            Equipment = new EquipmentHandler();
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override ICommand GetAction()
        {
            ICommand action = _nextCommand;
            _nextCommand = null;
            return action;
        }

        public override ReactionMessage GetReaction()
        {
            if (Game.Random.Next(100) < 50)
                return base.GetReaction();

            Game.MessageHandler.AddMessage("You dodge");

            Game.StateHandler.PushState(new State.TargettingState(Game.Player, new TargetZone(TargetShape.Range),
                returnTarget => new MoveCommand(this, returnTarget.First().X, returnTarget.First().Y)));

            return new ReactionMessage
            {
                Command = null,
                Delayed = true,
                Negating = true
            };
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
