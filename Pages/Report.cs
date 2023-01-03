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
        public IEnumerable<int> CategoryIdsSelected { get; set; } = new int[] {};

        public IEnumerable<TransactionCategory> CategoryRecords { get; set; }

        //TODO: Needs refactoring
        public LineChartDataModel LineChartData { get; set; }

        public LineChartDataModel ExpensesByPeriodForCategoriesChartData { get; set; }

        public FilterModel Filter { get; set; }

        [Inject]
        public IReportDataService reportDataService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //TODO: filtration needs refactoring
            this.Filter = new FilterModel();
            var categories = await this.categoryService.GetAllPreparedForVisualizationAsync();
            this.CategoryRecords = categories;
            this.CategoryIdsSelected = this.CategoryRecords.Select(e => e.TransactionCategoryId).ToList();

            this.Filter.CategoryIds = this.CategoryIdsSelected.ToArray();

            await LoadResults();
        }
            
        protected PieChart PieChart { get; set; }

        protected LineChart LineChart { get; set; }

        protected LineChart LineChartForExpesesByPeriodForCategory { get; set; }

        protected async Task HandleValidSubmit()
        {
            //TODO: Get only categories available for the current time
            this.Filter.CategoryIds = this.CategoryIdsSelected.ToArray();
            await this.LoadResults();
            this.PieChart.Update = this.LineChart.Update = this.LineChartForExpesesByPeriodForCategory.Update = true;
            await InvokeAsync(StateHasChanged);
        }

        public async Task OnCategoryDropDownChange()
        {
            this.Filter.CategoryIds = this.CategoryIdsSelected?.ToArray();
            var resultLineChartExpensesByCategory = await this.reportDataService.GetExpensesByPeriodPerCategoryAsync(this.Filter);
            this.ExpensesByPeriodForCategoriesChartData = resultLineChartExpensesByCategory;

            this.LineChartForExpesesByPeriodForCategory.Update = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task LoadResults()
        {
            var result = this.reportDataService.GetExpensesPerCategoryAsync(this.Filter);
            var resultLineChart = this.reportDataService.GetExpensesPerMonth(this.Filter);
            var resultLineChartExpensesByCategory = this.reportDataService.GetExpensesByPeriodPerCategoryAsync(this.Filter);

            await Task.WhenAll(result, resultLineChart, resultLineChartExpensesByCategory);
            this.Data = result.Result.ToArray();
            this.LineChartData = resultLineChart.Result;
            this.ExpensesByPeriodForCategoriesChartData = resultLineChartExpensesByCategory.Result;
        }
    }
}
