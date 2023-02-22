using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetItemDialog
    {
        private bool forUpdate = false;

        private BudgetItemModel OriginalBudgetItem = null;

        public BudgetItemModel BudgetItemModel { get; set; }

        [Inject]
        public IBudgetService BudgetService { get; set; }

        [Parameter]
        public TransactionCategory[] ТransactionCategories { get; set; }

        [Parameter]
        public int BudgetId { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public string CategoryId { get; set; }

        public bool ShowDialog { get; set; } = false;

        protected async Task HandleValidSubmit()
        {

            //TODO: Find a better way to check if the request is for update or for add
            this.BudgetItemModel.TransactionCategoryId = int.Parse(CategoryId);
            if (this.forUpdate)
            {
                await this.BudgetService.UpdateBudgetItem(this.BudgetId, this.BudgetItemModel);
            }
            else
            {
                await this.BudgetService.AddBudgetItem(this.BudgetId, this.BudgetItemModel);
            }

            ShowDialog = false;
            await CloseEventCallback.InvokeAsync(true);
        }

        public void Show(BudgetItemModel model = null)
        {
            if (model != null)
            {
                this.CategoryId = model.TransactionCategoryId.ToString();
                this.BudgetItemModel = model;
                this.forUpdate = true;
            }
            else
            {
                var defaultCategory = this.ТransactionCategories.First();
                this.BudgetItemModel = new BudgetItemModel
                {
                    TransactionCategoryId = (int)defaultCategory.TransactionCategoryId,
                    LimitAmount = 0
                };
            }

            this.OriginalBudgetItem = new BudgetItemModel
            {
                TransactionCategoryId = this.BudgetItemModel.TransactionCategoryId,
                LimitAmount = this.BudgetItemModel.LimitAmount
            };

            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            this.BudgetItemModel.TransactionCategoryId = this.OriginalBudgetItem.TransactionCategoryId;
            this.BudgetItemModel.LimitAmount = this.OriginalBudgetItem.LimitAmount;
            this.CategoryId = this.OriginalBudgetItem.TransactionCategoryId.ToString();
            StateHasChanged();
            this.ShowDialog = false;
        }

        private void ResetDialog()
        {
            var defaultCategory = this.ТransactionCategories.First();
            this.forUpdate = false;
            this.BudgetItemModel.TransactionCategoryId = (int)defaultCategory.TransactionCategoryId;
            this.BudgetItemModel.LimitAmount = 0;
            this.CategoryId = defaultCategory.TransactionCategoryId.ToString();
        }
    }
}
