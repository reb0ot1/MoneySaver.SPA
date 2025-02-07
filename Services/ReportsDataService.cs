using Microsoft.Extensions.Options;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Filters;
using MoneySaver.SPA.Models.Request;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneySaver.SPA.Services
{
    public class ReportsDataService : IReportDataService
    {
        private readonly IApiCallsService _apiCallsService;
        public ReportsDataService(IApiCallsService apiCallsService)
        {
            this._apiCallsService = apiCallsService;
        }

        public async Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterViewModel filter)
        {
            var response = await this._apiCallsService.PostAsync<FilterViewModel, List<DataItem>>("api/reports/expenses", filter);

            return response;
        }

        public async Task<LineChartDataModel> GetExpensesByPeriodPerCategoryAsync(FilterViewModel filter)
        {
            var response = await this._apiCallsService.PostAsync<FilterViewModel, LineChartDataModel>("api/reports/expensesbycategories", filter);

            return response;
        }

        public async Task<LineChartDataModel> GetExpensesPerMonth(FilterViewModel filter)
        {
            var response = await this._apiCallsService.PostAsync<FilterViewModel, LineChartDataModel>("api/reports/expensesperiod", filter);
            return response;
        }

        public async Task<IEnumerable<IdAndValue<double?>>> GetSpentAmountByCategory(DateTime start, DateTime end, int? itemsToTake)
        {
            var request = new PageRequestModel
            {
                ItemsPerPage = itemsToTake ?? 0,
                Filter = new FilterRequestModel { From = start, To = end }
            };
            
            var response = await this._apiCallsService.PostAsync<PageRequestModel, List<IdAndValue<double?>>>("api/reports/spentAmountByCategory", request);
            
            //TODO: Add logging if response is not valid
            return response.OrderByDescending(e => e.Value);
        }
    }
}
