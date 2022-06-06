using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public interface IReportDataService
    {
        Task<List<DataItem>> GetExpensesPerCategoryAsync(FilterModel filter);

        Task<LineChartDataModel> GetExpensesPerMonth(FilterModel filter);
    }
}
