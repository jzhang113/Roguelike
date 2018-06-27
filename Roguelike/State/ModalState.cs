using RLNET;
using Roguelike.Commands;
using Roguelike.Systems;
using Roguelike.Core;

namespace Roguelike.State
{
    class ModalState : IState
    {
        public virtual ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            return null;
        }

        public virtual ICommand HandleMouseInput(RLMouse mouse)
        {
            // TODO
            return null;
        }

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
