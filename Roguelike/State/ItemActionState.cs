using RLNET;
using Roguelike.Commands;
using Roguelike.Systems;
using Roguelike.Core;
using Roguelike.Utils;
using Roguelike.Items;

namespace Roguelike.State
{
    abstract class ItemActionState : IState
    {
        public virtual ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            char keyChar = keyPress.Key.ToChar();
            if (!Game.Player.Inventory.TryGetKey(keyChar, out ItemCount itemCount))
            {
                Game.MessageHandler.AddMessage("No such item.");
                return null;
            }

            System.Diagnostics.Debug.Assert(itemCount.Item != null);
            System.Diagnostics.Debug.Assert(itemCount.Count > 0);
            return ResolveInput(itemCount);
        }

        public virtual ICommand HandleMouseInput(RLMouse mouse)
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

            if (EventScheduler.Execute(Game.Player, command))
                Game.StateHandler.PopState();
        }

        public virtual void Draw()
        {
            Game.InventoryConsole.Clear(0, Colors.FloorBackground, Colors.TextHeading);
            Game.Player.Inventory.Draw(Game.InventoryConsole);
            RLConsole.Blit(Game.InventoryConsole, 0, 0, Game.Config.InventoryView.Width, Game.Config.InventoryView.Height, Game.RootConsole, Game.Config.Map.Width - 10, 0);
        }
    }
}
