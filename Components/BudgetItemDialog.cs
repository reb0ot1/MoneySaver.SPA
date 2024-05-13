using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Enums;
using MoneySaver.SPA.Services;

namespace MoneySaver.SPA.Components
{
    public partial class BudgetItemDialog
    {
        private CommandType? command;

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
            this.BudgetItemModel.TransactionCategoryId = int.Parse(CategoryId);
            if (this.command == CommandType.Update)
            {
                await this.BudgetService.UpdateBudgetItem(this.BudgetId, this.BudgetItemModel);
            }

            if (this.command == CommandType.Add)
            {
                await this.BudgetService.AddBudgetItem(this.BudgetId, this.BudgetItemModel);
            }

            ShowDialog = false;
            await CloseEventCallback.InvokeAsync(true);
        }

        public void Show(CommandType command, BudgetItemModel model = null)
        {
            switch (command)
            {
                case CommandType.Add:
                    var defaultCategory = this.ТransactionCategories.First();
                    this.CategoryId = defaultCategory.TransactionCategoryId.ToString();
                    this.BudgetItemModel = new BudgetItemModel
                    {
                        TransactionCategoryId = defaultCategory.TransactionCategoryId,
                        LimitAmount = 0
                    };
                    
                    break;
                case CommandType.Update:
                    this.CategoryId = model.TransactionCategoryId.ToString();
                    this.BudgetItemModel = model;
                    break;
                default:
                    return;
            }

            this.OriginalBudgetItem = new BudgetItemModel
            {
                TransactionCategoryId = this.BudgetItemModel.TransactionCategoryId,
                LimitAmount = this.BudgetItemModel.LimitAmount
            };

            this.command = command;
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
            this.command = null;
            this.BudgetItemModel.TransactionCategoryId = (int)defaultCategory.TransactionCategoryId;
            this.BudgetItemModel.LimitAmount = 0;
            this.CategoryId = defaultCategory.TransactionCategoryId.ToString();
        }
    }
}
