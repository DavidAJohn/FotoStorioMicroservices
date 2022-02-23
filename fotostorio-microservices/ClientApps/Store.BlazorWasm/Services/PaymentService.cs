using Blazored.LocalStorage;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Store.BlazorWasm.Services;

public class PaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;

    public PaymentService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(Basket basket)
    {
        var storedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(storedToken))
        {
            return null;
        }

        try
        {
            var client = _httpClient.CreateClient("StoreGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

            var intentToCreate = new PaymentIntentCreateDTO
            {
                Items = basket.BasketItems,
                PaymentIntentId = basket.PaymentIntentId
            };

            HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(intentToCreate));
            serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsync("Payments", serializedContent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var createdIntent = JsonSerializer.Deserialize<PaymentIntentResult>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return createdIntent;
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
