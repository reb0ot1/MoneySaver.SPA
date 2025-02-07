using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MoneySaver.SPA.Extensions;
using MoneySaver.SPA.Models.Configurations;

namespace MoneySaver.SPA.Services;

public class ApiCallsService : IApiCallsService
{
    private readonly HttpClient _httpClient;
    private readonly SpaSettings _spaSettings;
    private readonly Uri _baseUri;

    public ApiCallsService(HttpClient httpClient, IOptions<SpaSettings> spaSettingsOption)
    {
        this._httpClient = httpClient;
        this._spaSettings = spaSettingsOption.Value;
        this._baseUri = new Uri(this._spaSettings.DataApiAddress);
    }
    
    public async Task<T> GetAsync<T>(string path)
    {   
        var response = await this._httpClient.GetAsync(new Uri(this._baseUri, path));
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception("Failed to get data");
        }
        
        var resultAsString = await response.Content.ReadAsStreamAsync(); 
        var result = await JsonSerializer.DeserializeAsync<T>(resultAsString,new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return result;
    }

    public async Task<TC> PostAsync<T, TC>(string path, T content)
    {
        var payload = RequestContent.CreateContent(content);
        var response = await this._httpClient.PostAsync(new Uri(this._baseUri, path), payload);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception("Failed to post data");
        }

        var result = await JsonSerializer.DeserializeAsync<TC>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return result;
    }

    public async Task<TC> PutAsync<T, TC>(string path, T content)
    {
        var payload = RequestContent.CreateContent(content);

        var response = await this._httpClient.PutAsync(new Uri(this._baseUri, path), payload);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception("Failed to put data");
        }
        var result = await JsonSerializer.DeserializeAsync<TC>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return result;
    }

    public async Task DeleteAsync(string path)
    {
        var response = await this._httpClient.DeleteAsync(new Uri(this._baseUri, path));

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (response.IsSuccessStatusCode is false)
        {
            throw new Exception("Failed to delete data");
        }
        
        //TODO: Need to return something
    }
}