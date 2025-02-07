using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;

namespace MoneySaver.SPA.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiCallsService _apiCallsService;

        public CategoryService( IApiCallsService apiCallsService)
        {
            this._apiCallsService = apiCallsService;
        }

        public async Task AddCategory(TransactionCategory category)
        {
            var response = await this._apiCallsService.PostAsync<TransactionCategory, TransactionCategory>("api/category", category);
            //TODO: Should return result;
        }

        public async Task<IEnumerable<TransactionCategory>> GetAllAsync()
        {
            var response = await this._apiCallsService.GetAsync<IEnumerable<TransactionCategory>>("api/category");

            return response;
        }

        public async Task<IEnumerable<TransactionCategory>> GetAllPreparedForVisualizationAsync()
        {
            var resultDb = await this.GetAllAsync();
            var parentTransactionCategoryModels = resultDb
                .Where(w => w.ParentId == null);

            foreach (var parentCategory in parentTransactionCategoryModels)
            {
                var children = resultDb
                    .Where(w => w.ParentId == parentCategory.TransactionCategoryId)
                    .ToList();
                
                if (children.Any())
                {
                    foreach (var item in children)
                    {
                        item.AlternativeName = $"{parentCategory.Name}, {item.Name}";
                    }
                }

                parentCategory.AlternativeName = parentCategory.Name;
            }

            return resultDb.OrderBy(e => e.AlternativeName).ToList();
        }

        public async Task UpdateCategory(TransactionCategory category)
        {
            //TODO: Add Http policy
            var response = await this._apiCallsService.PutAsync<TransactionCategory, TransactionCategory>("api/category", category);
            //TODO: Should return result;
        }
    }
}
