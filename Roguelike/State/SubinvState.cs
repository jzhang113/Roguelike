using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using Roguelike.Items;
using Roguelike.Utils;

namespace Roguelike.State
{
    internal class SubinvState : ItemActionState
    {
        private readonly char _subKey;
        private readonly ItemGroup _subinv;
        private int _currIndex => CurrKey - 'a';
        protected override int Line => 2 + _subKey - 'a' + _currIndex;

        public SubinvState(ItemGroup group, char key)
        {
            CurrKey = 'a';
            _subinv = group;
            _subKey = key;
        }

        public override ICommand HandleKeyInput(int key)
        {
            switch (InputMapping.GetInventoryInput(key))
            {
                case InventoryInput.MoveDown:
                    if (_currIndex < _subinv.TypeCount - 1)
                        CurrKey++;
                    return null;
                case InventoryInput.MoveUp:
                    if (_currIndex > 0)
                        CurrKey--;
                    return null;
                case InventoryInput.Open:
                    Item it = _subinv.GetItem(_currIndex);
                    return (it != null) ? ResolveInput(it) : null;
                case InventoryInput.OpenLetter:
                    char charKey = key.ToChar();
                    if (_subinv.HasIndex(charKey - 'a'))
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

        public override ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            if (leftClick)
            {
                Item it = _subinv.GetItem(_currIndex);
                return (it != null) ? ResolveInput(it) : null;
            }

            CurrKey = (char)(y + _subKey - 2);
            if (CurrKey < 'a')
                CurrKey = 'a';
            else if (_currIndex >= _subinv.TypeCount)
                CurrKey = (char)('a' + _subinv.TypeCount - 1);

            return null;
        }

        protected override ICommand ResolveInput(Item item)
        {
            Game.StateHandler.PushState(new ItemMenuState(item, CurrKey, _subKey));
            return null;
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);
            Game.Player.Inventory.DrawItemStack(layer, _subKey);
        }
    }
}
