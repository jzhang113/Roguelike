using Roguelike.Interfaces;
using Roguelike.Systems;
using System;

namespace Roguelike.Core
{
    enum State { Wander, Chase, Flee, Dead };

    class SimpleAI
    {
        public static IAction GetAction(IActor monster, Random random)
        {
            switch (monster.State)
            {
                case State.Wander:
                    if (Game.Map.PlayerDistance[monster.X, monster.Y] < monster.Awareness)
                    {
                        monster.State = State.Chase;
                        return GetAction(monster, random);
                    }
                    else
                    {
                        WeightedPoint dir = Move.Directions[random.Next() % 8];
                        return new MoveAction(monster, monster.X + dir.X, monster.Y + dir.Y);
                    }
                case State.Chase:
                    if (monster.HP < 10)
                    {
                        monster.State = State.Flee;
                        return GetAction(monster, random);
                    }
                    else
                    {
                        WeightedPoint nextMove = Game.Map.MoveTowardsPlayer(monster.X, monster.Y);
                        return new MoveAction(monster, nextMove.X, nextMove.Y);
                    }
                case State.Flee:
                    if (monster.HP > 100)
                    {
                        monster.State = State.Wander;
                        return GetAction(monster, random);
                    }
                    else
                    {
                        WeightedPoint nextMove = Game.Map.MoveAwayFromPlayer(monster.X, monster.Y);
                        return new MoveAction(monster, nextMove.X, nextMove.Y);
                    }
                case State.Dead:
                    return null;
                default: return null;
            }
        }
    }
}
