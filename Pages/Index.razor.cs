using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.ViewModels;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Pages
{
    public partial class Index
    {
        //TODO: The number of spent amount by categories to be refactored
        private const int CountLastTransactions = 10;
        private const int TopCategoriesSpentAmount = 10;

        private string Name = string.Empty;

        private BudgetViewModel BudgetModel { get; set; }

        [Inject]
        private IBudgetService budgetService { get; set; }

        [Inject]
        private ICategoryService categoryService { get; set; }

        [Inject]
        private ITransactionService transactionService { get; set; }

        [Inject]
        private IReportDataService reportDataService { get; set; }

        [Inject]
        public AuthenticationStateProvider StateProv { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<TransactionRepresentationModel> Transactions { get; set; } = new List<TransactionRepresentationModel>();

        public IEnumerable<CategoryAmountSpentModel> CategoriesSpentAmount { get; set; } = new List<CategoryAmountSpentModel>();

        protected async override Task OnInitializedAsync()
        {
            var budgetTask = this.budgetService.GetBudgetInUseAsync();
            var categoriesTask = this.categoryService.GetAllPreparedForVisualizationAsync();

            await Task.WhenAll(budgetTask, categoriesTask);

            var currentBudget = budgetTask.Result;

            var budgetItems = await this.budgetService.GetBudgetItemsAsync(currentBudget.Id);
            currentBudget.BudgetItems = budgetItems.ToArray();

            this.BudgetModel = currentBudget;
            this.StartDate = currentBudget.StartDate;
            this.EndDate = currentBudget.EndDate;
            var budgetCategoryIds = currentBudget.BudgetItems.Select(e => e.TransactionCategoryId);
            var categories = categoriesTask.Result;
            var categoriesBasedOnBudget = categories.Where(w => budgetCategoryIds.Contains(w.TransactionCategoryId)).ToList();

            this.InitializeCategories(categoriesBasedOnBudget);
            this.InitializeTransactions(categories);
        }

        private async Task InitializeTransactions(IEnumerable<TransactionCategory> categories)
        {
            TransactionsPageModel resultTransactions = await this.transactionService.GetForPage(0, CountLastTransactions, null);
            var transactions = resultTransactions.Result ?? new List<Transaction>();
            var result = new List<TransactionRepresentationModel>();
            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    var category = categories.FirstOrDefault(e => e.TransactionCategoryId == transaction.TransactionCategoryId);
                    if (category == null)
                    {
                        continue;
                    }

                    result.Add(new TransactionRepresentationModel
                    {
                        //TODO: Remove if not needed
                        Id = transaction.Id.ToString(),
                        DateTime = transaction.TransactionDate,
                        Amount = transaction.Amount,
                        CategoryName = category.AlternativeName
                    });
                }
            }
            this.Transactions = result;

            StateHasChanged();
        }

        private async Task InitializeCategories(IEnumerable<TransactionCategory> categories)
        {
            var spentAmountByCategories = await this.reportDataService.GetSpentAmountByCategory(
                this.BudgetModel.StartDate, 
                this.BudgetModel.EndDate,
                TopCategoriesSpentAmount
                );

            var categoriesResult = new List<CategoryAmountSpentModel>();
            foreach (var category in spentAmountByCategories)
            {
                var categoryEntity = categories.FirstOrDefault(e => e.TransactionCategoryId == category.Id);
                if (categoryEntity != null)
                {
                    categoriesResult.Add(new CategoryAmountSpentModel { Id = category.Id, Amount = category.Value, Name = categoryEntity.AlternativeName });
                }
            }

            this.CategoriesSpentAmount = categoriesResult;

            StateHasChanged();
        }
    }
}
