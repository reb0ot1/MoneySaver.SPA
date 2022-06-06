using MoneySaver.SPA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<TransactionCategory>> GetAllAsync();

        Task AddCategory(TransactionCategory category);
        
        Task UpdateCategory(TransactionCategory category);
    }
}
