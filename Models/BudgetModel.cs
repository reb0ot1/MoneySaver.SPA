using System;
using System.Collections.Generic;
using System.Text;

namespace MoneySaver.SPA.Models
{
    public class BudgetModel
    {
        public int Id { get; set; }

        //public BudgetType Type { get; set; }

        public BudgetItemModel[] BudgetItems { get; set; }

        public double LimitAmount { get; set; }

        public double TotalSpentAmmount { get; set; }

        public double TotalLeftAmount { get; set; }
    }
}
