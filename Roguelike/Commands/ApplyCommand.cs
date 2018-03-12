﻿using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ApplyCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        private char _key;
        private IEnumerable<Terrain> _target;

        public ApplyCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.HasKey(_key))
            {
                Game.MessageHandler.AddMessage("No such item to apply.", OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(false);
            }
            
            Item item = Source.Inventory.GetItem(_key);
            if (item is IUsable)
            {
                var skill = (item as IUsable).ApplySkill;

                if (skill.Area.Aimed)
                {
                    if (_target == null)
                    {
                        InputHandler.BeginTargetting(this, skill);
                        Source.Inventory.Remove(item);
                        // TODO: Handle targetted items to allow cancelling

                        return new RedirectMessage(false);
                    }
                }
                else
                {
                    if (_target == null)
                        _target = skill.Area.GetTilesInRange(Source);
                }

                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage(string.Format("Cannot apply {0}.", item.Name), OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            Item item = Source.Inventory.GetItem(_key);
            Source.Inventory.Remove(item);
            (item as IUsable).Apply(_target);
        }
    }
}
