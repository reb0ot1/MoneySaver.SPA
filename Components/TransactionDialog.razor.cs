using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;
namespace MoneySaver.SPA.Components
{
    public partial class TransactionDialog
    {
        private bool forUpdate = false;
        private Transaction originalTransaction = null;
        public Transaction Transaction { get; set; }

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
            if (transaction != null)
            {
                this.Transaction = transaction;
                this.originalTransaction = this.Transaction.Copy();
                this.CategoryId = transaction.TransactionCategoryId.ToString();
                this.forUpdate = true;
            }
            else
            {
                this.Transaction = new Transaction
                {
                    TransactionDate = DateTime.UtcNow
                };

                this.CategoryId = this.TransactionCategories.First().TransactionCategoryId.ToString();
                this.forUpdate = false;
            }
            
            this.ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            if (this.forUpdate)
            {
                this.Transaction.Amount = this.originalTransaction.Amount;
                this.Transaction.TransactionDate = this.originalTransaction.TransactionDate;
                this.Transaction.TransactionCategoryId = this.originalTransaction.TransactionCategoryId;
                this.Transaction.AdditionalNote = this.originalTransaction.AdditionalNote;
                this.CategoryId = this.originalTransaction.TransactionCategoryId.ToString();
            }
            
            this.ShowDialog = false;
            StateHasChanged();
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
                this.Transaction.Id = Guid.NewGuid();
                await this.TransactionService.AddAsync(this.Transaction);
            }
            
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }   
}
