using Roguelike.Commands;
using Roguelike.Core;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Player : Actor
    {
        public ICommand NextCommand { private get; set; }

        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear NextCommand
        // once it has been sent.
        public override ICommand Act()
        {
            ICommand action = NextCommand;
            NextCommand = null;
            return action;
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
