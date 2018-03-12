using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    class EquipmentHandler
    {
        public Weapon DefaultWeapon { get; internal set; }
        public Weapon PrimaryWeapon { get; internal set; }
        public Weapon OffhandWeapon { get; internal set; }
        public IDictionary<ArmorType, Armor> Armor {get; private set;}

        public EquipmentHandler(Weapon defaultWeapon)
        {
            DefaultWeapon = defaultWeapon;
            PrimaryWeapon = DefaultWeapon;

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

        public bool IsDefaultWeapon() => PrimaryWeapon == DefaultWeapon && OffhandWeapon == null;

        public void Draw(RLConsole console)
        {
            console.Print(1, 1, "Weapon - " + PrimaryWeapon.Name, Colors.TextHeading);
            console.Print(1, 2, "Offhand - " + (OffhandWeapon?.Name ?? "none"), Colors.TextHeading);
            console.Print(1, 3, "Armor - " + (Armor[ArmorType.Armor]?.Name ?? "none"), Colors.TextHeading);
            console.Print(1, 4, "Helmet - " + (Armor[ArmorType.Helmet]?.Name ?? "none"), Colors.TextHeading);
            console.Print(1, 5, "Gloves - " + (Armor[ArmorType.Gloves]?.Name ?? "none"), Colors.TextHeading);
            console.Print(1, 6, "Boots - " + (Armor[ArmorType.Boots]?.Name ?? "none"), Colors.TextHeading);
        }
    }
}