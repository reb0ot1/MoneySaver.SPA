using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Exceptions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetComponent
    {
        public bool ShowComponent { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public BudgetViewModel BudgetModel { get; set; }

        [Parameter]
        public IEnumerable<TransactionCategory> TransactionCategories { get; set; }

        protected BudgetItemDialog BudgetItemDialog { get; set; }

        public BudgetViewModel BudgetComponentModel { get; set; }

        protected async override Task OnInitializedAsync()
        {
            if (this.BudgetModel is null || this.BudgetModel.Id == 0)
            {
                return;
            }
            
            this.BudgetComponentModel = new BudgetViewModel
            {
                Id = this.BudgetModel.Id,
                StartDate = this.BudgetModel.StartDate,
                EndDate = this.BudgetModel.EndDate,
                IsInUse = this.BudgetModel.IsInUse
            };
            
            await this.UpdateCompoment();
        }

        protected void AddItem()
        {
            this.BudgetItemDialog.Show(CommandType.Add);
            StateHasChanged();
        }

        protected async Task CopyBudgetInUseItems()
        {
            var currentlyInUseBudget = await this.BudgetService.GetBudgetInUseAsync();
            var currentlyInUseBudgetItems = await this.BudgetService.GetBudgetItemsAsync(currentlyInUseBudget.Data.Id);
        
            //TODO: Use bulk request
            foreach (var budgetItem in currentlyInUseBudgetItems)
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
            switch (percValue)
            {
                case <= Constants.BUDGET_ITEM_LOW_LEVEL:
                    return "bg-danger";
                case <= Constants.BUDGET_ITEM_MIDDLE_LEVEL:
                    return "bg-warning";
                default:
                    return "bg-success";
            }
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
            
            if (this.BudgetComponentModel != null)
            {
                this.BudgetComponentModel.LimitAmount = budgetItems.Sum(s => s.LimitAmount);
                this.BudgetComponentModel.TotalSpentAmmount = budgetItems.Sum(s => s.SpentAmount);
                this.BudgetComponentModel.TotalLeftAmount = this.BudgetComponentModel.LimitAmount - this.BudgetComponentModel.TotalSpentAmmount;
                var testItems = budgetItems.ToList();
                foreach (var item in testItems)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    var categoryToFind = this.TransactionCategories
                        .FirstOrDefault(e => e.TransactionCategoryId == item.TransactionCategoryId);


                    item.TransactionCategory = new TransactionCategory { 
                        AlternativeName = categoryToFind.AlternativeName,
                        Name = categoryToFind.Name,
                        TransactionCategoryId = categoryToFind.TransactionCategoryId,
                        ParentId = categoryToFind.ParentId
                    };
                }

                BudgetComponentModel.BudgetItems = testItems
                    .OrderBy(e => e.TransactionCategory?.AlternativeName)
                    .ToArray();    
            }
        }
    }
}
