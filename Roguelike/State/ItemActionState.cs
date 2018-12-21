using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Items;
using Roguelike.Utils;

namespace Roguelike.State
{
    internal abstract class ItemActionState : IState
    {
        public bool Nonblocking => false;

        public virtual ICommand HandleKeyInput(int key)
        {
            char keyChar = key.ToChar();
            Item item = Game.Player.Inventory.GetItem(keyChar);

            // TODO: what about stacks
            if (item == null)
            {
                Game.MessageHandler.AddMessage("No such item.");
                return null;
            }

            return ResolveInput(item);
        }

        public virtual ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // TODO do stuff and get the item selected
            return null;
        }

        protected abstract ICommand ResolveInput(Item item);

        public virtual void Update(ICommand command)
        {
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
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '╔', // 201
                TopRightChar = '╗', // 187
                BottomLeftChar = '╚', // 200
                BottomRightChar = '╝', // 188
                TopChar = '═', // 205
                BottomChar = '═',
                LeftChar = '║', // 186
                RightChar = '║'
            });
            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]INVENTORY{Constants.HEADER_SEP}" +
                $"[color=grass]EQUIPMENT[/color]{Constants.HEADER_RIGHT}");
        }
    }
}
