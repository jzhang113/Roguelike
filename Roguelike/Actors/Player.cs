using MessagePack;
using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actors
{
    class Player : Actor, IEquipped
    {
        public EquipmentHandler Equipment { get; }

        [IgnoreMember]
        public ICommand NextCommand { private get; set; }

        public Player(ActorParameters parameters) : base(parameters, Colors.Player, '@')
        {
            Equipment = new EquipmentHandler();
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override ICommand GetAction()
        {
            ICommand action = NextCommand;
            NextCommand = null;
            return action;
        }

        public override void TriggerDeath() => Game.GameOver();
        public override IAction BasicAttack
        {
            get
            {
                if (Equipment.IsDefaultWeapon())
                    return base.BasicAttack;
                else
                    return Equipment.PrimaryWeapon.Attack();
            }
        }
    }
}
