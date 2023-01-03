using Microsoft.Extensions.Options;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Filters;
using System.Net.Http.Json;
using System.Text;
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

        public async Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expenses");
            var result = new List<DataItem>();
            var filterJson = new StringContent(
                JsonSerializer.Serialize(filter),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<List<DataItem>>();
                result = responseResult;
            }

            return result;
        }

        public async Task<LineChartDataModel> GetExpensesByPeriodPerCategoryAsync(FilterModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expensesbycategories");
            var result = new LineChartDataModel();
            var filterJson = new StringContent(
                JsonSerializer.Serialize(filter),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<LineChartDataModel>();
                result = responseResult;
            }

            return result;
        }

        public async Task<LineChartDataModel> GetExpensesPerMonth(FilterModel filter)
        {
            var uri = new Uri(this.uri, "api/reports/expensesperiod");
            var result = new LineChartDataModel { Categories = new string[] { }, Series = new List<SeriesItemModel>() };
            var filterJson = new StringContent(
                JsonSerializer.Serialize(filter),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(uri, filterJson);

            if (response.IsSuccessStatusCode)
            {
                var responseResult = await response.Content.ReadFromJsonAsync<LineChartDataModel>();
                result = responseResult;
            }

            return result;
        }
    }
}
