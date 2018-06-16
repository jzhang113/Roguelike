using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Actors
{
    [Serializable]
    class Player : Actor
    {
        public Player()
        {
            Awareness = 10;
            Name = "Player";
            DrawingComponent.Color = Colors.Player;
            DrawingComponent.Symbol = '@';

            Hp = 100;
            Sp = 50;
            Mp = 50;
        }

        protected Player(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override ICommand Act() => InputHandler.HandleInput();

        public override void TriggerDeath() => Game.GameOver();
    }
}
