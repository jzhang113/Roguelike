using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.State;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ApplyCommand : ITargetCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;
        public IEnumerable<Terrain> Target { get; set; }

        private readonly char _key;
        private ItemCount _itemCount;

        public ApplyCommand(Actor source, char key, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            Target = targets;

            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _itemCount))
            {
                Game.MessageHandler.AddMessage("No such item to apply.");
                return new RedirectMessage(false);
            }

            if (_itemCount.Item is IUsable usable)
            {
                IAction action = usable.ApplySkill;

                if (action.Area.Aimed)
                {
                    if (Target != null)
                        return new RedirectMessage(true);

                    if (action.Area.Target != null)
                    {
                        Target = action.Area.GetTilesInRange(Source);
                    }
                    else
                    {
                        Game.StateHandler.PushState(new TargettingState(this, Source, action));
                        return new RedirectMessage(false);
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
                Game.MessageHandler.AddMessage($"Don't know how to apply {_itemCount.Item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_itemCount?.Item != null);

            ItemCount itemCount = Source.Inventory.Split(new ItemCount { Item = _itemCount.Item, Count = 1 });
            IUsable usableItem = (IUsable)itemCount.Item;
            usableItem.Apply(Source, Target);
        }
    }
}
