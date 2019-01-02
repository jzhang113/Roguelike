using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    public class Player : Actor, IEquipped
    {
        public EquipmentHandler Equipment { get; }

        public ICommand NextCommand { private get; set; }

        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
            Equipment = new EquipmentHandler();
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override ICommand GetAction()
        {
            ICommand action = NextCommand;
            NextCommand = null;
            return action;
        }

        public override ICommand GetReaction()
        {
            // TODO: what here? need to force a state change and wait for nextcommand to get set
            return null;
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
