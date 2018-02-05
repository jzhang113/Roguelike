using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Core
{
    // Provides the default behavior for Actors.
    class SimpleAI
    {
        // Monsters will wander randomly if awake and chase the Player if s/he is in their perception
        // range. If their health falls below a certain threshold, they will attempt to flee.
        public static IAction GetAction(Actor monster)
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
                        WeightedPoint dir = Move.Directions[Game.CombatRandom.Next() % 8];
                        return new MoveAction(monster, monster.X + dir.X, monster.Y + dir.Y);
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
                        return new MoveAction(monster, nextMove.X, nextMove.Y);
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
                        WeightedPoint nextMove = Game.Map.MoveTowardsTarget(monster.X, monster.Y, Game.Map.FleeMap);
                        return new MoveAction(monster, nextMove.X, nextMove.Y);
                    }
                case ActorState.Dead:
                    // Remove dead things if they die before they finish acting.
                    Game.EventScheduler.RemoveActor(monster);
                    return null;
                case ActorState.Sleep:
                    return new MoveAction(monster, monster.X, monster.Y);
                default:
                    // We should not be here
                    System.Diagnostics.Debug.Assert(false);
                    return new MoveAction(monster, monster.X, monster.Y);
            }
        }
    }
}
