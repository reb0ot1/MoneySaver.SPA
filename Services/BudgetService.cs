using Microsoft.Extensions.Options;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Response;
using System.Net.Http.Json;
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


        //TODO: Needs to be refactored. Get only the budget in use. The items gathering should be a different method
        public async Task<BudgetViewModel> GetBudgetInUseAsync()
        {
            var budgetInUserRequest = await httpClient.GetFromJsonAsync<BudgetResponseModel>(new Uri(this.uri, $"{BaseApiPath}/inuse"));
            if (budgetInUserRequest == null)
            {
                //TODO: Use model for returning results
                return null;
            }

            //var uri = new Uri(this.uri, $"{BaseApiPath}/{budgetInUserRequest.Id}/items");
            //var budgetInUseItems = await httpClient.GetFromJsonAsync<IEnumerable<BudgetItemResponseModel>>(uri);

            var budgetModel = new BudgetViewModel
            {
                Id = budgetInUserRequest.Id,
                StartDate = budgetInUserRequest.StartDate,
                EndDate = budgetInUserRequest.EndDate,
                IsInUse = budgetInUserRequest.IsInUse
                //BudgetItems = budgetInUseItems.Select(entity => new BudgetItemModel { 
                //    Id = entity.Id,
                //    BudgetId = budgetInUserRequest.Id,
                //    LimitAmount= entity.LimitAmount,
                //    Progress = entity.Progress,
                //    SpentAmount= entity.SpentAmount,
                //    TransactionCategoryId = entity.TransactionCategoryId,
                //    TransactionCategoryName = entity.TransactionCategoryName
                //}).OrderBy(o => o.TransactionCategoryName).ToArray()
            };

            //budgetModel.LimitAmount = budgetModel.BudgetItems.Sum(s => s.LimitAmount);
            //budgetModel.TotalSpentAmmount = budgetModel.BudgetItems.Sum(s => s.SpentAmount);
            //budgetModel.TotalLeftAmount = budgetModel.LimitAmount - budgetModel.TotalSpentAmmount;

            return budgetModel;
        }

        public async Task<IEnumerable<BudgetItemModel>> GetBudgetItemsAsync(int budgetId)
        {
            var uri = new Uri(this.uri, $"{BaseApiPath}/{budgetId}/items");
            var budgetInUseItems = await httpClient.GetFromJsonAsync<IEnumerable<BudgetItemResponseModel>>(uri);

            if (budgetInUseItems is null)
            {
                return new List<BudgetItemModel>();
            }

            return budgetInUseItems.Select(entity => new BudgetItemModel
            {
                Id = entity.Id,
                BudgetId = budgetId,
                LimitAmount = entity.LimitAmount,
                Progress = entity.Progress,
                SpentAmount = entity.SpentAmount,
                TransactionCategoryId = entity.TransactionCategoryId,
                TransactionCategoryName = entity.TransactionCategoryName
            });
        }

        public async Task<BudgetResponseModel> GetBudgetAsync(int budgetId)
        { 
            var budgetRequest = await httpClient.GetFromJsonAsync<BudgetResponseModel>(new Uri(this.uri, $"{BaseApiPath}/{budgetId}"));
            if (budgetRequest == null)
            {
                //TODO: Use model for returning results
                return null;
            }

            return budgetRequest;
        }

        public async Task AddBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var budgetItemJson = RequestContent.CreateContent(budgetItem);

            var response = await this.httpClient.PostAsync(new Uri(this.uri, $"{BaseApiPath}/{budgetId}/additem"), budgetItemJson);
            if (response.IsSuccessStatusCode)
            {
                await JsonSerializer.DeserializeAsync<BudgetItemModel>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task UpdateBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var budgetItemJson = RequestContent.CreateContent(budgetItem);

            await this.httpClient.PutAsync(new Uri(this.uri, $"{BaseApiPath}/{budgetId}/updateitem/{budgetItem.Id}"), budgetItemJson);
        }

        public async Task RemoveBudgetItem(int budgetId, int itemId)
        {
            var uri = new Uri(this.uri, $"{BaseApiPath}/{budgetId}/removeitem/{itemId}");
            var response = await this.httpClient.DeleteAsync(uri);
        }

        public async Task<PageResponseModel<BudgetResponseModel>> GetBudgetsPerPageAsync(int page, int itemsPerPage)
        {
            var uri = new Uri(this.uri, $"{BaseApiPath}/all?pageSize={itemsPerPage}&page={page}");
            var response = await httpClient.GetAsync(uri);
            PageResponseApiModel<BudgetResponseApiModel> result = null;
            if (response.IsSuccessStatusCode)
            {
                using var responseResult = await response.Content.ReadAsStreamAsync();

                result = await JsonSerializer.DeserializeAsync<PageResponseApiModel<BudgetResponseApiModel>>(responseResult);

                return new PageResponseModel<BudgetResponseModel>
                {
                    TotalCount = result.TotalCount,
                    Result = result.Result.Select(e => new BudgetResponseModel
                    {
                        Id = e.Id,
                        EndDate = e.EndDate,
                        StartDate = e.StartDate,
                        Name = e.Name,
                        BudgetType = e.BudgetType,
                        IsInUse = e.IsInUse
                    })
                    .ToList()
                };
            }

            //TODO: Should use Result class
            return new PageResponseModel<BudgetResponseModel>();
        }

        public async Task<BudgetResponseModel> CreateBudgetAsync(BudgetModel budgetModel)
        {
            var budgetItemJson = RequestContent.CreateContent(budgetModel);
            var response = await this.httpClient.PostAsync(new Uri(this.uri, $"{BaseApiPath}/add"), budgetItemJson);

            if (response.IsSuccessStatusCode)
            {
                using var responseResult = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<BudgetResponseModel>(responseResult);

                return result;
            }

            return null;
        }

        public async Task<BudgetResponseModel> UpdateBudgetAsync(BudgetModel budgetModel)
        {
            var budgetItemJson = RequestContent.CreateContent(budgetModel);
            var response = await this.httpClient.PutAsync(new Uri(this.uri, $"{BaseApiPath}/{budgetModel.Id}"), budgetItemJson);

            if (response.IsSuccessStatusCode)
            {
                using var responseResult = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<BudgetResponseModel>(responseResult);

                return result;
            }

            return null;
        }
    }
}
