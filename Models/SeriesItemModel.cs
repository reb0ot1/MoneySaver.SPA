namespace MoneySaver.SPA.Models
{
    public class SeriesItemModel
    {
        public string Name { get; set; }

        public double?[] Data { get; set; } = new double?[] { };
    }
}