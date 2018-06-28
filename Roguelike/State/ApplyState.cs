using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;
using System.Collections.Generic;

namespace Roguelike.State
{
    class ApplyState : ItemActionState
    {
        private static readonly Lazy<ApplyState> _instance = new Lazy<ApplyState>(() => new ApplyState());
        public static ApplyState Instance => _instance.Value;

        private ApplyState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            if (itemCount.Item is IUsable usableItem)
            {
                IAction action = usableItem.ApplySkill;
                if (action.Area.Aimed)
                {
                    if (action.Area.Target == null)
                    {
                        TargettingState state = new TargettingState(Game.Player, action);
                        state.Complete += (sender, args) =>
                            Game.Player.Inventory.Split(new ItemCount {Item = itemCount.Item, Count = 1});
                        Game.StateHandler.PushState(state);
                        return null;
                    }
                }

                IEnumerable<Terrain> target = action.Area.GetTilesInRange(Game.Player);
                Game.Player.Inventory.Split(new ItemCount { Item = itemCount.Item, Count = 1 });
                return new ApplyCommand(Game.Player, usableItem, target);
            }

            Game.MessageHandler.AddMessage($"Cannot apply {itemCount.Item}.");
            return null;
        }
    }
}