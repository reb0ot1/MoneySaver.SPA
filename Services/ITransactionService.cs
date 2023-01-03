using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public interface ITransactionService
    {
        Task<TransactionsPageModel> GetForPage(int pageNumber, int itemsPerPage, string search);
        Task<Transaction> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(string transactionId);
    }
}
