using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;

namespace MoneySaver.SPA.Services
{
    public interface ITransactionService
    {
        Task<TransactionsPageModel> GetForPage(int pageNumber, int itemsPerPage, string search);
        Task<Transaction> AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> AddManyAsync(IEnumerable<Transaction> transactions);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(string transactionId);
        Task<IEnumerable<IdAndValue<double?>>> GetSpentAmountByCategory(BudgetType budgetType, int? itemsToTake);
    }
}
