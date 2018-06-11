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

        public ApplyCommand(Actor source, char key, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            Target = targets;

            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.HasKey(_key))
            {
                Game.MessageHandler.AddMessage("No such item to apply.");
                return new RedirectMessage(false);
            }

            Item item = Source.Inventory.GetItem(_key).Item;
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
            ItemInfo itemGroup = Source.Inventory.GetItem(_key);
            Source.Inventory.Remove(itemGroup);

            IUsable usableItem = itemGroup.Item as IUsable;
            usableItem.Apply(Target);
        }
    }
}
