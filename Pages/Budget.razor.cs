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

        public BudgetModel BudgetModel { get; set; }

        public IEnumerable<TransactionCategory> TransactionCategories = new List<TransactionCategory>();

        protected async override Task OnInitializedAsync()
        {
            //TODO: Get budget type set in the appconfiguration
            await this.UpdateCompoment();
        }

        protected BudgetItemDialog BudgetItemDialog { get; set; }

        protected void AddItem()
        {
            this.BudgetItemDialog.Show();
        }

        protected void EditItem(BudgetItemModel item)
        {
            this.BudgetItemDialog.Show(item);
        }

        public async void AddItem_OnDialogClose()
        {
            await this.UpdateCompoment();

            StateHasChanged();
        }

        public async void RemoveItem(int id)
        {
            await this.BudgetService.RemoveBudgetItem(this.BudgetModel.Id, id);
            await this.UpdateCompoment();
            StateHasChanged();
        }

        //TODO: This method for customize visualization should be moved somewhere else
        public static string CheckLevel(int percValue)
        {
            if (levelLow < percValue && percValue <= levelMiddle)
            {
                return "bg-warning";
            }

            if (percValue <= levelLow)
            {
                return "bg-danger";
            }

            return "bg-success";
        }

        public static string CheckColor(double value)
        {
            if (value < 0)
            {
                return "red";
            }

            return "#000";
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

        private async Task UpdateCompoment()
        {
            var categories = await CategoryService.GetAllAsync();
            if (!categories.Any())
            {
                return;
            }

            TransactionCategories = this.PrepareForVisualization(categories);

            var budgetEntity = await BudgetService.GetBudgetInUseItems();
            foreach (var item in budgetEntity.BudgetItems)
            {
                if (item != null)
                {
                    item.TransactionCategory = this.TransactionCategories
                        .FirstOrDefault(e => e.TransactionCategoryId == item.TransactionCategoryId);
                }
            }

            budgetEntity.BudgetItems = budgetEntity
                                        .BudgetItems
                                        .OrderBy(o => o.TransactionCategory.AlternativeName)
                                        .ToArray();

            BudgetModel = budgetEntity;
        }
    }
}
