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
        private HttpClient httpClient;
        private Uri uri;

        public ReportsDataService(HttpClient httpClient, IOptions<SpaSettings> spaSettingsConfiguration)
        {
            this.uri = new Uri(spaSettingsConfiguration.Value.DataApiAddress);
            this.httpClient = httpClient;
        }

        public async Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterViewModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expenses");
            var result = new List<DataItem>();
            var filterJson = RequestContent.CreateContent(filter);

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<List<DataItem>>();
                result = responseResult;
            }

            return result;
        }

        public async Task<LineChartDataModel> GetExpensesByPeriodPerCategoryAsync(FilterViewModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expensesbycategories");
            var result = new LineChartDataModel();
            var filterJson = RequestContent.CreateContent(filter);

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<LineChartDataModel>();
                result = responseResult;
            }

            return result;
        }

        public async Task<LineChartDataModel> GetExpensesPerMonth(FilterViewModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expensesperiod");
            var result = new LineChartDataModel { Categories = new string[] { }, Series = new List<SeriesItemModel>() };
            var filterJson = RequestContent.CreateContent(filter);

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<LineChartDataModel>();
                result = responseResult;
            }

            return result;
        }

        public async Task<IEnumerable<IdAndValue<double?>>> GetSpentAmountByCategory(DateTime start, DateTime end, int? itemsToTake)
        {
            List<IdAndValue<double?>> result = new List<IdAndValue<double?>>();
            var request = RequestContent.CreateContent(new PageRequestModel{ ItemsPerPage = itemsToTake??0, Filter = new FilterRequestModel { From = start, To = end } });
            var uri = new Uri(this.uri, "api/reports/spentAmountByCategory");
            var response = await this.httpClient.PostAsync(uri, request);
            if (response.IsSuccessStatusCode)
            {
                using (var responseResult = await response.Content.ReadAsStreamAsync())
                {
                    result = await JsonSerializer.DeserializeAsync<List<IdAndValue<double?>>>(
                        responseResult,
                        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                        );
                }
            }

            //TODO: Add logging if response is not valid

            return result.OrderByDescending(e => e.Value);
        }
    }
}
