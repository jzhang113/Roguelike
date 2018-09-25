using BearLib;
using Roguelike.Core;
using Roguelike.Items;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        public void Draw()
        {
            Terminal.Color(Colors.Text);
            Terminal.Print(1, 1, ContentAlignment.TopCenter, "== Equipment ==");
            Terminal.Print(1, 1, "Weapon - " + PrimaryWeapon.Name);
            Terminal.Print(1, 2, "Offhand - " + (OffhandWeapon?.Name ?? "none"));
            Terminal.Print(1, 3, "Armor - " + (Armor[ArmorType.Armor]?.Name ?? "none"));
            Terminal.Print(1, 4, "Helmet - " + (Armor[ArmorType.Helmet]?.Name ?? "none"));
            Terminal.Print(1, 5, "Gloves - " + (Armor[ArmorType.Gloves]?.Name ?? "none"));
            Terminal.Print(1, 6, "Boots - " + (Armor[ArmorType.Boots]?.Name ?? "none"));
        }
    }
}