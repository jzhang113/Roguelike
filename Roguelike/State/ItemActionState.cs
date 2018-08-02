using RLNET;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Utils;

namespace Roguelike.State
{
    abstract class ItemActionState : IState
    {
        public virtual ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return null;

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
            Game.ForceRender();
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

        public virtual void Draw(RLConsole inventoryConsole)
        {
            inventoryConsole.Clear(0, Colors.FloorBackground, Colors.Text);
            Game.Player.Inventory.Draw(inventoryConsole);
        }
    }
}
