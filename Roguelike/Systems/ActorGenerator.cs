using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using System;
using System.Collections.Generic;
using Pcg;
using System.Linq;

namespace Roguelike.Systems
{
    // Generate Actors from base parts. Parts grant either abilities (movement, attacks) or
    // modifiers (poison resistance). Actors have a point allotment to balance difficulty.
    internal static class ActorGenerator
    {
        private static IList<ActorPart> PartList { get; }
        
        // TODO: generator should be non-static and initialized with the world layout. Then create
        // can be called for each level (or region?) to create specific creatures
        static ActorGenerator()
        {
            // TODO: represent parts as a tree structure to enforce prerequisites
            PartList = new[]
            {
                new ActorPart("Legs", 2, ActorFlag.CanMove),
                new ActorPart("Wings", 6, ActorFlag.CanFly),
                new ActorPart("Lightning Feet", 3, ActorFlag.FastMove),
                new ActorPart("Quick", 8, ActorFlag.Fast)
            };
        }

        // TODO: allow generator to specify a bias toward certain parts / traits
        // TODO: generator should grant a personality
        public static ActorType Generate(int points, PcgRandom random)
        {
            ICollection<string> parts = new List<string>();
            ICollection<ActorFlag> flags = new List<ActorFlag>();

            // TODO: better part selection
            // TODO: allow parts to be selected multiple times
            // TODO: additional balancing so actors have at least one attack and aren't too squishy
            int tries = 5;

            while (points > 0 && tries-- > 0)
            {
                ActorPart part = PartList[random.Next(PartList.Count)];
                if (parts.Contains(part.Name))
                    continue;

                parts.Add(part.Name);
                points -= part.Cost;

                foreach (ActorFlag flag in part.Flags)
                {
                    flags.Add(flag);
                }
            }

            // TODO: create stat ranges when generating
            // TODO: use leftover points as a stats boost
            return Identify(parts, flags);
        }

        public static Actor Create(ActorType type)
        {
            // TODO: create actors with stats
            return new Actor(type.Parameters, type.Color, type.Symbol);
        }

        // TODO: keep parts for descriptions
        // TODO: reject types if they have already been created
        private static ActorType Identify(ICollection<string> parts, ICollection<ActorFlag> flags)
        {
            // TODO: possible parts should be an enum
            // TODO: generate name from parts / flags
            if (parts.Contains("Wings"))
            {
                if (parts.Contains("Quick"))
                    return new ActorType(flags, "savage bird", 'b', Swatch.DbBlood);
                else
                    return new ActorType(flags, "bird", 'b', Swatch.DbSky);
            }

            if (parts.Contains("Legs"))
            {
                if (parts.Contains("Quick"))
                    return new ActorType(flags, "warhorse", 'h', Swatch.DbBlood);
                else
                    return new ActorType(flags, "horse", 'h', Swatch.DbSky);
            }

            return new ActorType(flags, "chimera", 'c', RLColor.White);
        }

        public readonly struct ActorPart
        {
            public string Name { get; }
            public int Cost { get; }
            public ICollection<ActorFlag> Flags { get; }

            public ActorPart(string name, int cost, params ActorFlag[] flags)
            {
                Name = name;
                Cost = cost;
                Flags = flags;
            }
        }

        public readonly struct ActorType
        {
            public ActorParameters Parameters { get; }
            public char Symbol { get; }
            public RLColor Color { get; }

            public ActorType(ICollection<ActorFlag> flags, string name, char symbol, RLColor color)
            {
                Parameters = new ActorParameters(name, flags.ToArray());
                Symbol = symbol;
                Color = color;
            }
        }
    }
}
