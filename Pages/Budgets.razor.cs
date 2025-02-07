using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Components;
using MoneySaver.SPA.Exceptions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Response;
using MoneySaver.SPA.Services;
using Radzen;
using Radzen.Blazor;

namespace MoneySaver.SPA.Pages
{
    public partial class Budgets
    {
        private IList<BudgetResponseModel> BudgetsList { get; set; }

        protected const int ItemsPerPage = 10;

        protected bool ShowBudgets = true;

        protected RadzenDataGrid<BudgetResponseModel> grid;

        public int TotalCount { get; set; }

        public int SkipedItems { get; set; }

        public int CurrentPage { get; set; }

        [Inject]
        public ICategoryService CategoryService { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        protected BudgetComponent BudgetComponent { get; set; }

        protected BudgetDialog BudgetDialog { get; set; }

        public BudgetViewModel BudgetModel { get; set; }

        public IEnumerable<TransactionCategory> TransactionCategories = new List<TransactionCategory>();

        protected async override Task OnInitializedAsync()
        {

            await this.ManageGridData();
        }

        public async Task LoadGridData(LoadDataArgs args)
        {
            this.CurrentPage = args.Skip is null ? 1 : ((int)args.Skip / ItemsPerPage) + 1;

            await this.ManageGridData();
        }

        public void CreateBudget()
        {
            BudgetDialog.Show(Models.Enums.CommandType.Add);
        }  

        public async Task OnDialogClose(bool result)
        {
            this.CurrentPage = 1;
            await this.ManageGridData();
            StateHasChanged();
        }

        protected async Task ShowBudgetItem(int budgetId)
        {
            await GetBudgetComponentDataAsync(budgetId);
            StateHasChanged();

            await this.BudgetComponent.Refresh();

            this.ShowBudgets = false;
            this.BudgetComponent.ShowComponent = true;
        }

        protected async Task EditBudget(int budgetId)
        {
            var budget = await this.BudgetService.GetBudgetAsync(budgetId);
            var budgetEntity = new BudgetModel
            {
                Id = budgetId,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                BudgetType= budget.BudgetType,
                Name = budget.Name,
                IsInUse = budget.IsInUse
            };

            BudgetDialog.Show(Models.Enums.CommandType.Update, budgetEntity);
        }

        private async Task ManageGridData()
        {
            var pageResult = await this.BudgetService.GetBudgetsPerPageAsync(this.CurrentPage, ItemsPerPage);
            this.BudgetsList = pageResult.Result.ToList();
            this.TotalCount = pageResult.TotalCount;
        }

        private IEnumerable<TransactionCategory> PrepareForVisualization(IEnumerable<TransactionCategory> categories)
        {
            var result = new List<TransactionCategory>();
            var parentTransactionCategoryModels = categories
                .Where(w => w.ParentId == null);

            foreach (var parentCategory in parentTransactionCategoryModels)
            {
                var children = categories
                    .Where(w => w.ParentId == parentCategory.TransactionCategoryId)
                    .ToList();

                if (children.Any())
                {
                    foreach (var item in children)
                    {
                        item.AlternativeName = $"{parentCategory.Name}, {item.Name}";
                    }
                }

                parentCategory.AlternativeName = parentCategory.Name;
            }

            return categories.OrderBy(e => e.AlternativeName);
        }

        private async Task GetBudgetComponentDataAsync(int budgetId)
        {
            var categories = await CategoryService.GetAllAsync();
            if (categories.Any())
            {
                TransactionCategories = this.PrepareForVisualization(categories);
            }

            var budgetEntity = await this.BudgetService.GetBudgetAsync(budgetId);

            BudgetModel = new BudgetViewModel {
                Id = budgetId,
                StartDate = budgetEntity.StartDate,
                EndDate = budgetEntity.EndDate
            };
        }
    }
}
