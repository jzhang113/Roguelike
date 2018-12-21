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
        private readonly char _subKey;
        private readonly ItemGroup _subinv;
        private int _currIndex;
        private int _row => 2 + _subKey - 'a' + _currIndex;

        public SubinvState(ItemGroup group, char key)
        {
            _subKey = key;
            _subinv = group;
            _currIndex = 0;
        }

        public override ICommand HandleKeyInput(int key)
        {
            if (key == Terminal.TK_DOWN)
            {
                if (_currIndex < _subinv.TypeCount - 1)
                    _currIndex++;
            }
            else if (key == Terminal.TK_UP)
            {
                if (_currIndex > 0)
                    _currIndex--;
            }
            else if (key == Terminal.TK_ENTER || key == Terminal.TK_SPACE)
            {
                OpenItemMenu();
            }
            else
            {
                char charKey = key.ToChar();
                if (_subinv.HasIndex(charKey))
                    _currIndex = charKey - 'a';
                OpenItemMenu();
            }

            return null;
        }

        private void OpenItemMenu()
        {
            Item it = _subinv.GetItem(_currIndex);
            if (it != null)
                Game.StateHandler.PushState(new ItemMenuState(it, _row));
        }

        public override ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return base.HandleMouseInput(x, y, leftClick, rightClick);
        }

        protected override ICommand ResolveInput(Item item)
        {
            throw new NotImplementedException();
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);

            Game.Player.Inventory.DrawItemStack(layer, _subKey);

            Terminal.Color(Colors.DimText);
            Terminal.Layer(layer.Z - 1);

            for (int x = 0; x < layer.Width; x++)
            {
                layer.Put(x, _row, '█');
            }

            Terminal.Layer(layer.Z);
        }
    }
}
