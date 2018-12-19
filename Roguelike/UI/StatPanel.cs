using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Statuses;

namespace Roguelike.UI
{
    internal static class StatPanel
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '╤', // 209
                TopRightChar = '╤',
                TopChar = '═', // 205
                LeftChar = '│', // 179
                RightChar = '│'
            });

            Actor player = Game.Player;
            const int stepSize = 5;
            const int yPos = 0;

            string name = player.Name.ToUpper();
            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]{name}[/color]{Constants.HEADER_RIGHT}",
                System.Drawing.ContentAlignment.TopCenter);

            Terminal.Color(Colors.Text);
            Terminal.Composition(true);
            // HP bar
            int hpWidth = player.Parameters.MaxHp / stepSize;
            int hpFilled = hpWidth * player.Hp / player.Parameters.MaxHp;
            string health = $" {player.Hp}/{player.Parameters.MaxHp}";

            Terminal.Color(Swatch.DbBlood);
            for (int i = 0; i <= hpFilled; i++)
                layer.Put(i, yPos, '█');

            Terminal.Color(Swatch.DbOldBlood);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                layer.Put(i, yPos, '█');

            Terminal.Color(Colors.Text);
            layer.Print(yPos, health);

            // Armor
            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                layer.Put(i + hpWidth + 2, yPos, '█');

            // MP bar
            int mpWidth = player.Parameters.MaxMp / stepSize;
            int mpFilled = mpWidth * player.Mp / player.Parameters.MaxMp;
            string mana = $"{player.Mp}/{player.Parameters.MaxMp} ";

            Terminal.Color(Swatch.DbWater);
            for (int i = 0; i <= mpFilled; i++)
                layer.Put(layer.Width - i - 1, yPos, '█');

            Terminal.Color(Swatch.DbDeepWater);
            for (int i = mpFilled + 1; i <= mpWidth; i++)
                layer.Put(layer.Width - i - 1, yPos, '█');

            Terminal.Color(Colors.Text);
            layer.Print(yPos, mana, System.Drawing.ContentAlignment.TopRight);

            Terminal.Composition(false);

            // Statuses
            int xPos = 0;
            if (player.StatusHandler.TryGetStatus(StatusType.Phasing, out _))
            {
                Terminal.Color(Swatch.DbMetal);
                layer.Print(xPos, yPos + 1, "Phasing");
                xPos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Burning, out _))
            {
                Terminal.Color(Colors.Fire);
                layer.Print(xPos, yPos + 1, "Burning");
                xPos += 8;
            }

            if (player.StatusHandler.TryGetStatus(StatusType.Frozen, out _))
            {
                Terminal.Color(Colors.Water);
                layer.Print(xPos, yPos + 1, "Frozen");
                xPos += 7;
            }
        }
    }
}
