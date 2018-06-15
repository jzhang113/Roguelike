using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ApplyCommand : ICommand, ITargettable
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;
        public IEnumerable<Terrain> Target { get; set; }

        private readonly char _key;
        private ItemInfo _itemGroup;

        public ApplyCommand(Actor source, char key, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            Target = targets;

            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _itemGroup))
            {
                Game.MessageHandler.AddMessage("No such item to apply.");
                return new RedirectMessage(false);
            }

            Item item = _itemGroup.Item;
            if (item is IUsable)
            {
                IAction action = (item as IUsable).ApplySkill;

                if (action.Area.Aimed)
                {
                    if (Target == null)
                    {
                        if (action.Area.Target != null)
                        {
                            Target = action.Area.GetTilesInRange(Source);
                        }
                        else
                        {
                            InputHandler.BeginTargetting(this, Source, action);
                            return new RedirectMessage(false);
                        }
                    }
                }
                else
                {
                    if (Target == null)
                        Target = action.Area.GetTilesInRange(Source);
                }

                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage($"Don't know how to apply {item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            Source.Inventory.Remove(_itemGroup);

            IUsable usableItem = (IUsable)_itemGroup.Item;
            usableItem.Apply(Target);
        }
    }
}
