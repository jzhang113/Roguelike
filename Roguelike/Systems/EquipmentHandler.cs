using BearLib;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Items;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    [Serializable]
    public class EquipmentHandler
    {
        public Weapon PrimaryWeapon { get; set; }
        public Weapon OffhandWeapon { get; set; }
        public IDictionary<ArmorType, Armor> Armor { get; }

        public EquipmentHandler()
        {
            Armor = new Dictionary<ArmorType, Armor>
            {
                [ArmorType.Armor] = null,
                [ArmorType.Boots] = null,
                [ArmorType.Gloves] = null,
                [ArmorType.Helmet] = null,
                [ArmorType.RingLeft] = null,
                [ArmorType.RingRight] = null
            };
        }

        public bool IsDefaultWeapon() => PrimaryWeapon == null;

        public void Equip(Weapon weapon)
        {
            PrimaryWeapon = weapon;
        }

        public void Swap()
        {
            Weapon temp = PrimaryWeapon;
            PrimaryWeapon = OffhandWeapon;
            OffhandWeapon = temp;
        }

        public void Equip(Armor armor)
        {
            Armor[armor.Type] = armor;
        }

        public void Unequip()
        {
            PrimaryWeapon = null;
        }

        public void Unequip(Armor armor)
        {
            Armor[armor.Type] = null;
        }

        public void Draw(LayerInfo layer)
        {
            Game.ShowEquip = true;
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopRightChar = '╗', // 187
                BottomRightChar = '╝', // 188
                TopChar = '═', // 205
                BottomChar = '═',
                RightChar = '║' // 186
            });
            layer.Print(-1, $"{Constants.HEADER_LEFT}INVENTORY" +
                $"[color=white]{Constants.HEADER_SEP}EQUIPMENT{Constants.HEADER_RIGHT}");

            Terminal.Color(Colors.Text);
            layer.Print(2, "a) Primary:    " + (PrimaryWeapon?.Name ?? "none"));
            layer.Print(3, "b) Offhand:    " + (OffhandWeapon?.Name ?? "none"));
            layer.Print(4, "c) Armor:      " + (Armor[ArmorType.Armor]?.Name ?? "none"));
            layer.Print(5, "d) Helmet:     " + (Armor[ArmorType.Helmet]?.Name ?? "none"));
            layer.Print(6, "e) Gloves:     " + (Armor[ArmorType.Gloves]?.Name ?? "none"));
            layer.Print(7, "f) Boots:      " + (Armor[ArmorType.Boots]?.Name ?? "none"));
            layer.Print(8, "g) Left ring:  " + (Armor[ArmorType.RingLeft]?.Name ?? "none"));
            layer.Print(9, "h) Right ring: " + (Armor[ArmorType.RingRight]?.Name ?? "none"));
        }
    }
}