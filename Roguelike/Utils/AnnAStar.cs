using Roguelike.Core;
using System;
using System.Collections.Generic;

namespace Roguelike.Utils
{
    // Implementing annotated A* - essentially A* but considers a bit of extra information
    // (clearance) as well
    public static class AnnAStar
    {
        public static Dictionary<Loc, int> explored = new Dictionary<Loc, int>();
        public static Dictionary<Loc, Loc> prev = new Dictionary<Loc, Loc>();

        public static IEnumerable<Loc> Search(Loc start, in Loc goal, int clearance)
        {
            explored.Clear();
            prev.Clear();
            var path = new List<Loc>();

            var frontier = new MaxHeap<LocCost>(16)
            {
                new LocCost(start, 0)
            };

            prev[start] = start;
            explored[start] = 0;

            while (frontier.Count > 0)
            {
                LocCost current = frontier.PopMax();

                if (current.Loc.X == goal.X && current.Loc.Y == goal.Y)
                    break;

                foreach (Loc next in Game.Map.GetPointsInRadius(current.Loc, 1))
                {
                    if (Game.Map.Clearance[next.X, next.Y] < clearance)
                        continue;

                    // assuming all squares have equal movement cost - update if this is not so
                    int newCost = current.Cost + 1;

                    if (!explored.ContainsKey(next) || newCost < explored[next])
                    {
                        explored[next] = newCost;
                        int priority = newCost + Heuristic(next, goal);
                        frontier.Add(new LocCost(next, priority));
                        prev[next] = current.Loc;
                    }
                }
            }

            return path;
        }

        private static int Heuristic(in Loc a, in Loc b)
        {
            // manhattan distance, exact
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
