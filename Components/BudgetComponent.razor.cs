using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetComponent
    {

        //TODO: Move these constants somewhere else
        public const int levelLow = 20;
        public const int levelMiddle = 60;

        public bool ShowComponent { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public BudgetViewModel BudgetModel { get; set; }

        [Parameter]
        public IEnumerable<TransactionCategory> TransactionCategories { get; set; }

        protected BudgetItemDialog BudgetItemDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (BudgetModel != null)
            {
                await this.UpdateCompoment();
            }
        }

        protected void AddItem()
        {
            this.BudgetItemDialog.Show(CommandType.Add);
        }

        protected async Task CopyBudgetInUseItems()
        {
            var currentlyInUseBudget = await this.BudgetService.GetBudgetInUseItems();

            //TODO: Use bulk request
            foreach (var budgetItem in currentlyInUseBudget.BudgetItems)
            {
                await this.BudgetService.AddBudgetItem(this.BudgetModel.Id, budgetItem);
            }

            await this.UpdateCompoment();

            StateHasChanged();
        }

        protected void EditItem(BudgetItemModel item)
        {
            this.BudgetItemDialog.Show(CommandType.Update, item);
        }

        public async Task Refresh()
        {
            await this.UpdateCompoment();
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
        private static string CheckLevel(int percValue)
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

        private async Task UpdateCompoment()
        {
            var budgetItems = await this.BudgetService.GetBudgetItemsAsync(BudgetModel.Id);

            this.BudgetModel.LimitAmount = budgetItems.Sum(s => s.LimitAmount);
            this.BudgetModel.TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount);
            this.BudgetModel.TotalLeftAmount = this.BudgetModel.LimitAmount - this.BudgetModel.TotalSpentAmmount;
            this.BudgetModel.BudgetItems = budgetItems.ToArray();

            foreach (var item in this.BudgetModel.BudgetItems)
            {
                if (item != null)
                {
                    item.TransactionCategory = this.TransactionCategories
                        .FirstOrDefault(e => e.TransactionCategoryId == item.TransactionCategoryId);
                }
            }
        }
    }
}
