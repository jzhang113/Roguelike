using Optional;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Items;
using Roguelike.Systems;
using System.Linq;

namespace Roguelike.Commands
{
    internal class PickupCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Data.Constants.HALF_TURN;
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly Option<InventoryHandler> _itemStack;

        public PickupCommand(Actor source, Option<InventoryHandler> itemStack)
        {
            Source = source;
            _itemStack = itemStack;
        }

        public RedirectMessage Validate() =>
            _itemStack.Match(
                some: _ => new RedirectMessage(true),
                none: () =>
                {
                    Game.MessageHandler.AddMessage("There's nothing to pick up here.");
                    return new RedirectMessage(false);
                });

        public void Execute() =>
            _itemStack.MatchSome(stack =>
            {
                Item item;
                switch (stack.Count)
                {
                    case 1:
                        item = stack.First();
                        Game.Map.SplitItem(item).Match(
                            some: split =>
                            {
                                Source.Inventory.Add(split);
                                Game.MessageHandler.AddMessage($"You pick up {split}.");
                            },
                            none: () => Game.MessageHandler.AddMessage($"No such {item}."));
                        break;
                    default:
                        if (Source is Player)
                        {
                            // HACK: handle pickup menu - this placeholder at least lets you pick up the top item
                            item = stack.First();
                            Game.Map.SplitItem(item).Match(
                                some: split =>
                                {
                                    Source.Inventory.Add(split);
                                    Game.MessageHandler.AddMessage($"You pick up {split}.");
                                },
                                none: () => Game.MessageHandler.AddMessage($"No such {item}."));
                        }
                        else
                        {
                            // HACK: Monsters will simply grab the top item off of a pile if they try to pick stuff up.
                            item = stack.First();
                            Game.Map.SplitItem(item).MatchSome(split =>
                            {
                                Source.Inventory.Add(split);
                                // TODO: Tell the player only if they can see / notice this
                                Game.MessageHandler.AddMessage($"{Source} picks up {split}.");
                            });
                        }
                        break;
                }
            });
    }
}
