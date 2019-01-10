using Optional;
using Roguelike.Commands;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal sealed class InventoryState : ItemActionState
    {
        private static readonly Lazy<InventoryState> _instance = new Lazy<InventoryState>(() => new InventoryState());
        public static InventoryState Instance => _instance.Value;

        private InventoryState()
        {
        }

        protected override Option<ICommand> ResolveInput(Item item)
        {
            Game.StateHandler.PushState(new ItemMenuState(item, CurrKey, '\0', Selected));
            return Option.None<ICommand>();
        }
    }
}