using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
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

            HP = 100;
            SP = 50;
            MP = 50;
        }

        protected Player(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override ICommand Act() => InputHandler.HandleInput();

        public override void TriggerDeath() => Game.GameOver();
    }
}
