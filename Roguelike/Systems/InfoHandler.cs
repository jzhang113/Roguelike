using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Statuses;

namespace Roguelike.Systems
{
    internal static class InfoHandler
    {
        public static void Draw()
        {
            Actor player = Game.Player;

            const int stepSize = 5;
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $"{player.Hp}/{player.Parameters.MaxHp}";
            Terminal.Print((hpWidth - health.Length) / 2 + 2, 1, health, Colors.Text);

            Terminal.Color(Swatch.DbBlood);
            for (int i = 0; i <= hpFilled; i++)
                Terminal.Put(i + 1, 1, Terminal.Pick(i + 1, 1));

            Terminal.Color(Swatch.DbOldBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                Terminal.Put(i + 1, 1, Terminal.Pick(i + 1, 1));

            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                Terminal.Put(i + hpWidth + 3, 1, Terminal.Pick(i + hpWidth + 3, 1));

            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp}";
            Terminal.Color(Colors.Text);
            Terminal.Print((mpWidth - mana.Length) / 2 + 2, 2, mana);

            Terminal.Color(Swatch.DbWater);
            for (int i = 0; i <= mpFilled; i++)
                Terminal.Put(i + 1, 2, Terminal.Pick(i + 1, 2));

            Terminal.Color(Swatch.DbDeepWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                Terminal.Put(i + 1, 2, Terminal.Pick(i + 1, 2));

            int pos = 1;
            if (player.StatusHandler.TryGetStatus(StatusType.Phasing, out _))
            {
                Terminal.Color(Swatch.DbMetal);
                Terminal.Print(pos, 4, "Phasing");
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Burning, out _))
            {
                Terminal.Color(Colors.Fire);
                Terminal.Print(pos, 4, "Burning");
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Frozen, out _))
            {
                Terminal.Color(Colors.Water);
                Terminal.Print(pos, 4, "Frozen");
                pos += 7;
            }
        }
    }
}
