using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Services;
namespace MoneySaver.SPA.Components
{
    public partial class TransactionDialog
    {
        private int Index = 0;
        private bool forUpdate = false;
        private int MaxTransactionsToAdd = 10;
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

        public static List<TransactionVM> TransactionsToAdd { get; set; } = new List<TransactionVM>();

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
            else
            {
                TransactionsToAdd.Clear();
                Index = 0;
            }
            
            this.ShowDialog = false;
            StateHasChanged();
        }

        public void Add()
        {
            if (TransactionsToAdd.Count == MaxTransactionsToAdd)
            {
                return;
            }

            TransactionsToAdd.Add(new TransactionVM
            {
                Index = this.Index,
                TransactionDate = DateTime.UtcNow,
                TransactionCategoryId = this.TransactionCategories.First().TransactionCategoryId,
                Amount = 0,
                AdditionalNote = string.Empty
            });

            Index++;
        }

        public void Remove(int index)
        {
            TransactionsToAdd.RemoveAt(index);
            
            Index = 0;
            foreach (var item in TransactionsToAdd)
            {
                item.Index = Index;
                Index++;
            }

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
                var transactionsToAdd = new List<Transaction>();
                foreach (var item in TransactionsToAdd)
                {
                    transactionsToAdd.Add(new Transaction { 
                       Id = Guid.NewGuid(),
                       AdditionalNote = item.AdditionalNote,
                       Amount = item.Amount,
                       TransactionCategoryId= item.TransactionCategoryId,
                       TransactionDate = item.TransactionDate
                    });
                }

                await this.TransactionService.AddManyAsync(transactionsToAdd);

                TransactionsToAdd.Clear();
                Index = 0;
            }
            
            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
