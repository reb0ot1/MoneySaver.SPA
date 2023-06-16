using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Pages
{
    public partial class Budget
    {
        //TODO: Move these constants somewhere else
        public const int levelLow = 20;
        public const int levelMiddle = 60;

        [Inject]
        public ICategoryService CategoryService { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public int Id { get; set; }

        protected BudgetComponent BudgetComponent { get; set; }

        public BudgetViewModel BudgetModel { get; set; }

        public IEnumerable<TransactionCategory> TransactionCategories = new List<TransactionCategory>();

        protected async override Task OnInitializedAsync()
        {
            var categories = await CategoryService.GetAllAsync();
            if (categories.Any())
            {
                TransactionCategories = this.PrepareForVisualization(categories);
            }

            var budgetEntity = await BudgetService.GetBudgetInUseAsync();
           
            BudgetModel = budgetEntity;

            StateHasChanged();

            BudgetComponent.ShowComponent = true;
        }

        //TODO: The method bellow needs to be declare once, because it`s used by other pages.
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
    }
}
