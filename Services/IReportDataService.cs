using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Models.Filters;

namespace MoneySaver.SPA.Services
{
    public interface IReportDataService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterViewModel filter);

        Task<LineChartDataModel> GetExpensesPerMonth(FilterViewModel filter);

        Task<LineChartDataModel> GetExpensesByPeriodPerCategoryAsync(FilterViewModel filter);

        Task<IEnumerable<IdAndValue<double?>>> GetSpentAmountByCategory(DateTime start, DateTime end, int? itemsToTake);
    }
}
