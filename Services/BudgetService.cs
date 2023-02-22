using Microsoft.Extensions.Options;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Response;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MoneySaver.SPA.Services
{
    public class BudgetService : IBudgetService
    {
        private HttpClient httpClient;
        private Uri uri;
        private const string BaseApiPath = "api/budget";

        public BudgetService(HttpClient httpClient, IOptions<SpaSettings> spaSettingsConfiguration)
        {
            this.uri = new Uri(spaSettingsConfiguration.Value.DataApiAddress);
            this.httpClient = httpClient;
        }
        public async Task<BudgetModel> GetBudgetInUseItems()
        {
            var budgetInUserRequest = await httpClient.GetFromJsonAsync<BudgetResponseModel>(new Uri(this.uri, $"{BaseApiPath}/inuse"));
            if (budgetInUserRequest == null)
            {
                //TODO: Use model for returning results
                return null;
            }

            var uri = new Uri(this.uri, $"{BaseApiPath}/{budgetInUserRequest.Id}/items");
            var budgetInUseItems = await httpClient.GetFromJsonAsync<IEnumerable<BudgetItemResponseModel>>(uri);

            var budgetModel = new BudgetModel
            {
                Id = budgetInUserRequest.Id,
                BudgetItems = budgetInUseItems.Select(entity => new BudgetItemModel { 
                    Id = entity.Id,
                    BudgetId = budgetInUserRequest.Id,
                    LimitAmount= entity.LimitAmount,
                    Progress = entity.Progress,
                    SpentAmount= entity.SpentAmount,
                    TransactionCategoryId = entity.TransactionCategoryId,
                    TransactionCategoryName = entity.TransactionCategoryName
                }).OrderBy(o => o.TransactionCategoryName).ToArray()
            };

            budgetModel.StartDate = budgetInUserRequest.StartDate;
            budgetModel.EndDate = budgetInUserRequest.EndDate;
            budgetModel.LimitAmount = budgetModel.BudgetItems.Sum(s => s.LimitAmount);
            budgetModel.TotalSpentAmmount = budgetModel.BudgetItems.Sum(s => s.SpentAmount);
            budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            return budgetModel;
        }

        public async Task AddBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var budgetItemJson = new StringContent(
                JsonSerializer.Serialize(budgetItem),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(new Uri(this.uri, $"{BaseApiPath}/{budgetId}/additem"), budgetItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<BudgetItemModel>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task UpdateBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var budgetItemJson = new StringContent(
                JsonSerializer.Serialize(budgetItem),
                Encoding.UTF8, "application/json"
                );

            await this.httpClient.PutAsync(new Uri(this.uri, $"{BaseApiPath}/{budgetId}/updateitem/{budgetItem.Id}"), budgetItemJson);
        }

        public async Task RemoveBudgetItem(int budgetId, int itemId)
        {
            var uri = new Uri(this.uri, $"{BaseApiPath}/{budgetId}/removeitem/{itemId}");
            await this.httpClient.DeleteAsync(uri);
        }
    }
}
