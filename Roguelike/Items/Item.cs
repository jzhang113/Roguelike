using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;
using Roguelike.Utils;
using RLNET;
using System.Runtime.Serialization;

namespace Roguelike.Items
{
    [Serializable]
    public class Item: ISerializable
    {
        public string Name { get; }
        public IMaterial Material { get; }
        public bool BlocksLight { get; }

        public Actor Carrier { get; set; }
        public Drawable DrawingComponent { get; }

        public int X
        {
            get => DrawingComponent.X;
            set => DrawingComponent.X = value;
        }

        public int Y
        {
            get => DrawingComponent.Y;
            set => DrawingComponent.Y = value;
        }

        public RLColor Color
        {
            get => DrawingComponent.Color;
            set => DrawingComponent.Color = value;
        }

        internal int AttackSpeed { get; set; } = Constants.FULL_TURN;
        internal int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        internal float MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        internal float ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        private IList<IAction> _abilities { get; }

        public Item(string name, IMaterial material, bool blocksLight = false)
        {
            Name = name;
            Material = material;
            BlocksLight = blocksLight;
            DrawingComponent = new Drawable();

            _abilities = new List<IAction>();
        }

        protected Item(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(nameof(Name));
            Material = (IMaterial)info.GetValue(nameof(Material), typeof(IMaterial));
            BlocksLight = info.GetBoolean(nameof(BlocksLight));

            DrawingComponent = (Drawable)info.GetValue(nameof(DrawingComponent), typeof(Drawable));

            AttackSpeed = info.GetInt32(nameof(AttackSpeed));
            Damage = info.GetInt32(nameof(Damage));
            MeleeRange = info.GetInt32(nameof(MeleeRange));
            ThrowRange = info.GetInt32(nameof(ThrowRange));

            _abilities = (IList<IAction>)info.GetValue(nameof(_abilities), typeof(IList<IAction>));
        }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
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

        public IAction GetBasicAttack(int targetX, int targetY)
        {
            return new DamageAction(Damage, new Core.TargetZone(Enums.TargetShape.Directional, (targetX, targetY)));
        }

        public IAction GetAbility(int index)
        {
            if (index >= _abilities.Count)
                return null;
            else
                return _abilities[index];
        }

        public void AddAbility(IAction skill)
        {
            // TODO: check that the skill doesn't already exist
            _abilities.Add(skill);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Material), Material);
            info.AddValue(nameof(BlocksLight), BlocksLight);

            info.AddValue(nameof(DrawingComponent), DrawingComponent);

            info.AddValue(nameof(AttackSpeed), AttackSpeed);
            info.AddValue(nameof(Damage), Damage);
            info.AddValue(nameof(MeleeRange), MeleeRange);
            info.AddValue(nameof(ThrowRange), ThrowRange);

            info.AddValue(nameof(_abilities), _abilities);
        }
    }
}
