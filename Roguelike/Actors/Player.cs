using Roguelike.Commands;
using Roguelike.Core;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Player : Actor
    {
        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
        }

        // Sentinel so that the EventScheduler breaks if it is the Player's turn to act.
        public override ICommand Act() => null;

        public override void TriggerDeath() => Game.GameOver();
    }
}
