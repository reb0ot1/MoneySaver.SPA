using MoneySaver.SPA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public interface IBudgetService
    {
        Task<BudgetModel> GetBudgetByTimeType(int intType);

        Task AddBudgetItem(BudgetItemModel budgetItem);

        Task UpdateBudgetItem(BudgetItemModel budgetItem);

        Task RemoveBudgetItem(int id);
    }
}
