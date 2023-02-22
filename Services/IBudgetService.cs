using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public interface IBudgetService
    {
        Task<BudgetModel> GetBudgetInUseItems();

        Task AddBudgetItem(int budgetId, BudgetItemModel budgetItem);

        Task UpdateBudgetItem(int budgetId, BudgetItemModel budgetItem);

        Task RemoveBudgetItem(int budgetId, int itemId);
    }
}