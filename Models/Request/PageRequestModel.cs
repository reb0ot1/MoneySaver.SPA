namespace MoneySaver.SPA.Models.Request
{
    public class PageRequestModel
    {
        public int ItemsToSkip { get; set; }

        public int ItemsPerPage { get; set; }

        public FilterRequestModel Filter { get; set; }
    }
}
