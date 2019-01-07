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

            Terminal.Color(Swatch.Compliment);
            for (int i = 0; i <= hpFilled; i++)
                layer.Put(i, yPos, '█');

            Terminal.Color(Swatch.ComplimentDarkest);
            for (int i = hpFilled + 1; i <= hpWidth; i++)
                layer.Put(i, yPos, '█');

            Terminal.Color(Colors.Text);
            layer.Print(yPos, health);

            // Armor
            int armorWidth = player.Armor / stepSize;
            Terminal.Color(Swatch.DbMetal);
            for (int i = 0; i <= armorWidth; i++)
                layer.Put(i + hpWidth + 2, yPos, '█');

            // SP bar
            int spWidth = player.Parameters.MaxSp / stepSize;
            int spFilled = spWidth * player.Sp / player.Parameters.MaxSp;
            string stamina = $"{player.Sp}/{player.Parameters.MaxSp} ";

            Terminal.Color(Swatch.Secondary);
            for (int i = 0; i <= spFilled; i++)
                layer.Put(layer.Width - i - 1, yPos, '█');

            Terminal.Color(Swatch.SecondaryDarkest);
            for (int i = spFilled + 1; i <= spWidth; i++)
                layer.Put(layer.Width - i - 1, yPos, '█');

            Terminal.Color(Colors.Text);
            layer.Print(yPos, stamina, System.Drawing.ContentAlignment.TopRight);

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
