using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Response;

namespace MoneySaver.SPA.Services
{
    public interface IBudgetService
    {
        Task<PageResponseModel<BudgetResponseModel>> GetBudgetsPerPageAsync(int itemsToSkip, int itemsPerPage);

        Task<BudgetViewModel> GetBudgetInUseItems();

        Task<BudgetResponseModel> CreateBudgetAsync(BudgetModel budgetModel);

        Task<BudgetResponseModel> GetBudgetAsync(int id);

        Task<IEnumerable<BudgetItemModel>> GetBudgetItemsAsync(int budgetId);

        Task AddBudgetItem(int budgetId, BudgetItemModel budgetItem);

        Task UpdateBudgetItem(int budgetId, BudgetItemModel budgetItem);

        Task RemoveBudgetItem(int budgetId, int itemId);

        Task<BudgetResponseModel> UpdateBudgetAsync(BudgetModel budgetModel);
    }
}