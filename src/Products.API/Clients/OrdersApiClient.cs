namespace Products.API.Clients
{
    public class OrdersApiClient
    {
        private readonly HttpClient _httpClient;

        public OrdersApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ProductHasActiveOrders(Guid productId)
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    $"http://localhost:5232/api/orders/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {

                return false;

            }
            
            string content =

                await response.Content.ReadAsStringAsync();

            return bool.Parse(content);
        }
    }
}