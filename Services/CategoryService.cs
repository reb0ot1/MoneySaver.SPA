using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public class CategoryService : ICategoryService
    {
        private HttpClient httpClient;
        private Uri uri;

        public CategoryService(HttpClient httpClient, DataApi dataApi)
        {
            this.uri = new Uri(dataApi.Url);
            this.httpClient = httpClient;
        }

        public async Task AddCategory(TransactionCategory category)
        {
            var categoryItemJson = new StringContent(
               JsonSerializer.Serialize(category),
               Encoding.UTF8, "application/json"
               );

            var response = await this.httpClient.PostAsync(new Uri(this.uri, "api/category"), categoryItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<TransactionCategory>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task<IEnumerable<TransactionCategory>> GetAllAsync()
        {
            IEnumerable<TransactionCategory> result = new List<TransactionCategory>();
            result = await httpClient.GetFromJsonAsync<IEnumerable<TransactionCategory>>(new Uri(this.uri, "api/category"));

            return result;
        }

        public async Task UpdateCategory(TransactionCategory category)
        {
            var categoryItemJson = new StringContent(
               JsonSerializer.Serialize(category),
               Encoding.UTF8, "application/json"
               );

            var response = await this.httpClient.PutAsync(new Uri(this.uri, "api/category"), categoryItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<TransactionCategory>(await response.Content.ReadAsStreamAsync());
            }
        }
    }
}
