using System.Text.Json;
using System.Text;

namespace MoneySaver.SPA.Extensions
{
    public static class RequestContent
    {
        public static StringContent CreateContent<T>(T entity)
        { 
            return new StringContent(
                JsonSerializer.Serialize(entity),
                Encoding.UTF8, "application/json"
                );
        }
    }
}
