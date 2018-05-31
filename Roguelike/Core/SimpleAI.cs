using Roguelike.Commands;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Enums;

namespace Roguelike.Core
{
    // Provides the default behavior for Actors.
    class SimpleAI
    {
        // Monsters will wander randomly if awake and chase the Player if s/he is in their perception
        // range. If their health falls below a certain threshold, they will attempt to flee.
        public static ICommand GetAction(Actor monster)
        {
            switch (monster.State)
            {
                case ActorState.Wander:
                    if (Game.Map.PlayerMap[monster.X, monster.Y] < monster.Awareness)
                    {
                        monster.State = ActorState.Chase;
                        return GetAction(monster);
                    }
                    else
                    {
                        WeightedPoint dir = Direction.Directions[Game.CombatRandom.Next() % 8];
                        return new MoveCommand(monster, monster.X + dir.X, monster.Y + dir.Y);
                    }
                case ActorState.Chase:
                    // TODO: set dynamic threshold
                    if (monster.HP < 10)
                    {
                        monster.State = ActorState.Flee;
                        return GetAction(monster);
                    }
                    else
                    {
                        WeightedPoint nextMove = Game.Map.MoveTowardsTarget(monster.X, monster.Y, Game.Map.PlayerMap);
                        return new MoveCommand(monster, nextMove.X, nextMove.Y);
                    }
                case ActorState.Flee:
                    // TODO: set dynamic threshold
                    if (monster.HP > 100)
                    {
                        monster.State = ActorState.Wander;
                        return GetAction(monster);
                    }
                    else
                    {
                        // implement fleeing?
                        WeightedPoint dir = Direction.Directions[Game.CombatRandom.Next() % 8];
                        return new MoveCommand(monster, monster.X + dir.X, monster.Y + dir.Y);
                    }
                case ActorState.Dead:
                    // Remove dead things if they die before they finish acting.
                    monster.TriggerDeath();
                    return new WaitCommand(monster);
                case ActorState.Sleep:
                    return new WaitCommand(monster);
                default:
                    // We should not be here
                    System.Diagnostics.Debug.Assert(false);
                    return new WaitCommand(monster);
            }
        }
    }
}
