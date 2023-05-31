using MoneySaver.SPA.Models.Enums;

namespace MoneySaver.SPA.Models
{
    public class BudgetModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public BudgetType BudgetType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsInUse { get; set; }
    }
}
