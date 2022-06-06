using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Pages;
using MoneySaver.SPA.Services;
namespace MoneySaver.SPA.Components
{
    public partial class TransactionDialog
    {
        private bool forUpdate = false;
        public Transaction Transaction { get; set; }
            = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionDate = DateTime.Now,
            };

        public bool ShowDialog { get; set; }

        protected string CategoryId = string.Empty;

        [Inject]
        public ITransactionService TransactionService { get; set; }

        [Parameter]
        public TransactionCategory[] TransactionCategories { get; set; }

        [Parameter]
        public EventCallback<bool> SaveTransactionCallback { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public void Show(Transaction transaction = null)
        {
            ResetDialog();
            if (transaction != null)
            {
                this.Transaction = transaction;
                this.CategoryId = transaction.TransactionCategoryId.ToString();
                this.forUpdate = true;
            }
            else
            {
                this.CategoryId = this.TransactionCategories
                .First().TransactionCategoryId
                .ToString();
                this.forUpdate = false;
            }
            
            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            this.ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            this.CategoryId = default;
            this.Transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                TransactionDate = DateTime.Now,
                TransactionCategoryId = (int)TransactionCategories.First().TransactionCategoryId
            };
            this.forUpdate = false;
        }

        protected async Task HandleValidSubmit()
        {
            ShowDialog = false;
            this.Transaction.TransactionCategoryId = int.Parse(CategoryId);
            if (this.forUpdate)
            {
                await this.TransactionService.UpdateAsync(this.Transaction);
            }
            else
            {
                await this.TransactionService.AddAsync(this.Transaction);
            }
            
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }   
}
