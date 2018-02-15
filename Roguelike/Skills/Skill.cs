using Roguelike.Actors;
using System;

namespace Roguelike.Skills
{
    public abstract class Skill
    {
        public Actor Source { get; internal set; }
        public int Speed { get; }
        public int Power { get; }

        public Skill(Actor source, int speed, int power)
        {
            Source = source;
            Speed = speed;
            Power = power;
        }

        public abstract void Activate(Actor target);

        public void ApplyTargets(Func<Actor, Actor, bool> predicate, int limit)
        {
            int count = 0;

            foreach (Actor actor in Game.Map.Units)
            {
                if (count >= limit)
                    return;

                if (predicate.Invoke(Source, actor))
                {
                    Activate(actor);
                    count++;
                }
            }
        }
    }
}
