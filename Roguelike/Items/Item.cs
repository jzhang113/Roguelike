using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;

namespace Roguelike.Items
{
    abstract class Item : Drawable
    {
        public string Name { get; protected set; }
        public IMaterial Material { get; protected set; }
        public Actor Carrier { get; set; }

        public override RLColor Color { get; set; }
        public override char Symbol { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        protected IList<ActionSequence> Abilities { get; set; }

        protected int AttackSpeed { get; set; }
        protected int Damage { get; set; }
        protected int MeleeRange { get; set; }
        protected int ThrowRange { get; set; }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public virtual void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Throw()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public ActionSequence GetBasicAttack()
        {
            List<IAction> actions = new List<IAction>
            {
                new DamageAction(Damage, new Core.TargetZone(Core.TargetShape.Ray))
            };

            return new ActionSequence(AttackSpeed, actions);
        }

        public ActionSequence GetAbility(int index)
        {
            if (index >= Abilities.Count)
                return null;
            else
                return Abilities[index];
        }

        public void AddAbility(ActionSequence skill)
        {
            // TODO: check that the skill doesn't already exist
            Abilities.Add(skill);
        }
    }
}
