using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetItemDialog
    {
        private bool forUpdate = false;
        public BudgetItemModel BudgetItemModel { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public TransactionCategory[] ТransactionCategories { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public string CategoryId { get; set; }

        public bool ShowDialog { get; set; } = false;

        protected async Task HandleValidSubmit()
        {
            this.BudgetItemModel.TransactionCategoryId = int.Parse(CategoryId);
            if (this.forUpdate)
            {
                await this.BudgetService.UpdateBudgetItem(this.BudgetItemModel);
            }
            else
            {
                await this.BudgetService.AddBudgetItem(this.BudgetItemModel);
            }

            ShowDialog = false;
            await CloseEventCallback.InvokeAsync(true);
            //StateHasChanged();
        }

        public void Show(BudgetItemModel model = null)
        {
            ResetDialog();
            if (model != null)
            {
                CategoryId = model.TransactionCategoryId.ToString();
                this.BudgetItemModel = model;
                this.forUpdate = true;
            }

            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ResetDialog();
            this.ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            var defaultCategory = this.ТransactionCategories.First();
            this.forUpdate = false;
            this.BudgetItemModel = new BudgetItemModel
            {
                TransactionCategoryId = (int)defaultCategory.TransactionCategoryId,
                LimitAmount = 0
            };
            this.CategoryId = defaultCategory.TransactionCategoryId.ToString();
        }
    }
}
