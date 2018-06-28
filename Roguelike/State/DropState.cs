﻿using Roguelike.Commands;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    class DropState : ItemActionState
    {
        private static readonly Lazy<DropState> _instance = new Lazy<DropState>(() => new DropState());
        public static DropState Instance => _instance.Value;

        private DropState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            if (itemCount.Count == 1)
                return new DropCommand(Game.Player, itemCount, 1);

            Game.StateHandler.PushState(new TextInputState(input =>
            {
                if (int.TryParse(input, out int dropAmount) && dropAmount > 0)
                    return new DropCommand(Game.Player, itemCount, dropAmount);

                Game.MessageHandler.AddMessage($"Unknown amount: {input}");
                return null;
            }));
            return null;
        }
    }
}