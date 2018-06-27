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

        public override ICommand Act() => Game.StateHandler.HandleInput();

        public override void TriggerDeath() => Game.GameOver();
    }
}
