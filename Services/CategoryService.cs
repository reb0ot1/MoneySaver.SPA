using Microsoft.Extensions.Options;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MoneySaver.SPA.Services
{
    public class CategoryService : ICategoryService
    {
        private HttpClient httpClient;
        private Uri uri;

        public CategoryService(HttpClient httpClient, IOptions<SpaSettings> spaSettingConfiguration)
        {
            this.uri = new Uri(spaSettingConfiguration.Value.DataApiAddress);
            this.httpClient = httpClient;
        }

        public async Task AddCategory(TransactionCategory category)
        {
            var categoryItemJson = new StringContent(
               JsonSerializer.Serialize(category),
               Encoding.UTF8, "application/json"
               );

            //TODO: Add Http policy
            var response = await this.httpClient.PostAsync(new Uri(this.uri, "api/category"), categoryItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<TransactionCategory>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task<IEnumerable<TransactionCategory>> GetAllAsync()
        {
            //TODO: Add Http policy
            IEnumerable<TransactionCategory> result = new List<TransactionCategory>();
            result = await httpClient.GetFromJsonAsync<IEnumerable<TransactionCategory>>(new Uri(this.uri, "api/category"));

            return result;
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
            var categoryItemJson = new StringContent(
               JsonSerializer.Serialize(category),
               Encoding.UTF8, "application/json"
               );
            //TODO: Add Http policy
            var response = await this.httpClient.PutAsync(new Uri(this.uri, "api/category"), categoryItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<TransactionCategory>(await response.Content.ReadAsStreamAsync());
            }
        }
    }
}
