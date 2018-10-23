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
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.Put(0, 0, '╦'); // 203
            layer.Put(layer.Width - 1, 0, '╦');

            for (int x = 1; x < layer.Width - 1; x++)
            {
                layer.Put(x, 0, '═'); // 205
            }

            for (int y = 1; y < layer.Height; y++)
            {
                layer.Put(0, y, '║'); // 186
                layer.Put(layer.Width - 1, y, '║'); // 186
            }

            Actor player = Game.Player;
            const int stepSize = 5;
            Terminal.Composition(true);

            string name = player.Name.ToUpper();
            layer.Print(1, $"\\ {name} /", System.Drawing.ContentAlignment.TopCenter);

            // HP bar
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $" {player.Hp}/{player.Parameters.MaxHp}";

            Terminal.Color(Swatch.DbBlood);
            for (int i = 0; i <= hpFilled; i++)
                layer.Put(i + 1, 1, '█');

            Terminal.Color(Swatch.DbOldBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                layer.Put(i + 1, 1, '█');

            Terminal.Color(Colors.Text);
            layer.Print(1, health);

            // Armor
            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                layer.Put(i + hpWidth + 3, 1, '█');

            // MP bar
            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp} ";

            Terminal.Color(Swatch.DbWater);
            for (int i = 0; i <= mpFilled; i++)
                layer.Put(layer.Width - i - 2, 1, '█');

            Terminal.Color(Swatch.DbDeepWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                layer.Put(layer.Width - i - 2, 1, '█');

            Terminal.Color(Colors.Text);
            layer.Print(1, mana, System.Drawing.ContentAlignment.TopRight);

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
