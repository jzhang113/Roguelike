using Roguelike.Commands;
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
            return new DropCommand(Game.Player, itemCount);
        }
    }
}