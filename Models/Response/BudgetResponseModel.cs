using MoneySaver.SPA.Models.Enums;
using System.Text.Json.Serialization;

namespace MoneySaver.SPA.Models.Response
{
    public class BudgetResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //TODO: Use enum option name instead enum option value
        public BudgetType BudgetType { get; set; }

        public bool IsInUse { get; set; }

        //TODO: Think if the currency type should be included
    }

    public class BudgetResponseApiModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("budgetType")]
        //TODO: Use enum option name instead enum option value
        public BudgetType BudgetType { get; set; }

        [JsonPropertyName("isInUse")]
        public bool IsInUse { get; set; }

        //TODO: Think if the currency type should be included
    }
}
