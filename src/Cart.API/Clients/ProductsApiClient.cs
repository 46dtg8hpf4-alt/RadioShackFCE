using Cart.API.DTOs;

namespace Cart.API.Clients;

public class ProductsApiClient
{
    private readonly HttpClient _httpClient;

    public ProductsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductDTO?> GetProductAsync(Guid productId)
    {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDTO>();
        return product;
    }
}
