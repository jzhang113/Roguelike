using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearLib;
using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;

namespace Roguelike.State
{
    internal class ItemMenuState : ItemActionState
    {
        private readonly ItemCount _item;
        private readonly int _line;

        public ItemMenuState(char curr)
        {
            Game.Player.Inventory.TryGetKey(curr, out ItemStack stack);
            _item = stack.First();
            _line = 1 + curr - 'a';
        }

        public override ICommand HandleKeyInput(int key)
        {
            switch (key)
            {
                case Terminal.TK_A:
                    if (!(_item.Item is IUsable usableItem))
                    {
                        Game.MessageHandler.AddMessage($"Cannot apply {_item.Item}.");
                        return null;
                    }

                    IAction action = usableItem.ApplySkill;
                    TargettingState state = new TargettingState(Game.Player, action.Area, returnTarget =>
                    {
                        Game.Player.Inventory.Split(new ItemCount { Item = _item.Item, Count = 1 });
                        return new ApplyCommand(Game.Player, usableItem, returnTarget);
                    });
                    Game.StateHandler.PushState(state);
                    return null;
                case Terminal.TK_C:
                case Terminal.TK_T:
                    return null;
                case Terminal.TK_W:
                    if (!(_item.Item is IEquippable))
                    {
                        Game.MessageHandler.AddMessage($"Cannot equip {_item.Item}.");
                        return null;
                    }

                    ItemCount splitCount = Game.Player.Inventory.Split(new ItemCount
                    {
                        Item = _item.Item,
                        Count = 1
                    });
                    IEquippable equipable = splitCount.Item as IEquippable;
                    return new EquipCommand(Game.Player, equipable);
                default:
                    return null;
            }
        }

        public override ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return base.HandleMouseInput(x, y, leftClick, rightClick);
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            throw new NotImplementedException();
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);

            LayerInfo itemMenu = new LayerInfo("Item menu", layer.Z + 1, layer.X + 0, layer.Y + _line + 2, 9, 4);
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

            Terminal.Color(Colors.HighlightColor);
            itemMenu.Print(0, 0, "(a)[color=white]pply");
            itemMenu.Print(0, 1, "(c)[color=white]onsume");
            itemMenu.Print(0, 2, "(t)[color=white]hrow");
            itemMenu.Print(0, 3, "(w)[color=white]ear");
        }
    }
}
