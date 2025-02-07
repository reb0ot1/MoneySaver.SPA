using System.Net;
using Microsoft.Extensions.Options;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Enums;
using System.Text.Json;
using MoneySaver.SPA.Exceptions;
using MoneySaver.SPA.Models.Request;

namespace MoneySaver.SPA.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IApiCallsService _apiCallsService;
        public TransactionService(
            IApiCallsService apiCallsService)
        {
            this._apiCallsService = apiCallsService;
        }

        public async Task<TransactionsPageModel> GetForPage(int itemsToSkip, int itemsPerPage, string search)
        {
            var transactionJson = new PageRequestModel { ItemsToSkip = itemsToSkip, ItemsPerPage = itemsPerPage, Filter = new FilterRequestModel { SearchText = search }};

            var response = await this._apiCallsService.PostAsync<PageRequestModel, TransactionsPageModel>($"api/transaction/page", transactionJson);

            return response;
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            var response = await this._apiCallsService.PostAsync<Transaction, Transaction>("api/transaction", transaction);
            
            return response;
        }

        public async Task<IEnumerable<Transaction>> AddManyAsync(IEnumerable<Transaction> transactions)
        {
            var tasks = new List<Task<Transaction>>();
            foreach (var transaction in transactions)
            {
                var response = this._apiCallsService.PostAsync<Transaction, Transaction>("api/transaction", transaction);
                tasks.Add(response);
            }

            await Task.WhenAll(tasks);

            return tasks.Select(task => task.Result).ToList();
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            var result = await this._apiCallsService.PutAsync<Transaction, Transaction>("api/transaction", transaction);
        }

        public async Task DeleteAsync(string transactionId)
        {
            await this._apiCallsService.DeleteAsync($"api/transaction/remove/{transactionId}");
        }
    }
}
