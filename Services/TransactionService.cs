using Microsoft.Extensions.Options;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using MoneySaver.SPA.Models.Enums;
using System.Text.Json;

namespace MoneySaver.SPA.Services
{
    public class TransactionService : ITransactionService
    {
        private string baseUrl;
        private HttpClient httpClient;
        private HttpClient httpClient2;
        public TransactionService(IHttpClientFactory httpClientFactory, HttpClient httpClient, IOptions<SpaSettings> spaSettingsConfiguration)
        {
            this.httpClient2 = httpClientFactory.CreateClient("apiCallsTransactions");
            this.httpClient = httpClient;
            this.baseUrl = spaSettingsConfiguration.Value.DataApiAddress + "api/transaction";
        }

        public async Task<TransactionsPageModel> GetForPage(int itemsToSkip, int itemsPerPage, string search)
        {
            var transactionJson = RequestContent.CreateContent(new { ItemsToSkip = itemsToSkip, ItemsPerPage = itemsPerPage, Filter = new { SearchText = search } });

            try
            {
                var response = await this.httpClient2.PostAsync($"api/transaction/page", transactionJson);
                if (response.IsSuccessStatusCode)
                {
                    TransactionsPageModel result = null;
                    using (var responseResult = await response.Content.ReadAsStreamAsync())
                    {
                        result = await JsonSerializer.DeserializeAsync<TransactionsPageModel>(responseResult, new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {

            }

           return null;
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            var transactionJson = RequestContent.CreateContent(transaction);

            var response = await this.httpClient.PostAsync(baseUrl, transactionJson);
            if (response.IsSuccessStatusCode)
            {
                using var responseResult = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Transaction>(responseResult);
            }

            return null;
        }

        public async Task<IEnumerable<Transaction>> AddManyAsync(IEnumerable<Transaction> transactions)
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            foreach (var transaction in transactions)
            {
                var transactionJson = RequestContent.CreateContent(transaction);

                var response = this.httpClient.PostAsync(baseUrl, transactionJson);
                tasks.Add(response);
            }

            await Task.WhenAll<HttpResponseMessage>(tasks);

            var result = new List<Transaction>();

            foreach (var task in tasks)
            {
                if (task.Result.IsSuccessStatusCode)
                {
                    using var responseResult = await task.Result.Content.ReadAsStreamAsync();
                    var deserializeResult = await JsonSerializer.DeserializeAsync<Transaction>(responseResult);

                    result.Add(deserializeResult);
                }
            }

            return result;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            var transactionJson = RequestContent.CreateContent(transaction);

            await this.httpClient.PutAsync(baseUrl, transactionJson);
        }

        public async Task DeleteAsync(string transactionId)
        {
            await this.httpClient.GetAsync($"{baseUrl}/remove/{transactionId}");
        }
    }
}
