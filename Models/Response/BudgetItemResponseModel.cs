namespace MoneySaver.SPA.Models.Response
{
    public class BudgetItemResponseModel
    {
        public int Id { get; set; }

        public int TransactionCategoryId { get; set; }

        public string TransactionCategoryName { get; set; }

        public double LimitAmount { get; set; }

        public double SpentAmount { get; set; }

        public int Progress { get; set; }
    }
}
