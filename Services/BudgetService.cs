using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public class BudgetService : IBudgetService
    {
        private HttpClient httpClient;
        private Uri uri;
        private const string BaseApiPath = "api/budget";
        public BudgetService(HttpClient httpClient, DataApi dataApi)
        {
            this.uri = new Uri(dataApi.Url);
            this.httpClient = httpClient;
        }
        public async Task<BudgetModel> GetBudgetByTimeType(int intType)
        {
            BudgetModel result = null;
            var uri = new Uri(this.uri, $"{BaseApiPath}/items");
            result = await httpClient.GetFromJsonAsync<BudgetModel>(uri);

            return result;
        }

        public async Task AddBudgetItem(BudgetItemModel budgetItem)
        {
            var budgetItemJson = new StringContent(
                JsonSerializer.Serialize(budgetItem),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(new Uri(this.uri, $"{BaseApiPath}/additem"), budgetItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<BudgetItemModel>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task UpdateBudgetItem(BudgetItemModel budgetItem)
        {
            var budgetItemJson = new StringContent(
                JsonSerializer.Serialize(budgetItem),
                Encoding.UTF8, "application/json"
                );

            await this.httpClient.PutAsync(new Uri(this.uri, $"{BaseApiPath}/updateitem"), budgetItemJson);
        }

        public async Task RemoveBudgetItem(int id)
        {
            var uri = new Uri(this.uri, $"{BaseApiPath}/removeitem/{id}");
            await this.httpClient.DeleteAsync(uri);
        }
    }
}
