using BearLib;
using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal class ItemMenuState : ItemActionState
    {
        private readonly Item _item;
        private readonly bool _fromSubinv;
        private readonly char _subinvKey;

        private readonly bool _usable;
        private readonly bool _equippable;

        protected override int Line => _fromSubinv ? base.Line + _subinvKey - 'a' + 1 : base.Line;

        public ItemMenuState(Item item, char prevKey, char subinvKey, Func<Item, bool> selected)
        {
            CurrKey = prevKey;
            Selected = selected;
            _item = item;

            if (subinvKey != '\0')
            {
                _fromSubinv = true;
                _subinvKey = subinvKey;
            }
            else
            {
                _fromSubinv = false;
            }

            _usable = item is IUsable;
            _equippable = item is IEquippable;
        }

        public override ICommand HandleKeyInput(int key)
        {
            switch (key)
            {
                case Terminal.TK_A:
                    if (!_usable)
                    {
                        Game.MessageHandler.AddMessage($"Cannot apply {_item}.");
                        return null;
                    }

                    IAction action = ((IUsable)_item).ApplySkill;
                    TargettingState state = new TargettingState(Game.Player, action.Area, returnTarget =>
                    {
                        Item usable = Game.Player.Inventory.Split(_item, 1);
                        Game.StateHandler.PopState();
                        return new ApplyCommand(Game.Player, usable as IUsable, returnTarget);
                    });
                    Game.StateHandler.PushState(state);
                    return null;
                case Terminal.TK_C:
                case Terminal.TK_T:
                    return null;
                case Terminal.TK_W:
                    if (!_equippable)
                    {
                        Game.MessageHandler.AddMessage($"Cannot equip {_item}.");
                        return null;
                    }

                    Item split = Game.Player.Inventory.Split(_item, 1);
                    IEquippable equipable = split as IEquippable;
                    return new EquipCommand(Game.Player, equipable);
                default:
                    return null;
            }
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

            if (_fromSubinv)
                Game.Player.Inventory.DrawStackSelected(layer, _subinvKey, Selected);

            LayerInfo itemMenu = new LayerInfo("Item menu", layer.Z + 1, layer.X + 0, layer.Y + Line + 2, 9, 4);
            itemMenu.Clear();
            Terminal.Color(Colors.HighlightColor);
            itemMenu.DrawBorders(new BorderInfo
            {
                TopLeftChar = '╠', // 204
                TopRightChar = '╗', // 187
                BottomLeftChar = '╠',
                BottomRightChar = '╝', // 188
                TopChar = '═', // 205
                BottomChar = '═',
                LeftChar = '║', // 186
                RightChar = '║'
            });

            if (_usable)
            {
                Terminal.Color(Colors.HighlightColor);
                itemMenu.Print(0, 0, "(a)[color=white]pply");
            }
            else
            {
                Terminal.Color(Colors.DimText);
                itemMenu.Print(0, 0, "(a)pply");
            }

            // TODO: check edibility
            Terminal.Color(Colors.DimText);
            itemMenu.Print(0, 1, "(c)onsume");

            Terminal.Color(Colors.HighlightColor);
            itemMenu.Print(0, 2, "(t)[color=white]hrow");

            if (_equippable)
            {
                Terminal.Color(Colors.HighlightColor);
                itemMenu.Print(0, 3, "(w)[color=white]ear");
            }
            else
            {
                Terminal.Color(Colors.DimText);
                itemMenu.Print(0, 3, "(w)ear");
            }
        }
    }
}
