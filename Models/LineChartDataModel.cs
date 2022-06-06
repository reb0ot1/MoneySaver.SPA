namespace MoneySaver.SPA.Models
{
    public class LineChartDataModel
    {
        public string[] Categories { get; set; } = new string[] { };

        public List<SeriesItemModel> Series { get; set; } = new List<SeriesItemModel>();
    }
}
