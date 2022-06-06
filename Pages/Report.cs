using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Filters;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Pages
{
    public partial class Report
    {
        public DataItem[] Data { get; set; }

        //TODO: Needs refactoring
        public LineChartDataModel LineChartData { get; set; }

        public FilterModel Filter { get; set; }

        [Inject]
        public IReportDataService reportDataService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            this.Filter = new FilterModel();
               //TODO: filtration needs refactoring
            var result = this.reportDataService.GetExpensesPerCategoryAsync(this.Filter);
            var resultLineChart = this.reportDataService.GetExpensesPerMonth(this.Filter);
            await Task.WhenAll(result, resultLineChart);
            this.Data = result.Result.ToArray();
            this.LineChartData = resultLineChart.Result;
        }

        protected PieChart PieChart { get; set; }

        protected LineChart LineChart { get; set; }

        protected async Task HandleValidSubmit()
        {
            var resultPieChart = await this.reportDataService.GetExpensesPerCategoryAsync(this.Filter);
            
            this.Data = resultPieChart.ToArray();

            PieChart.Update = true;
        }
    }
}
