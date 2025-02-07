using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Response;

namespace MoneySaver.SPA.Services
{
    public class BudgetService : IBudgetService
    {
        private const string BaseApiPath = "api/budget";
        private readonly IApiCallsService _apiCallsService;

        public BudgetService(IApiCallsService apiCallsService)
        {
            this._apiCallsService = apiCallsService;
        }

        //TODO: Needs to be refactored. Get only the budget in use. The items gathering should be a different method
        public async Task<BudgetViewModel> GetBudgetInUseAsync()
        {
            var budgetInUse = await this._apiCallsService.GetAsync<BudgetResponseModel>($"{BaseApiPath}/inuse");
            
            var budgetModel = new BudgetViewModel
            {
                Id = budgetInUse.Id,
                StartDate = budgetInUse.StartDate,
                EndDate = budgetInUse.EndDate,
                IsInUse = budgetInUse.IsInUse
            };

            return budgetModel;
        }

        public async Task<IEnumerable<BudgetItemModel>> GetBudgetItemsAsync(int budgetId)
        {
            var budgetInUseItems = await _apiCallsService.GetAsync<IEnumerable<BudgetItemResponseModel>>($"{BaseApiPath}/{budgetId}/items");

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
            var budgetRequest = await _apiCallsService.GetAsync<BudgetResponseModel>($"{BaseApiPath}/{budgetId}");

            return budgetRequest;
        }

        public async Task AddBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var response = await this._apiCallsService.PostAsync<BudgetItemModel, BudgetItemModel>($"{BaseApiPath}/{budgetId}/additem", budgetItem);

            //TODO: Return response
        }

        public async Task UpdateBudgetItem(int budgetId, BudgetItemModel budgetItem)
        {
            var result = await this._apiCallsService.PutAsync<BudgetItemModel, BudgetItemModel>($"{BaseApiPath}/{budgetId}/updateitem/{budgetItem.Id}", budgetItem);
            
            //TODO: Return result;
        }

        public Task RemoveBudgetItem(int budgetId, int itemId)
            => this._apiCallsService.DeleteAsync($"{BaseApiPath}/{budgetId}/removeitem/{itemId}");

        public async Task<PageResponseModel<BudgetResponseModel>> GetBudgetsPerPageAsync(int page, int itemsPerPage)
        {
            var response = await _apiCallsService.GetAsync<PageResponseApiModel<BudgetResponseApiModel>>($"{BaseApiPath}/all?pageSize={itemsPerPage}&page={page}");

            return new PageResponseModel<BudgetResponseModel>
            {
                TotalCount = response.TotalCount,
                Result = response.Result.Select(e => new BudgetResponseModel
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

        public async Task<BudgetResponseModel> CreateBudgetAsync(BudgetModel budgetModel)
        {
            var response = await this._apiCallsService.PostAsync<BudgetModel, BudgetResponseModel>($"{BaseApiPath}/add", budgetModel);
            return response;
        }

        public async Task<BudgetResponseModel> UpdateBudgetAsync(BudgetModel budgetModel)
        {
            var response = await this._apiCallsService.PutAsync<BudgetModel, BudgetResponseModel>($"{BaseApiPath}/{budgetModel.Id}", budgetModel);
            return response;
        }
    }
}
