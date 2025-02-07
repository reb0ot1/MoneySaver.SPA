namespace MoneySaver.SPA.Services;

public interface IApiCallsService
{
    Task<T> GetAsync<T>(string url);
    
    Task<TC> PostAsync<T, TC>(string path, T content);
    
    Task<TC> PutAsync<T, TC>(string path, T content);
    
    Task DeleteAsync(string path);
}