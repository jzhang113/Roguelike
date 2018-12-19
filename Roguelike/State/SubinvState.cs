using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    internal class SubinvState : ItemActionState
    {
        private char _subKey;
        private int _currKey;
        private ItemStack _subinv;

        internal SubinvState(char curr)
        {
            _subKey = curr;
            _currKey = 0;
            Game.Player.Inventory.TryGetKey(curr, out _subinv);
        }

        public override ICommand HandleKeyInput(int key)
        {
            if (key == Terminal.TK_DOWN)
            {
                if (_currKey < _subinv.TypeCount - 1)
                    _currKey++;
            }
            else if (key == Terminal.TK_UP)
            {
                if (_currKey > 0)
                    _currKey--;
            }
            else if (key == Terminal.TK_ENTER || key == Terminal.TK_SPACE)
            {
                System.Diagnostics.Debug.Assert(_subinv.HasIndex(_currKey));
                // TODO: getting item from dict based on index is inefficient
                // TODO: open use item menu
            }
            else
            {
                char charKey = key.ToChar();
                if (_subinv.HasIndex(charKey))
                    _currKey = charKey;
            }

            return null;
        }

        public override ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return base.HandleMouseInput(x, y, leftClick, rightClick);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Update(ICommand command)
        {
            base.Update(command);
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            throw new NotImplementedException();
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);

            Game.Player.Inventory.DrawItemStack(layer, _subKey);

            int row = 2 + _subKey - 'a' + _currKey;
            Terminal.Color(Colors.DimText);
            Terminal.Layer(layer.Z - 1);

            for (int x = 0; x < layer.Width; x++)
            {
                layer.Put(x, row, '█');
            }

            Terminal.Layer(layer.Z);
        }
    }
}
