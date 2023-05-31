namespace MoneySaver.SPA.Models
{
    public class BudgetViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsInUse { get; set; }

        public BudgetItemModel[] BudgetItems { get; set; }

        public double LimitAmount { get; set; }

        public double TotalSpentAmmount { get; set; }

        public double TotalLeftAmount { get; set; }
    }
}
