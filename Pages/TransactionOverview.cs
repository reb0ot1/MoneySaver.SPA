using MoneySaver.SPA.Components;
using MoneySaver.SPA.Models;
using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Services;
using Radzen;

namespace MoneySaver.SPA.Pages
{
    public partial class TransactionOverview
    {
        protected const int ItemsPerPage = 50;

        public IEnumerable<Transaction> Transactions { get; set; }

        [Inject]
        public ICategoryService CategoryService { get; set; }

        [Inject]
        public ITransactionService TransactionService { get; set; }

        public IEnumerable<TransactionCategory> TransactionCategories { get; set; }

        public int TotalCount { get; set; } = 0;

        public int SkipedItems { get; set; } = 0;

        public async Task LoadData(LoadDataArgs args)
        {
            var skip = args.Skip ?? 0;
            this.SkipedItems = skip;
            var pageResult = await this.TransactionService.GetForPage(this.SkipedItems, ItemsPerPage);
            this.TotalCount = pageResult.TotalCount;
            Transactions = pageResult.Result;
        }

        protected async override Task OnInitializedAsync()
        {
            var result = await CategoryService.GetAllAsync();
            TransactionCategories = this.PrepareForVisualization(result);

            var pageResult = await this.TransactionService.GetForPage(0, ItemsPerPage);
            this.TotalCount = pageResult.TotalCount;
            this.Transactions = pageResult.Result;
        }

        protected TransactionDialog TransactionDialog { get; set; }

        protected ConfirmationDialog ConfirmationDialog { get; set; }

        protected void AddTransaction()
        {
            TransactionDialog.Show();
        }

        protected void UpdateTransaction(Guid transactionId)
        {
            TransactionDialog.Show(this.Transactions.FirstOrDefault(f => f.Id == transactionId));
        }

        protected void DeleteOperation(Guid transactionId)
        {
            this.ConfirmationDialog.Show(transactionId.ToString());
        }

        public async void DeleteTransaction(string transactionId)
        {
            await this.TransactionService.DeleteAsync(transactionId);
            await OnDialogClose(true);
        }

        public async Task OnDialogClose(bool result)
        {
            var pageResult = await this.TransactionService.GetForPage(this.SkipedItems, ItemsPerPage);
            this.TotalCount = pageResult.TotalCount;
            Transactions = pageResult.Result;
            StateHasChanged();
        }

        //TODO: This logic should be moved to the API ???
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

            return categories.OrderBy(e => e.AlternativeName).ToList();
        }
    }
}
