﻿using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Player : Actor
    {
        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
        }

        public override ICommand Act() => InputHandler.HandleInput();

        public override void TriggerDeath() => Game.GameOver();
    }
}
