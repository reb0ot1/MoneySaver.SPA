using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<TransactionCategory>> GetAllAsync();

        Task<IEnumerable<TransactionCategory>> GetAllPreparedForVisualizationAsync();

        Task AddCategory(TransactionCategory category);
        
        Task UpdateCategory(TransactionCategory category);
    }
}
