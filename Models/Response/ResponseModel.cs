using System.Net;

namespace MoneySaver.SPA.Models.Response;

public class ResponseModel<T>
{
    public bool IsUnauthorized { get; set; }
    public string ErrorMessage { get; set; }
    public T Data { get; set; }
}