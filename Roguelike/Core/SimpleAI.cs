using Roguelike.Actors;
using Roguelike.Commands;

namespace Roguelike.Core
{
    // Provides the default behavior for Actors.
    // ReSharper disable once InconsistentNaming
    static class SimpleAI
    {
        // Monsters will wander randomly if awake and chase the Player if s/he is in their perception
        // range. If their health falls below a certain threshold, they will attempt to flee.
        public static ICommand GetAction(Actor monster)
        {
            switch (monster.State)
            {
                case ActorState.Wander:
                    if (Game.Map.PlayerMap[monster.X, monster.Y] < monster.Parameters.Awareness)
                    {
                        monster.State = ActorState.Chase;
                        return GetAction(monster);
                    }
                    else
                    {
                        (int dx, int dy) = Direction.DirectionList[Game.World.Random.Next(8)];
                        return new MoveCommand(monster, monster.X + dx, monster.Y + dy);
                    }
                case ActorState.Chase:
                    // TODO: set dynamic threshold
                    if (monster.Hp < 10)
                    {
                        monster.State = ActorState.Flee;
                        return GetAction(monster);
                    }
                    else
                    {
                        WeightedPoint nextMove = Game.Map.MoveTowardsTarget(
                            monster.X, monster.Y, Game.Map.PlayerMap);
                        return new MoveCommand(monster, nextMove.X, nextMove.Y);
                    }
                case ActorState.Flee:
                    // TODO: set dynamic threshold
                    if (monster.Hp > 100)
                    {
                        monster.State = ActorState.Wander;
                        return GetAction(monster);
                    }
                    else
                    {
                        // implement fleeing?
                        (int dx, int dy) = Direction.DirectionList[Game.World.Random.Next(8)];
                        return new MoveCommand(monster, monster.X + dx, monster.Y + dy);
                    }
                case ActorState.Dead:
                    // Remove dead things if they die before they finish acting.
                    monster.TriggerDeath();
                    return new WaitCommand(monster);
                case ActorState.Sleep:
                    if (Game.Map.PlayerMap[monster.X, monster.Y] < monster.Parameters.Awareness)
                    {
                        monster.State = ActorState.Chase;
                        return GetAction(monster);
                    }
                    else
                    {
                        return new WaitCommand(monster);
                    }
                default:
                    // We should not be here
                    System.Diagnostics.Debug.Assert(false);
                    return new WaitCommand(monster);
            }
        }
    }
}
