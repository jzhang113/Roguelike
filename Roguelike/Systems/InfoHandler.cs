using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Statuses;
using System.Drawing;

namespace Roguelike.Systems
{
    internal static class InfoHandler
    {
        public static void Draw(LayerInfo layer)
        {
            Actor player = Game.Player;

            const int stepSize = 5;
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $"{player.Hp}/{player.Parameters.MaxHp}";

            Terminal.Color(Colors.Text);
            layer.Print(1, health);

            Terminal.Color(Swatch.DbBlood);
            for (int i = 0; i <= hpFilled; i++)
                layer.Put(i + 1, 1, Terminal.Pick(i + 1, 1));

            Terminal.Color(Swatch.DbOldBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                layer.Put(i + 1, 1, Terminal.Pick(i + 1, 1));

            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                layer.Put(i + hpWidth + 3, 1, Terminal.Pick(i + hpWidth + 3, 1));

            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp}";

            Terminal.Color(Colors.Text);
            layer.Print(2, mana);

            Terminal.Color(Swatch.DbWater);
            for (int i = 0; i <= mpFilled; i++)
                layer.Put(i + 1, 2, Terminal.Pick(i + 1, 2));

            Terminal.Color(Swatch.DbDeepWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                layer.Put(i + 1, 2, Terminal.Pick(i + 1, 2));

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
