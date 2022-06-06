using System;

namespace MoneySaver.SPA.Models
{
    public class BudgetItemModel
    {
        public BudgetItemModel()
        {
        }

        public int Id { get; set; }

        public int BudgetId { get; set; }

        public int TransactionCategoryId { get; set; }

        public TransactionCategory TransactionCategory { get; set; }

        public double LimitAmount { get; set; } = 0;

        public double SpentAmount { get; set; } = 0;

        public int Progress { get; set; }
    }
}
