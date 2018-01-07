using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Core
{
    class SimpleAI
    {
        public static IAction GetAction(IActor monster)
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
                    return null;
                default: return null;
            }
        }
    }
}
