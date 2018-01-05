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
                    if (Game.Map.PlayerDistance[monster.X, monster.Y] < monster.Awareness)
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
                        WeightedPoint nextMove = Game.Map.MoveTowardsPlayer(monster.X, monster.Y);
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
                        WeightedPoint nextMove = Game.Map.MoveAwayFromPlayer(monster.X, monster.Y);
                        return new MoveAction(monster, nextMove.X, nextMove.Y);
                    }
                case ActorState.Dead:
                    return null;
                default: return null;
            }
        }
    }
}
