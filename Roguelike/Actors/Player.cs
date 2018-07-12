using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Player : Actor, IEquipped
    {
        public EquipmentHandler Equipment { get; }

        public ICommand NextCommand { private get; set; }

        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
            Equipment = new EquipmentHandler();
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear NextCommand
        // once it has been sent.
        public override ICommand Act()
        {
            ICommand action = NextCommand;
            NextCommand = null;
            return action;
        }

        public override void TriggerDeath() => Game.GameOver();
        public override IAction GetBasicAttack()
        {
            if (Equipment.IsDefaultWeapon())
                return base.GetBasicAttack();
            else
                return Equipment.PrimaryWeapon.Attack();
        }
    }
}
