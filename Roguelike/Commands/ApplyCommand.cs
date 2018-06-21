using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System.Collections.Generic;
using Roguelike.Actions;

namespace Roguelike.Commands
{
    class ApplyCommand : ICommand, ITargetCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;
        public IEnumerable<Terrain> Target { get; set; }

        private readonly char _key;
        private Item _item;

        public ApplyCommand(Actor source, char key, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            Target = targets;

            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _item))
            {
                Game.MessageHandler.AddMessage("No such item to apply.");
                return new RedirectMessage(false);
            }

            if (_item is IUsable usable)
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
                        InputHandler.BeginTargetting(this, Source, action);
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
                Game.MessageHandler.AddMessage($"Don't know how to apply {_item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_item != null);
            
            IUsable usableItem = (IUsable)Source.Inventory.Split(_item, 1);
            usableItem.Apply(Source, Target);
        }
    }
}
