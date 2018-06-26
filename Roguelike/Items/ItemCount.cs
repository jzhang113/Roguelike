namespace Roguelike.Items
{
    public class ItemCount
    {
        public Item Item { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return Count == 1 ? $"a {Item}" : $"{Count} {Item.ToString().Pluralize()}";
        }
    }
}