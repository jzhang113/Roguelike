using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Statuses;

namespace Roguelike.Systems
{
    static class InfoHandler
    {
        public static void Draw(RLConsole console)
        {
            Actor player = Game.Player;

            int stepSize = 5;
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $"{player.Hp}/{player.Parameters.MaxHp}";
            console.Print((hpWidth - health.Length) / 2 + 2, 1, health, Colors.Text);
            for (int i = 0; i <= hpFilled; i++)
                console.SetBackColor(i + 1, 1, Swatch.DbBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                console.SetBackColor(i + 1, 1, Swatch.DbOldBlood);

            int armorWidth = player.Armor / stepSize;
            for (int i = 0; i <= armorWidth; i++)
                console.SetBackColor(i + hpWidth + 3, 1, Swatch.DbMetal);

            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp}";
            console.Print((mpWidth - mana.Length) / 2 + 2, 2, mana, Colors.Text);
            for (int i = 0; i <= mpFilled; i++)
                console.SetBackColor(i + 1, 2, Swatch.DbWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                console.SetBackColor(i + 1, 2, Swatch.DbDeepWater);

            int pos = 1;
            if (player.StatusHandler.TryGetStatus(StatusType.Phasing, out _))
            {
                console.Print(pos, 4, "Phasing", Swatch.DbMetal);
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Burning, out _))
            {
                console.Print(pos, 4, "Burning", Colors.Fire);
                pos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Frozen, out _))
            {
                console.Print(pos, 4, "Frozen", Colors.Water);
                pos += 7;
            }
        }
    }
}
