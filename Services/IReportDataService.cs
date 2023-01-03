using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Filters;

namespace MoneySaver.SPA.Services
{
    public interface IReportDataService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter);

        Task<LineChartDataModel> GetExpensesPerMonth(FilterModel filter);

        Task<LineChartDataModel> GetExpensesByPeriodPerCategoryAsync(FilterModel filter);
    }
}
