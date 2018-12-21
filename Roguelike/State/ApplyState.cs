using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal sealed class ApplyState : ItemActionState
    {
        private static readonly Lazy<ApplyState> _instance = new Lazy<ApplyState>(() => new ApplyState());
        public static ApplyState Instance => _instance.Value;

        private ApplyState()
        {
        }

        protected override ICommand ResolveInput(Item item)
        {
            if (!(item is IUsable usableItem))
            {
                Game.MessageHandler.AddMessage($"Cannot apply {item}.");
                return null;
            }

            IAction action = usableItem.ApplySkill;
            TargettingState state = new TargettingState(Game.Player, action.Area, returnTarget =>
            {
                Game.Player.Inventory.Split(item, 1);
                return new ApplyCommand(Game.Player, usableItem, returnTarget);
            });
            Game.StateHandler.PushState(state);
            return null;
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);
            // highlight equippable items
            Game.Player.Inventory.DrawSelected(layer, x => x is IUsable);
        }
    }
}