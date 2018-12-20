using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    internal sealed class InventoryState : ItemActionState
    {
        private static readonly Lazy<InventoryState> _instance = new Lazy<InventoryState>(() => new InventoryState());
        public static InventoryState Instance => _instance.Value;

        private char _currKey;

        private InventoryState()
        {
            _currKey = 'a';
        }

        public override ICommand HandleKeyInput(int key)
        {
            var inv = Game.Player.Inventory;

            // TODO: what if inv is empty?
            if (key == Terminal.TK_DOWN)
            {
                if (_currKey < inv.LastKey)
                    _currKey++;
            }
            else if (key == Terminal.TK_UP)
            {
                if (_currKey > 'a')
                    _currKey--;
            }
            else if (key == Terminal.TK_ENTER || key == Terminal.TK_SPACE)
            {
                System.Diagnostics.Debug.Assert(inv.HasKey(_currKey));
                if (Game.Player.Inventory.IsStacked(_currKey))
                {
                    Game.StateHandler.PushState(new SubinvState(_currKey));
                }
                else
                {
                    Game.StateHandler.PushState(new ItemMenuState(_currKey));
                }
            }
            else
            {
                char charKey = key.ToChar();
                if (inv.HasKey(charKey))
                    _currKey = charKey;
            }

            return null;
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            throw new NotImplementedException();
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);

            int row = 1 + _currKey - 'a';
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