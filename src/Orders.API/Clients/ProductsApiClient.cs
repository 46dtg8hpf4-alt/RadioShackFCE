using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Orders.API.DTOs;

namespace Orders.API.Clients
{
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
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDTO>();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return null;
        }
    }
}
