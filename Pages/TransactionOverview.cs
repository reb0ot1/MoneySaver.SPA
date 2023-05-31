using MoneySaver.SPA.Components;
using MoneySaver.SPA.Models;
using Microsoft.AspNetCore.Components;
using MoneySaver.SPA.Services;
using Radzen;
using Radzen.Blazor;

namespace MoneySaver.SPA.Pages
{
    public partial class TransactionOverview
    {
        protected RadzenGrid<Transaction> grid;

        [Inject]
        public NavigationManager navigation { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }

        [Inject]
        public ICategoryService CategoryService { get; set; }

        [Inject]
        public ITransactionService TransactionService { get; set; }

        public IEnumerable<TransactionCategory> TransactionCategories { get; set; }

        public int TotalCount { get; set; } = 0;

        public int SkipedItems { get; set; } = 0;

        public string Search { get; set; }

        protected async override Task OnInitializedAsync()
        {
            TransactionCategories = await CategoryService.GetAllPreparedForVisualizationAsync();

            await this.ManageGridData();
        }

        public async Task LoadGridData(LoadDataArgs args)
        {
            this.SkipedItems = args.Skip ?? 0;
            
            await this.ManageGridData();
        }

        public async void OnValueChange(ChangeEventArgs e)
        {
            this.SkipedItems = 0;
            this.Search = e.Value.ToString();
            await grid.FirstPage();
            await ManageGridData();
            await InvokeAsync(StateHasChanged);
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
            var pageResult = await this.TransactionService.GetForPage(this.SkipedItems, Constants.ITEMS_PER_PAGE, null);
            this.TotalCount = pageResult.TotalCount;
            Transactions = pageResult.Result;
            StateHasChanged();
        }

        private async Task ManageGridData()
        {
            var pageResult = await this.TransactionService.GetForPage(this.SkipedItems, Constants.ITEMS_PER_PAGE, this.Search);
            this.TotalCount = pageResult.TotalCount;
            this.Transactions = pageResult.Result;
        }
    }
}
