using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Statuses;

namespace Roguelike.Systems
{
    internal static class InfoHandler
    {
        public static void Draw(LayerInfo layer)
        {
            Actor player = Game.Player;
            const int stepSize = 5;
            Terminal.Composition(true);

            // HP bar
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $"{player.Hp}/{player.Parameters.MaxHp}";

            Terminal.Color(Swatch.DbBlood);
            for (int i = 0; i <= hpFilled; i++)
                layer.Put(i + 1, 1, '█');

            Terminal.Color(Swatch.DbOldBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                layer.Put(i + 1, 1, '█');

            Terminal.Color(Colors.Text);
            layer.Print(1, 1, health);

            // Armor
            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                layer.Put(i + hpWidth + 3, 1, '█');

            // MP bar
            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp}";

            Terminal.Color(Swatch.DbWater);
            for (int i = 0; i <= mpFilled; i++)
                layer.Put(i + 1, 2, '█');

            Terminal.Color(Swatch.DbDeepWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                layer.Put(i + 1, 2, '█');

            Terminal.Color(Colors.Text);
            layer.Print(1, 2, mana);

            Terminal.Composition(false);

            // Statuses
            int pos = 1;
            if (player.StatusHandler.TryGetStatus(StatusType.Phasing, out _))
            {
                Terminal.Color(Swatch.DbMetal);
                layer.Print(pos, 4, "Phasing");
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Burning, out _))
            {
                Terminal.Color(Colors.Fire);
                layer.Print(pos, 4, "Burning");
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Frozen, out _))
            {
                Terminal.Color(Colors.Water);
                layer.Print(pos, 4, "Frozen");
                pos += 7;
            }
        }
    }
}
