using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Input;
using Roguelike.Items;
using Roguelike.Utils;

namespace Roguelike.State
{
    internal abstract class ItemActionState : IState
    {
        public bool Nonblocking => false;

        protected virtual char CurrKey { get; set; }

        protected virtual int Line => 1 + CurrKey - 'a';

        protected ItemActionState()
        {
            CurrKey = 'a';
        }

        public virtual ICommand HandleKeyInput(int key)
        {
            switch (InputMapping.GetInventoryInput(key))
            {
                case InventoryInput.MoveDown:
                    if (CurrKey < Game.Player.Inventory.LastKey)
                        CurrKey++;
                    return null;
                case InventoryInput.MoveUp:
                    if (CurrKey > 'a')
                        CurrKey--;
                    return null;
                case InventoryInput.Open:
                    return HandleOpen();
                case InventoryInput.OpenLetter:
                    char charKey = key.ToChar();
                    if (Game.Player.Inventory.HasKey(charKey))
                    {
                        CurrKey = charKey;
                        goto case InventoryInput.Open;
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        public virtual ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            if (leftClick)
                return HandleOpen();

            CurrKey = (char)(y + 'a' - 1);
            if (CurrKey < 'a')
                CurrKey = 'a';
            else if (CurrKey > Game.Player.Inventory.LastKey)
                CurrKey = Game.Player.Inventory.LastKey;

            return null;
        }

        private ICommand HandleOpen()
        {
            if (Game.Player.Inventory.IsStacked(CurrKey))
            {
                ItemGroup group = Game.Player.Inventory.GetStack(CurrKey);
                Game.StateHandler.PushState(new SubinvState(group, CurrKey));
                return null;
            }
            else
            {
                return ResolveInput(Game.Player.Inventory.GetItem(CurrKey));
            }
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

            Terminal.Color(Colors.RowHighlight);
            Terminal.Layer(layer.Z - 1);

            for (int x = 0; x < layer.Width; x++)
            {
                layer.Put(x, Line, '█');
            }

            Terminal.Layer(layer.Z);
        }
    }
}
