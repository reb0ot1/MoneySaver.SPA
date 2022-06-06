using MoneySaver.SPA.Models;
using MoneySaver.SPA.Models.Configurations;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneySaver.SPA.Services
{
    public class TransactionService : ITransactionService
    {
        private string baseUrl;
        //TODO: Change HttpClient with httpClientFactory
        private HttpClient httpClient { get; set; }

        public TransactionService(HttpClient httpClient, DataApi dataApi)
        {
            this.httpClient = httpClient;
            this.baseUrl = dataApi.Url + "api/transaction";
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            IEnumerable<Transaction> result = new List<Transaction>();
            result = await httpClient
                .GetFromJsonAsync<IEnumerable<Transaction>>(baseUrl);

            return result;
        }

        public async Task<TransactionsPageModel> GetForPage(int itemsToSkip, int itemsPerPage)
        {
            var transactionJson = new StringContent(
                JsonSerializer.Serialize(new { ItemsToSkip = itemsToSkip, ItemsPerPage = itemsPerPage}),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync($"{baseUrl}/page", transactionJson);
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

           return null;
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            var transactionJson = new StringContent(
                JsonSerializer.Serialize(transaction),
                Encoding.UTF8, "application/json"
                );

            var response = await this.httpClient.PostAsync(baseUrl, transactionJson);
            if (response.IsSuccessStatusCode)
            {
                using var responseResult = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Transaction>(responseResult);
            }

            return null;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            var transactionJson = new StringContent(JsonSerializer.Serialize(transaction), Encoding.UTF8, "application/json");

            await this.httpClient.PutAsync(baseUrl, transactionJson);
        }

        public async Task DeleteAsync(string transactionId)
        {
            await this.httpClient.GetAsync($"{baseUrl}/remove/{transactionId}");
        }
    }
}
