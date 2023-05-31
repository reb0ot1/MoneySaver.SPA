namespace MoneySaver.SPA.Models.Request
{
    public class FilterRequestModel
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string SearchText { get; set; }

        public int[] CategoryIds { get; set; } = new int[] { };
    }
}
