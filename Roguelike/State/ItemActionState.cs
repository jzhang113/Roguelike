﻿using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Utils;

namespace Roguelike.State
{
    internal abstract class ItemActionState : IState
    {
        public virtual ICommand HandleKeyInput(int key)
        {
            char keyChar = key.ToChar();
            if (!Game.Player.Inventory.TryGetKey(keyChar, out ItemCount itemCount))
            {
                Game.MessageHandler.AddMessage("No such item.");
                return null;
            }

            System.Diagnostics.Debug.Assert(itemCount.Item != null);
            System.Diagnostics.Debug.Assert(itemCount.Count > 0);
            return ResolveInput(itemCount);
        }

        public virtual ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // TODO do stuff and get the item selected
            return null;
        }

        protected abstract ICommand ResolveInput(ItemCount itemCount);

        public virtual void Update()
        {
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();

            //Q: can any item action events even have animations?
            if (command.Animation != null)
                Game.StateHandler.PushState(new AnimationState(command.Animation));
        }

        public virtual void Draw(LayerInfo layer)
        {
            Game.ShowEquip = false;
            BearLib.Terminal.Color(Swatch.DbSun);
            layer.Print(0, "INVENTORY", System.Drawing.ContentAlignment.TopCenter);
        }
    }
}
