namespace Products.API.Clients
{
    public class OrdersApiClient
    {
        private readonly HttpClient _httpClient;

        public OrdersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ProductHasActiveOrders(int productId)
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"http://localhost:5000/api/orders/product/{productId}");

            return response.IsSuccessStatusCode;
        }
    }
}