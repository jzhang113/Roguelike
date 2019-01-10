using Optional;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using Roguelike.Items;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    internal class SubinvState : ItemActionState
    {
        private readonly char _subKey;
        private readonly ItemGroup _subinv;
        private int _currIndex => CurrKey - 'a';
        protected override int Line => 2 + _subKey - 'a' + _currIndex;

        public SubinvState(ItemGroup group, char key, Func<Item, bool> selected)
        {
            CurrKey = 'a';
            Selected = selected;
            _subinv = group;
            _subKey = key;
        }

        public override Option<ICommand> HandleKeyInput(int key)
        {
            switch (InputMapping.GetInventoryInput(key))
            {
                case InventoryInput.MoveDown:
                    if (_currIndex < _subinv.TypeCount - 1)
                        CurrKey++;
                    return Option.None<ICommand>();
                case InventoryInput.MoveUp:
                    if (_currIndex > 0)
                        CurrKey--;
                    return Option.None<ICommand>();
                case InventoryInput.Open:
                    Item item = _subinv.GetItem(_currIndex);
                    return ResolveInput(item);
                case InventoryInput.OpenLetter:
                    char charKey = key.ToChar();
                    if (_subinv.HasIndex(charKey - 'a'))
                    {
                        CurrKey = charKey;
                        goto case InventoryInput.Open;
                    }
                    else
                    {
                        return Option.None<ICommand>();
                    }
                default:
                    return Option.None<ICommand>();
            }
        }

        public override Option<ICommand> HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            if (leftClick)
            {
                Item item = _subinv.GetItem(_currIndex);
                return ResolveInput(item);
            }

            CurrKey = (char)(y + _subKey - 2);
            if (CurrKey < 'a')
                CurrKey = 'a';
            else if (_currIndex >= _subinv.TypeCount)
                CurrKey = (char)('a' + _subinv.TypeCount - 1);

            return Option.None<ICommand>();
        }

        protected override Option<ICommand> ResolveInput(Item item)
        {
            if (item != null)
                Game.StateHandler.PushState(new ItemMenuState(item, CurrKey, _subKey, Selected));
            return Option.None<ICommand>();
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);
            Game.Player.Inventory.DrawStackSelected(layer, _subKey, Selected);
        }
    }
}
