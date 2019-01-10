using Optional;
using Roguelike.Actions;
using Roguelike.Commands;
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
            Selected = x => x is IUsable;
        }

        protected override Option<ICommand> ResolveInput(Item item)
        {
            if (!(item is IUsable usableItem))
            {
                Game.MessageHandler.AddMessage($"Cannot apply {item}.");
                return Option.None<ICommand>();
            }

            IAction action = usableItem.ApplySkill;
            TargettingState state = new TargettingState(Game.Player, action.Area, returnTarget =>
            {
                Item split = Game.Player.Inventory.Split(item, 1);
                Game.StateHandler.PopState(); // exit targetting state
                return new ApplyCommand(Game.Player, split as IUsable, returnTarget);
            });
            Game.StateHandler.PushState(state);
            return Option.None<ICommand>();
        }
    }
}