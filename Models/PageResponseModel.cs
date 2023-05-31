using System.Text.Json.Serialization;

namespace MoneySaver.SPA.Models
{
    public class PageResponseModel<T> where T : class
    {
        public PageResponseModel()
        {
            this.Result = new List<T>();
        }

        public IEnumerable<T> Result { get; set; }

        public int TotalCount { get; set; }
    }

    public class PageResponseApiModel<T> where T : class
    {
        public PageResponseApiModel()
        {
            this.Result = new List<T>();
        }

        [JsonPropertyName("result")]
        public IEnumerable<T> Result { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }

}
