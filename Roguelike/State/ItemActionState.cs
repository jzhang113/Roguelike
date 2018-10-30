using BearLib;
using Roguelike.Commands;
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
            // highlight draw borders
            Terminal.Color(Colors.HighlightColor);
            layer.Put(layer.Width - 1, 0, '╗'); // 187
            layer.Put(layer.Width - 1, layer.Height - 1, '╝'); // 188
            layer.Put(-1, 0, '╔'); // 188
            layer.Put(-1, layer.Height - 1, '╚'); // 188

            for (int x = 0; x < layer.Width - 1; x++)
            {
                layer.Put(x, 0, '═'); // 205
                layer.Put(x, layer.Height - 1, '═');
            }

            for (int y = 1; y < layer.Height - 1; y++)
            {
                layer.Put(-1, y, '║');
                layer.Put(layer.Width - 1, y, '║'); // 186
            }

            Game.ShowEquip = false;
            layer.Print(0, "[color=white][[INVENTORY]][/color][color=grass]EQUIPMENT]]");
        }
    }
}
